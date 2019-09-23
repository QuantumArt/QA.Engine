using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QA.DotNetCore.Engine.QpData;

namespace DemoWebSite.PagesAndWidgets.Widgets
{
    public class HtmlWidget : AbstractWidget
    {
        public string HTML => GetDetail("HTML", string.Empty);
    }
}
