using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApplication.Components
{
    public class TargetingDisplayViewComponent : ViewComponent
    {
        public TargetingDisplayViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Yield();

            var ctx = ((ITargetingContext)ViewContext.HttpContext.RequestServices.GetService(typeof(ITargetingContext)));

            var model = new TargetingDisplayViewModel
            {
                Items = ctx.GetTargetingKeys().ToDictionary(k => k, k => ctx.GetTargetingValue(k).ToString())
            };

            return View(model);
        }
    }

    public class TargetingDisplayViewModel
    {
        public Dictionary<string, string> Items { get; set; }
    }
}
