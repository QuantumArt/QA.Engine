using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.AbTesting.Mvc
{
    public class AbTestController : Controller
    {
        readonly AbTestStorage _abTestStorage;
        readonly ITargetingContext _targetingContext;
        readonly AbTestChoiceResolver _abTestChoiceResolver;

        public AbTestController(AbTestStorage abTestStorage, AbTestChoiceResolver abTestChoiceResolver, ITargetingContext targetingContext)
        {
            _abTestStorage = abTestStorage;
            _abTestChoiceResolver = abTestChoiceResolver;
            _targetingContext = targetingContext;
        }

        //[NoCache]
        [RequireReferrer(CheckOrigin = true)]
        public virtual ActionResult InlineScript(string u)
        {
            Response.ContentType = "application/x-javascript";

            if (string.IsNullOrEmpty(u))
            {
                return Content("/* ABTEST ERROR! parameter u is null or empty.*/");
            }

            var tests = _abTestStorage.GetTestsWithContainersForPage(u);
            if (tests != null && tests.Any())
            {
                //построим js-объект для таргетирования тестов
                var jsCodeForTargetingObject = String.Empty;
                var keys = _targetingContext.GetTargetingKeys();
                if (keys != null)
                {
                    jsCodeForTargetingObject = String.Join(", ", keys.Select(k => $"{k}: '{_targetingContext.GetTargetingValue(k)}'"));
                }

                return Content($@"
(function(window, $){{
    ""use strict""
    var ctx = {{ {jsCodeForTargetingObject} }};
    {JsCodeForTests(tests)}
}})(window, jQuery);
");
            }

            return Content("/* no tests for ab */");
        }

        private string JsCodeForSetCookies(AbTestPersistentData test, int choice)
        {
            var cookieExpireDate = test.EndDate ?? DateTime.Today.AddYears(1);
            return $@"
if (Cookies && Cookies.set) {{
    var options = {{}};
    options.expires = new Date({cookieExpireDate.Year},{cookieExpireDate.Month - 1},{cookieExpireDate.Day},{cookieExpireDate.Hour},{cookieExpireDate.Minute})
    options.path = '{Url.Action("InlineScript", "AbTest")}';
    Cookies.set('{AbTestChoiceResolver.CookieNamePrefix + test.Id}', '{choice}', options);
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
(function(ctx, window, $){{
    if(ctx && !({precondition})) return;
    window.location = '{redirect.RedirectUrl}';
}})(ctx, window, $);
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
(function(ctx, window, $){{
    if(ctx && !({precondition})) return;
    {container.Scripts.First(_ => _.VersionNumber == choice).ScriptText}
}})(ctx, window, $);
");
                }
            }
            return sb.ToString();
        }
    }
}
