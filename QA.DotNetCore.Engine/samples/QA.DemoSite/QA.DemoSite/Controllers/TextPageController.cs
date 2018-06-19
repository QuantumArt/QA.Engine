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
    public class TextPageController : ContentControllerBase<TextPage>
    {
        AbstractItemStorage _provider;
        private static ILogger<TextPageController> _logger;

        public TextPageController(IAbstractItemStorageProvider abstractItemProvider, ILogger<TextPageController> logger)
        {
            _provider = abstractItemProvider.Get();
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(CurrentItem);
        }
    }
}
