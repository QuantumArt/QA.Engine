using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;

namespace DemoWebApplication.Controllers
{
    public class TestExtensionlessPageController : ContentControllerBase<TestExtensionlessPage>
    {
        // GET
        public IActionResult Index()
        {
            var tags = CurrentItem.Tags;
            return Content($"extensionless page. tags = {tags}");
        }
    }
}
