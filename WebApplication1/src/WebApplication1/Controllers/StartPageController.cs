using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.PageModel;

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
