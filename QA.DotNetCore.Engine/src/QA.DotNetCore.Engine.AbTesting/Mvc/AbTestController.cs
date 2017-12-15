using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.AbTesting.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace QA.DotNetCore.Engine.AbTesting.Mvc
{
    public class AbTestController : Controller
    {
        private readonly IAbTestService _abTestService;
        private readonly ITargetingContext _targetingContext;
        private readonly AbTestChoiceResolver _abTestChoiceResolver;

        public AbTestController(IAbTestService abTestService, AbTestChoiceResolver abTestChoiceResolver, ITargetingContext targetingContext)
        {
            _abTestService = abTestService;
            _abTestChoiceResolver = abTestChoiceResolver;
            _targetingContext = targetingContext;
        }

        //[NoCache]
        [RequireReferrer(CheckOrigin = true)]
        public virtual ActionResult InlineScript(string d, string p)
        {
            Response.ContentType = "application/x-javascript";

            if (string.IsNullOrEmpty(d))
            {
                return Content("/* ABTEST ERROR! parameter d is null or empty.*/");
            }
            if (string.IsNullOrEmpty(p))
            {
                return Content("/* ABTEST ERROR! parameter p is null or empty.*/");
            }

            var tests = _abTestService.GetTestsWithContainersForPage(d, p);
            if (tests != null && tests.Any())
            {
                //построим js-объект для таргетирования тестов
                var jsCodeForTargetingObject = String.Empty;
                var keys = _targetingContext.GetTargetingKeys();
                if (keys != null)
                {
                    jsCodeForTargetingObject = String.Join(", ", keys.Select(k => $"{k}: {JsStringifyObject(_targetingContext.GetTargetingValue(k))}"));
                }

                const string cookiesJs = "!function(e,t){\"use strict\";var o=function(e){if(\"object\"!=typeof e.document)throw new Error(\"Cookies.js requires a `window` with a `document` object\");var t=function(e,o,n){return 1===arguments.length?t.get(e):t.set(e,o,n)};return t._document=e.document,t._cacheKeyPrefix=\"cookey.\",t._maxExpireDate=new Date(\"Fri, 31 Dec 9999 23:59:59 UTC\"),t.defaults={path:\"/\",secure:!1},t.get=function(e){t._cachedDocumentCookie!==t._document.cookie&&t._renewCache();var o=t._cache[t._cacheKeyPrefix+e];return void 0===o?void 0:decodeURIComponent(o)},t.set=function(e,o,n){return n=t._getExtendedOptions(n),n.expires=t._getExpiresDate(void 0===o?-1:n.expires),t._document.cookie=t._generateCookieString(e,o,n),t},t.expire=function(e,o){return t.set(e,void 0,o)},t._getExtendedOptions=function(e){return{path:e&&e.path||t.defaults.path,domain:e&&e.domain||t.defaults.domain,expires:e&&e.expires||t.defaults.expires,secure:e&&void 0!==e.secure?e.secure:t.defaults.secure}},t._isValidDate=function(e){return\"[object Date]\"===Object.prototype.toString.call(e)&&!isNaN(e.getTime())},t._getExpiresDate=function(e,o){if(o=o||new Date,\"number\"==typeof e?e=e===1/0?t._maxExpireDate:new Date(o.getTime()+1e3*e):\"string\"==typeof e&&(e=new Date(e)),e&&!t._isValidDate(e))throw new Error(\"`expires` parameter cannot be converted to a valid Date instance\");return e},t._generateCookieString=function(e,t,o){var n=(e=(e=e.replace(/[^#$&+\\^`|]/g,encodeURIComponent)).replace(/\\(/g,\"%28\").replace(/\\)/g,\"%29\"))+\"=\"+(t=(t+\"\").replace(/[^!#$&-+\\--:<-\\[\\]-~]/g,encodeURIComponent));return n+=(o=o||{}).path?\";path=\"+o.path:\"\",n+=o.domain?\";domain=\"+o.domain:\"\",n+=o.expires?\";expires=\"+o.expires.toUTCString():\"\",n+=o.secure?\";secure\":\"\"},t._getCacheFromString=function(e){for(var o={},n=e?e.split(\"; \"):[],r=0;r<n.length;r++){var i=t._getKeyValuePairFromCookieString(n[r]);void 0===o[t._cacheKeyPrefix+i.key]&&(o[t._cacheKeyPrefix+i.key]=i.value)}return o},t._getKeyValuePairFromCookieString=function(e){var t=e.indexOf(\"=\");t=t<0?e.length:t;var o,n=e.substr(0,t);try{o=decodeURIComponent(n)}catch(e){console&&\"function\"==typeof console.error&&console.error('Could not decode cookie with key \"'+n+'\"',e)}return{key:o,value:e.substr(t+1)}},t._renewCache=function(){t._cache=t._getCacheFromString(t._document.cookie),t._cachedDocumentCookie=t._document.cookie},t._areEnabled=function(){var e=\"1\"===t.set(\"cookies.js\",1).get(\"cookies.js\");return t.expire(\"cookies.js\"),e},t.enabled=t._areEnabled(),t},n=e&&\"object\"==typeof e.document?o(e):o;\"function\"==typeof define&&define.amd?define(function(){return n}):\"object\"==typeof exports?(\"object\"==typeof module&&\"object\"==typeof module.exports&&(exports=module.exports=n),exports.Cookies=n):e.Cookies=n}(\"undefined\"==typeof window?this:window);";

                return Content($@"
{cookiesJs}
(function(window, cookies){{
    ""use strict""
    var ctx = {{ {jsCodeForTargetingObject} }};
    window.abTestingContext = {{}};
    {JsCodeForTests(tests)}
}})(window, Cookies);
");
            }

            return Content("/* no tests for ab */");
        }

        private string JsCodeForSetCookies(AbTestPersistentData test, int choice)
        {
            var cookieExpireDate = test.EndDate ?? DateTime.Today.AddYears(1);
            return $@"
    var options = {{}};
    options.expires = new Date({cookieExpireDate.Year},{cookieExpireDate.Month - 1},{cookieExpireDate.Day},{cookieExpireDate.Hour},{cookieExpireDate.Minute})
    options.path = '{Url.Action("InlineScript", "AbTest").ToLower()}';
    cookies.set('{AbTestChoiceResolver.CookieNamePrefix + test.Id}', '{choice}', options);
    window.abTestingContext['{AbTestChoiceResolver.CookieNamePrefix + test.Id}'] = {{ choice: {choice}, title: '{test.Title}', comment: '{test.Comment}', percentage: [{String.Join(",", test.Percentage)}]}};
";
        }

        private string JsCodeForTests(AbTestWithContainers[] abTestWithContainers)
        {
            var sb = new StringBuilder();
            foreach (var test in abTestWithContainers)
            {
                var choice = _abTestChoiceResolver.ResolveChoice(test.Test.Id);
                sb.Append(JsCodeForSetCookies(test.Test, choice));
                if (test.ClientRedirectContainer != null)
                {
                    var redirect = test.ClientRedirectContainer.Redirects.FirstOrDefault(r => r.VersionNumber == choice && !String.IsNullOrWhiteSpace(r.RedirectUrl));
                    if (redirect != null)
                    {
                        var precondition = test.ClientRedirectContainer.Precondition;
                        if (String.IsNullOrWhiteSpace(precondition))
                        {
                            precondition = "true";
                        }
                        sb.Append($@"
(function(ctx, window){{
    if(ctx && !({precondition})) return;
    window.location = '{redirect.RedirectUrl}';
}})(ctx, window);
");
                        break;
                    }
                }

                foreach (var container in test.ScriptContainers.Where(c => c.Scripts.Any(s => s.VersionNumber == choice)))
                {
                    var precondition = container.Precondition;
                    if (String.IsNullOrWhiteSpace(precondition))
                    {
                        precondition = "true";
                    }
                    sb.Append($@"
(function(ctx, window){{
    if(ctx && !({precondition})) return;
    {container.Scripts.First(_ => _.VersionNumber == choice).ScriptText}
}})(ctx, window);
");
                }
            }
            return sb.ToString();
        }

        private string JsStringifyObject(object obj)
        {
            if (obj is string str)
            {
                return "'" + str + "'";
            }
            if (obj is IEnumerable enumerable)
            {
                var sb = new StringBuilder("[");
                foreach (var item in enumerable)
                {
                    sb.Append(item);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
                return sb.ToString();
            }
            return "'" + obj.ToString() + "'";
        }
    }
}
