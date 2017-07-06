using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;

namespace DemoWebApplication.Controllers
{
    public class TextPageController : ContentControllerBase<TextPage>
    {
        public ActionResult Index()
        {
            return View("Index", CurrentItem);
        }

        public ActionResult Details(string id)
        {
            ViewBag.Id = id;
            return View("Details", CurrentItem);
        }
    }
}
