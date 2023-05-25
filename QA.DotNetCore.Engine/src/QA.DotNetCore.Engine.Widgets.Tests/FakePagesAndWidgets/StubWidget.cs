using System;
using System.Collections.Generic;
using System.Text;
using QA.DotNetCore.Engine.QpData;

namespace QA.DotNetCore.Engine.Widgets.Tests.FakePagesAndWidgets
{
    public sealed class StubWidget : AbstractWidget
    {
        public StubWidget(string zoneName, string[] allowedUrlPatterns, string[] deniedUrlPatterns)
        {
            ZoneName = zoneName;
            AllowedUrlPatterns = allowedUrlPatterns;
            DeniedUrlPatterns = deniedUrlPatterns;
        }

        public override string ZoneName { get; set; }

        public override string[] AllowedUrlPatterns { get; }

        public override string[] DeniedUrlPatterns { get; }
    }
}
