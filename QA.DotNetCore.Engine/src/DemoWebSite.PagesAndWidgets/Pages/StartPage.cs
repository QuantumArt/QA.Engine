using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DemoWebSite.PagesAndWidgets.Pages
{
    public class StartPage : AbstractPage, IStartPage
    {
        public string Bindings { get { return GetDetail("Bindings", String.Empty); } }

        public string[] GetDNSBindings()
        {
            return Bindings
                .Split(new char[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(_ => _.Trim())
                .ToArray();
        }

        public ITargetingUrlResolver GetUrlResolver()
        {
            var factory = Storage.ServiceProvider.GetService<UrlTokenResolverFactory>();
            var config = Storage.ServiceProvider.GetService<UrlTokenConfig>();
            return factory.Create(config);
            //return null;
        }
    }
}
