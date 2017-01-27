using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.PageModel;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
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
