using QA.DotNetCore.Engine.QpData;

namespace QA.DemoSite.Models.Pages
{
    public class TextPage : AbstractPage
    {
        public string Text { get { return GetDetail("Text", string.Empty); } }

        public bool HideTitle { get { return GetDetail("HideTitle", false); } }
    }
}
