using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets
{
    public class StartPage : AbstractPage, IStartPage
    {
        public string DnsBinding { get { return GetDetail("DnsBinding", ""); } }

        public string[] GetDNSBindings()
        {
            return DnsBinding.Split('|');
        }
    }
}
