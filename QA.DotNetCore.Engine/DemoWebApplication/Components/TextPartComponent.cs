using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DemoWebSite.PagesAndWidgets.Widgets;
using QA.DotNetCore.Engine.Widgets;

namespace WebApplication1
{
    public class TextPartViewComponent : WidgetComponentBase<TextPart>
    {        
        public override async Task<IViewComponentResult> RenderAsync(TextPart widget, IDictionary<string, object> argumets)
        {         
            // call some service:  var data = await _service.GetSomedata(widget.SomeProperty)

            await Task.Yield(); // do stub work to satisfy async-await syntax

            // and do some logic: var model = new TextPartViewModel(data, widget)

            return View(widget);
        }
    }
}
