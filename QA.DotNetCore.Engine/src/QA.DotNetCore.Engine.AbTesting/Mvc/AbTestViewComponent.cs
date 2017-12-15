using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace QA.DotNetCore.Engine.AbTesting.Mvc
{
    public class AbTestViewComponent : ViewComponent
    {
        private readonly IAbTestService _abTestService;

        public AbTestViewComponent(IAbTestService abTestService)
        {
            _abTestService = abTestService;
        }

        public HtmlString Invoke()
        {
            try
            {
                var hasContainers = _abTestService.HasContainersForPage(HttpContext.Request.Host.Value, HttpContext.Request.Path);

                if (hasContainers)
                {
                    string url = AbTestControllerUrl();

                    return new HtmlString($@"
<script> 
        var qa_abtScriptUrl = '{url}';
        qa_abtScriptUrl += '/?p=' + encodeURIComponent(window.location.pathname) + '&d=' + encodeURIComponent(window.location.host) + '&t=' + (new Date()).getTime();
        if (window.location.href.indexOf('?') > 0){{
            var qparams = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for(var i = 0; i < qparams.length; i++)
            {{
                if (qparams[i].indexOf('{AbTestChoiceResolver.QueryParamPrefix}')==0)
                    qa_abtScriptUrl += '&' + qparams[i];
            }}
        }}
       
        document.write(""<scr"" + ""ipt src='""+qa_abtScriptUrl+""'></sc"" + ""ript>"");
</script>"
);
                }
            }
            catch (Exception e)
            {
                return new HtmlString("<!-- Error while initializing A/B tests container is occured. -->");
            }

            return new HtmlString("<!-- No A/B tests for this page. -->");
        }

        private string AbTestControllerUrl()
        {
            var url = Url.Action("InlineScript", "AbTest").ToLower();
            var indexOf = url.IndexOf('?');
            if (indexOf > -1)
            {
                url = url.Substring(0, indexOf);
            }
            url = url.TrimEnd('/');
            return url;
        }
    }
}
