using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Routing;
using QA.DemoSite.Models.Pages;

namespace QA.DemoSite.Controllers
{
    public class StartPageController : ContentControllerBase<StartPage>
    {
        private static ILogger<StartPageController> _logger;

        public StartPageController(ILogger<StartPageController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //if (CurrentItem.GetChildren().Where(w => w.IsPage).Any())
            //{
            //    try
            //    {
            //        var item = CurrentItem.GetChildren().Where(w => w.IsPage).OrderBy(o => o.SortOrder)?.First();
            //        if (item != null)
            //        {
            //            return Redirect(item.GetUrl());
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Error getting url for first page");
            //        return View(CurrentItem);
            //    }
            //}

            ViewBag.Title = CurrentItem.Title;
            return View(CurrentItem);
        }
    }
}
