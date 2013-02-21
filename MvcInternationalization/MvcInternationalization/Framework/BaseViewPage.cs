using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcInternationalization.Framework
{
    public class BaseViewPage<TModel> : WebViewPage<TModel> where TModel : class
    {
        /// <summary>
        /// 返回 HtmlString 本地化字串
        /// </summary>
        /// <param name="neutralValue"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IHtmlString T(string neutralValue, params object[] args)
        {
            return new HtmlString(S(neutralValue, args));
        }

        /// <summary>
        /// 返回本地化字串
        /// </summary>
        /// <param name="neutralValue"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual string S(string neutralValue, params object[] args)
        {
            return string.Format(ResourceProvider.R(neutralValue), args);
        }

        /// <summary>
        /// 返回本地化字串
        /// </summary>
        /// <param name="neutralValue"></param>
        /// <returns></returns>
        public virtual string S(string neutralValue)
        {
            return ResourceProvider.R(neutralValue);
        }

        public override void Execute()
        {
            
        }

        protected override void InitializePage()
        {
            // 設置 UICulture
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(ResourceProvider.Culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(ResourceProvider.Culture);

            base.InitializePage();
        }
    }
}