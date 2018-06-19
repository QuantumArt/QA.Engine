using QA.DotNetCore.Engine.QpData;

namespace QA.DemoSite.Models.Pages
{
    public class TextPage : AbstractPage
    {
        public string Text { get { return this.GetDetail("Text", string.Empty); } }
    }
}
