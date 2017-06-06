using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Replacements;
using System;

namespace DemoWebSite.PagesAndWidgets.Pages
{
    public class TextPage : AbstractPage
    {
        public string Text { get { return GetDetail("Text", String.Empty); } }

        [LibraryUrl]
        public string Picture { get { return GetDetail("Picture", String.Empty); } }
    }
}
