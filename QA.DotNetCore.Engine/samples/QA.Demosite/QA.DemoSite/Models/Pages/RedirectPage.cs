using QA.DotNetCore.Engine.QpData;

namespace QA.DemoSite.Models.Pages
{
    public class RedirectPage : AbstractPage
    {
        public string RedirectTo => GetDetail("RedirectTo", string.Empty);
    }
}
