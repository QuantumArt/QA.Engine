using Microsoft.AspNetCore.Mvc;
using QA.DemoSite.Models.Pages;
using QA.DotNetCore.Engine.Routing;
using System;
using System.Linq;

namespace QA.DemoSite.Controllers
{
    public class StartPageController : ContentControllerBase<StartPage>
    {
        public IActionResult Index()
        {
            var firstChildPage = CurrentItem.GetChildren().OrderBy(i => i.SortOrder).FirstOrDefault();
            if (firstChildPage != null)
                return new RedirectResult(firstChildPage.GetUrl(), false);
            else
                throw new Exception("Site is empty");
        }
    }
}
