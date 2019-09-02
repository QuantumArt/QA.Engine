using Microsoft.AspNetCore.Mvc;
using QA.DemoSite.ViewModels;
using QA.DotNetCore.Engine.Routing;

namespace QA.DemoSite.Components
{
    public class BreadcrumbsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string additional)
        {
            var currentPage = ViewContext.GetCurrentItem();

            var model = new BreadcrumbsViewModel(currentPage, additional);

            return View(model);
        }
    }
}
