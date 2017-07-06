using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.Routing;

namespace DemoWebApplication.Controllers
{
    public class StartPageController : ContentControllerBase<StartPage>
    {
        public IActionResult Index()
        {
            var item = CurrentItem;
            ViewBag.Title = item.Title;
            return View(item);
        }
    }
}
