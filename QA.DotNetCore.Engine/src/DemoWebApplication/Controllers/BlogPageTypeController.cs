using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApplication.Controllers
{
    public class BlogPageController : ContentControllerBase<BlogPage>
    {
        public ITargetingContext TargetingContext { get; }

        public BlogPageController(ITargetingContext targetingContext)
        {
            TargetingContext = targetingContext;
        }
        public IActionResult Index()
        {
            var keys = TargetingContext.GetTargetingKeys();
            return Content($"blogpage index CurrentItem.Id {CurrentItem.Id}, Alias {CurrentItem.Alias}. Targeting {String.Join(",", keys.Select(k => $"{k}={TargetingContext.GetTargetingValue(k)}"))}.");
        }

        public IActionResult Details(string id, int? page)
        {
            var keys = TargetingContext.GetTargetingKeys();
            return Content($"blogpage details {id}, page {page}, CurrentItem.Id {CurrentItem.Id}, CurrentItem.Alias {CurrentItem.Alias}. Targeting {String.Join(",", keys.Select(k => $"{k}={TargetingContext.GetTargetingValue(k)}"))}.");
        }
    }
}
