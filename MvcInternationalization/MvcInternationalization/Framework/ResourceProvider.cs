using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Xml;

namespace MvcInternationalization.Framework
{
    public static class ResourceProvider
    {
        /// <summary>
        /// 資源儲存位置
        /// </summary>
        private const string ResourcePath = "~/App_Data/Language.xml";

        /// <summary>
        /// 快取檔案
        /// </summary>
        private const string LanguageCacheName = "LanguagePack.xml";

        /// <summary>
        /// 默認語言
        /// </summary>
        private const string DefaultCulture = "en-US";

        /// <summary>
        /// CultureInfo.CurrentCultureName (獲取當前語言)
        /// 可以設置語言環境，然後得到指定語言的資源，默認為 en-US
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string R(string culture, string name)
        {
            Dictionary<string, Dictionary<CultureInfo, string>> res = null;

            // 緩存到內存
            if (HttpRuntime.Cache[LanguageCacheName] == null)
            {
                var fullPath = System.Web.HttpContext.Current.Server.MapPath(ResourcePath);
                if (!System.IO.File.Exists(fullPath))
                    throw new Exception("資源文件不存在！");

                XmlDocument doc = new XmlDocument();
                doc.Load(fullPath);

                var dicts = doc.SelectNodes("dicts/dict");
                res = new Dictionary<string, Dictionary<CultureInfo, string>>();

                // 遞歸資源
                foreach (XmlNode item in dicts)
                {
                    var kvs = new Dictionary<CultureInfo, string>();
                    foreach (XmlNode el in item.ChildNodes)
                    {
                        kvs.Add(new CultureInfo(el.Name), el.InnerText);
                    }
                    res.Add(item.Attributes["name"].Value, kvs);
                }

                // 插入緩存依賴於文件本身
                HttpRuntime.Cache.Insert(LanguageCacheName, res, new System.Web.Caching.CacheDependency(fullPath));
            }
            else
            {
                res = HttpRuntime.Cache[LanguageCacheName] as Dictionary<string, Dictionary<CultureInfo, string>>;
            }

            try
            {
                if (res[name] != null)
                {
                    if (res[name][new CultureInfo(culture)] != null)
                    {
                        return res[name][new CultureInfo(culture)];
                    }
                    else
                    {
                        return res[name][new CultureInfo(DefaultCulture)];
                    }
                }
                return res[name][new CultureInfo(culture)];
            }
            catch
            {
                // 加載失敗返回空字串
                return string.Empty;
            }
        }

        public static string R(string name)
        {
            return R(Culture, name);
        }

        private const string CultureCookieName = "Culture";

        /// <summary>
        /// 獲取或設置語言配置。(這裡使用的是Cookie，當然也能自己實現登錄帳號的profile配置)
        /// </summary>
        public static string Culture
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies[CultureCookieName];
                if (cookie == null)
                    return CultureInfo.CurrentCulture.Name;
                return cookie.Value;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var cookie = HttpContext.Current.Request.Cookies[CultureCookieName];
                    if (cookie != null)
                    {
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }

                    cookie = new HttpCookie(CultureCookieName, value)
                    {
                        Expires = DateTime.Now.AddYears(1)
                    };

                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }
    }
}