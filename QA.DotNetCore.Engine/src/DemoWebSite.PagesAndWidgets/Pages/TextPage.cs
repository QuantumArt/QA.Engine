using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Replacements;
using System;

namespace DemoWebSite.PagesAndWidgets.Pages
{
    public class TextPage : AbstractPage
    {
        public string Text { get { return GetDetail("Text", String.Empty); } }

        [LibraryUrl]
        public virtual string Picture { get { return GetDetail("Picture", String.Empty); } }
    }


    public class BlogPage : TextPage
    {
        public override string Picture => base.Picture;
    }
}
