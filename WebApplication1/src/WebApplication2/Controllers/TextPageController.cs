using Microsoft.AspNetCore.Mvc;
using Common.PageModel;

namespace WebApplication1.Controllers
{
    public class TextPageController:Controller
    {
        public ActionResult Index()
        {
            var model = GetCurrentItem() as TextPage;
            return View("Index", model);
        }


        public ActionResult Details(string id)
        {
            var model = GetCurrentItem() as TextPage;
            ViewBag.Id = id;
            return View("Details", model);
        }

        protected virtual AbstractItem GetCurrentItem()
        {
            return RouteData.DataTokens["ui-item"] as AbstractItem;
        }
    }
}
