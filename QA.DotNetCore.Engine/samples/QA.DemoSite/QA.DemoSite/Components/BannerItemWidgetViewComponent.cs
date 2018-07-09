using Microsoft.AspNetCore.Mvc;
using QA.DemoSite.Models.Widgets;
using QA.DotNetCore.Engine.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DemoSite.Components
{
    public class BannerItemWidgetViewComponent : WidgetComponentBase<BannerItemWidget>
    {
        public override Task<IViewComponentResult> RenderAsync(BannerItemWidget widget, IDictionary<string, object> argumets)
        {
            return Task.FromResult<IViewComponentResult>(View(widget));
        }
    }
}
