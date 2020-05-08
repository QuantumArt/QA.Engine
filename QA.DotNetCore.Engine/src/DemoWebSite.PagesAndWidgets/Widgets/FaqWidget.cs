using QA.DotNetCore.Engine.QpData;
using System.Collections.Generic;

namespace DemoWebSite.PagesAndWidgets.Widgets
{

    public class FaqWidget : AbstractWidget
    {
        public string Header => GetDetail("Header", string.Empty);
        public IEnumerable<int> Questions => GetRelationIds("Questions");
    }
}
