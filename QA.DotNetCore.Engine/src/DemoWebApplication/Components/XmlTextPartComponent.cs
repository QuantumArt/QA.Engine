using DemoWebSite.PagesAndWidgets.Xml;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Widgets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoWebApplication.Components
{
    public class XmlTextPartViewComponent : WidgetComponentBase<XmlTextPart>
    {        
        public override async Task<IViewComponentResult> RenderAsync(XmlTextPart widget, IDictionary<string, object> argumets)
        {         
            // call some service:  var data = await _service.GetSomedata(widget.SomeProperty)

            await Task.Yield(); // do stub work to satisfy async-await syntax

            // and do some logic: var model = new TextPartViewModel(data, widget)

            return View(widget);
        }
    }
}
