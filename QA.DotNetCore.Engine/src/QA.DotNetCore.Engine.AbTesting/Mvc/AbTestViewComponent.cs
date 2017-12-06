using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QA.DotNetCore.Engine.AbTesting.Mvc
{
    public class AbTestViewComponent : ViewComponent
    {
        readonly AbTestStorage _abTestStorage;

        public AbTestViewComponent(AbTestStorage abTestStorage)
        {
            _abTestStorage = abTestStorage;
        }

        public HtmlString Invoke()
        {
            try
            {
                var hasContainers = _abTestStorage.HasContainersForPage(HttpContext.Request.Path);

                if (hasContainers)
                {
                    var url = Url.Action("InlineScript", "AbTest").ToLower().TrimEnd('/');

                    return new HtmlString($@"
<script> 
        var qa_abtScriptUrl = '{url}';
        qa_abtScriptUrl += '/?u=' + encodeURIComponent(window.location.href) + '&t=' + (new Date()).getTime();
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
            catch
            {
                return new HtmlString("<!-- Error while initializing A/B tests container is occured. -->");
            }

            return new HtmlString("<!-- No A/B tests for this page. -->");
        }
    }
}
