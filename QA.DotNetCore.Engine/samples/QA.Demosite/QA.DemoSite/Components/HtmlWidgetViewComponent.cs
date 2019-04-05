using Microsoft.AspNetCore.Mvc;
using QA.DemoSite.Models.Widgets;
using QA.DotNetCore.Engine.Widgets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QA.DemoSite.Components
{
    public class HtmlWidgetViewComponent : WidgetComponentBase<HtmlWidget>
    {
        public override Task<IViewComponentResult> RenderAsync(HtmlWidget widget, IDictionary<string, object> argumets)
        {
            return Task.FromResult<IViewComponentResult>(View(widget));
        }
    }
}
