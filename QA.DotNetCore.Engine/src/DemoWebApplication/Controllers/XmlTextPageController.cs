using DemoWebSite.PagesAndWidgets.Xml;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;

namespace DemoWebApplication.Controllers
{
    public class XmlTextPageController : ContentControllerBase<XmlTextPage>
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
