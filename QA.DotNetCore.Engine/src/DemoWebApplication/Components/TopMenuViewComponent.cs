using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.Targeting.Filters;

namespace DemoWebApplication.Components
{
    public class TopMenuViewComponent : ViewComponent
    {
        public TopMenuViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Yield();

            var startPage = ViewContext.GetStartPage();

            if (startPage == null) return null;

            var filter = ((ITargetingFilterAccessor)ViewContext.HttpContext.RequestServices.GetService(typeof(ITargetingFilterAccessor)))?.Get();

            var model = new TopMenuViewModel
            {
                Items = startPage.GetChildren(new UnitedFilter(NavigationFilter.Default, filter)).ToArray()
            };

            return View(model);
        }
    }

    public class TopMenuViewModel
    {        
        public IEnumerable<IAbstractItem> Items { get; set; } = new List<IAbstractItem>();
    }
}
