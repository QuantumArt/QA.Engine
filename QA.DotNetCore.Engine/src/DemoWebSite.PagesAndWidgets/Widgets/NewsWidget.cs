using QA.DotNetCore.Engine.QpData;
using System.Collections.Generic;

namespace DemoWebSite.PagesAndWidgets.Widgets
{
    public class NewsWidget : AbstractWidget
    {
        public int Count { get { return GetDetail("Count", 0); } }
        public int NewsPageId { get { return GetDetail("NewsPage", 0); } }

        [LoadManyToManyRelations]
        public IEnumerable<int> CategoryIds { get { return GetRelationIds("Categories"); } }
    }
}
