using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Routing;
using QA.DemoSite.Models.Pages;

namespace QA.DemoSite.Controllers
{
    public class ContentPageController : ContentControllerBase<ContentPage>
    {
        public IActionResult Index()
        {
            return View(CurrentItem);
        }
    }
}
