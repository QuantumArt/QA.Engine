using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.QpData;

namespace WebApplication1.Controllers
{
    public class StartPageController : Controller
    {
        public IActionResult Index()
        {
            var item = GetCurrentItem();
            ViewBag.Title = item.Title;
            return View(item);
        }

        protected virtual AbstractItem GetCurrentItem()
        {
            return RouteData.DataTokens["ui-item"] as AbstractItem;
        }
    }
}
