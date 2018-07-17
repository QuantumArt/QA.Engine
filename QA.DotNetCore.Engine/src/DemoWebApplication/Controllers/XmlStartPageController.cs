using DemoWebSite.PagesAndWidgets.Xml;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;

namespace DemoWebApplication.Controllers
{
    public class XmlStartPageController : ContentControllerBase<XmlStartPage>
    {
        public IActionResult Index()
        {
            var item = CurrentItem;
            ViewBag.Title = item.Title;
            return View(item);
        }
    }
}
