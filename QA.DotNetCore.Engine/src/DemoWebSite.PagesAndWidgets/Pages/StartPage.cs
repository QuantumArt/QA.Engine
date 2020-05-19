using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;

namespace DemoWebSite.PagesAndWidgets.Pages
{
    public class StartPage : AbstractPage, IStartPage
    {
        public string Bindings => GetDetail("Bindings", String.Empty);

        public string[] GetDNSBindings()
        {
            return Bindings
                .Split(new char[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(_ => _.Trim())
                .ToArray();
        }

        //public ITargetingUrlResolver GetUrlResolver()
        //{
        //    return _urlTokenResolverFactory.Create(_urlTokenConfig);
        //}
    }
}
