using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.AbTesting.Mvc
{
    /// <summary>
    /// Проверяет наличие или значение referrer
    /// </summary>
    public class RequireReferrerAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// проверять домен referrer
        /// </summary>
        public bool CheckOrigin { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var url = filterContext.HttpContext.Request.Headers["Referer"].ToString();
            //if (url == null)
            //{
            //    filterContext.Result = new HttpStatusCodeResult(403, "not allowed!");
            //}

            //if (CheckOrigin)
            //{
            //    var requireReffererDomains = ConfigurationManager.AppSettings["AbTest.RequireReffererDomains"] ?? "";
            //    var domains = requireReffererDomains.Split(new[] { ";", "," }, System.StringSplitOptions.RemoveEmptyEntries);

            //    if (domains.Length > 0 && url != null)
            //    {
            //        var host = url.DnsSafeHost;
            //        if (host.IsNullOrEmpty())
            //            return;

            //        var isAllowed = false;
            //        foreach (var domain in domains)
            //        {
            //            if (host.EndsWith(domain) || host.Equals(domain.TrimStart(".")))
            //                isAllowed = true;
            //        }

            //        if (!isAllowed)
            //            filterContext.Result = new HttpStatusCodeResult(403, "not allowed for this origin!");
            //    }
            //}

            //not implemented yet
            base.OnActionExecuting(filterContext);
        }
    }
}
