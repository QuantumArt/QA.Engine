using QA.DotNetCore.Engine.QpData;

namespace DemoWebSite.PagesAndWidgets.Pages
{

    public class TestExtensionlessPage : AbstractPage
    {
        public string Tags => GetDetail<string>("Tags", null);
    }
}
