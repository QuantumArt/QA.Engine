using DemoWebSite.PagesAndWidgets.Widgets;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApplication.Components
{
    public class BannerWidgetViewComponent : WidgetComponentBase<BannerWidget>
    {
        public override Task<IViewComponentResult> RenderAsync(BannerWidget widget, IDictionary<string, object> argumets)
        {
            return Task.FromResult<IViewComponentResult>(View(widget));
        }
    }
}
