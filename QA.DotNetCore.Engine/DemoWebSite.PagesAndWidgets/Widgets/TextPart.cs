using QA.DotNetCore.Engine.QpData;
using System;

namespace DemoWebSite.PagesAndWidgets.Widgets
{
    public class TextPart : AbstractWidget
    {
        public string Text { get { return GetDetail("Text", String.Empty); } }
    }
}
