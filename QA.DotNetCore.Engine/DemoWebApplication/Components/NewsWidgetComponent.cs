using DemoWebSite.PagesAndWidgets.Widgets;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Widgets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoWebApplication.Components
{
    public class NewsWidgetViewComponent : WidgetComponentBase<NewsWidget>
    {
        public override async Task<IViewComponentResult> RenderAsync(NewsWidget widget, IDictionary<string, object> argumets)
        {
            return View(widget);
        }
    }
}