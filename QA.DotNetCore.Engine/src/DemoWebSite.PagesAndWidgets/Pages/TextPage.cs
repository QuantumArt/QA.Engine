using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Replacements;
using System;

namespace DemoWebSite.PagesAndWidgets.Pages
{
    public class TextPage : AbstractPage
    {
        public string Text { get { return GetDetail("Text", String.Empty); } }

        [LibraryUrl("Picture")]
        public virtual string PictureUrl { get { return GetDetail("Picture", String.Empty); } }

        [LibraryUrl]
        public virtual string Icon { get { return GetDetail("Icon", String.Empty); } }
    }

}
