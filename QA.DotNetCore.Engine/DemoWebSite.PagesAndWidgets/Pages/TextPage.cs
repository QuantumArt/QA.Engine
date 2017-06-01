using QA.DotNetCore.Engine.QpData;
using System;

namespace DemoWebSite.PagesAndWidgets.Pages
{
    public class TextPage : AbstractPage
    {
        public string Text { get { return GetDetail("Text", String.Empty); } }
    }
}
