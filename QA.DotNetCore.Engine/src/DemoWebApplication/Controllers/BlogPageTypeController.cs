using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApplication.Controllers
{
    public class BlogPageController : ContentControllerBase<BlogPage>
    {
        public IActionResult Index()
        {
            return Content($"blogpage CurrentItem.Id={CurrentItem.Id}, Alias={CurrentItem.Alias}, tail=nope");
        }

        public IActionResult Details(string id, int? page)
        {
            return Content($"blogpage details {id}, page {page} {CurrentItem.Id}, {CurrentItem.Alias}");
        }
    }
}
