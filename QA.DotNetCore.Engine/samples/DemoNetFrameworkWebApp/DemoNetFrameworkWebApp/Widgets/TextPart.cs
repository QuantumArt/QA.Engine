using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Replacements;
using System;

namespace DemoNetFrameworkWebApp.Widgets
{
    public class TextPart : AbstractWidget
    {
        public string Text { get { return GetDetail("Text", String.Empty); } }
    }

}
