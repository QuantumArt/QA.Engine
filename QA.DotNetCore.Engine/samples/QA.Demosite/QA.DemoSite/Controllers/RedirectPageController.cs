using Microsoft.AspNetCore.Mvc;
using QA.DemoSite.Models.Pages;
using QA.DotNetCore.Engine.Routing;

namespace QA.DemoSite.Controllers
{
    public class RedirectPageController : ContentControllerBase<RedirectPage>
    {
        public IActionResult Index()
        {
            return new RedirectResult(CurrentItem.RedirectTo, true);
        }
    }
}
