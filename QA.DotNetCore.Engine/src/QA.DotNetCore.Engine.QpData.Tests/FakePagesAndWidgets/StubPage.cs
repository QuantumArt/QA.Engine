using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets
{
    public class StubPage : AbstractPage
    {
        public string StubField => GetDetail<string>("StubField", null);
    }
}
