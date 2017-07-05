using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData;
using System;
using System.Linq;

namespace DemoNetFrameworkWebApp.Pages
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
    }
}
