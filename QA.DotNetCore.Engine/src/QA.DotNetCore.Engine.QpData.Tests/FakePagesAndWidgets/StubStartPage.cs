using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets
{
    public class StubStartPage : AbstractPage, IStartPage 
    {
        public static readonly string DnsRegistered = "quantumart.ru";

        public string[] GetDNSBindings()
        {
            return new[] { DnsRegistered };
        }
    }
}
