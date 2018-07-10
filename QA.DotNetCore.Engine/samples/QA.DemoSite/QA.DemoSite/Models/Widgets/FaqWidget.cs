using QA.DotNetCore.Engine.QpData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DemoSite.Models.Widgets
{
    
    public class FaqWidget : AbstractWidget
    {
        public string Header => GetDetail("Header", string.Empty);

        [LoadManyToManyRelations]
        public IEnumerable<int> Questions => GetRelationIds("Questions");
    }
}
