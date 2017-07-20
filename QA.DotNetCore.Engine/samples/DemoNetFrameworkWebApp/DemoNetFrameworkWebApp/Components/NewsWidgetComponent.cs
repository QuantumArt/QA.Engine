using DemoNetFrameworkWebApp.Widgets;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Widgets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoNetFrameworkWebApp.Components
{
    public class NewsWidgetViewComponent : WidgetComponentBase<NewsWidget>
    {
        public override async Task<IViewComponentResult> RenderAsync(NewsWidget widget, IDictionary<string, object> argumets)
        {
            await Task.Yield();
            return View(widget);
        }
    }
}
