using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets
{
    public class ManyToManyWidget : AbstractWidget
    {
        public IEnumerable<int> BaseContentRelationIds => GetRelationIds("BaseContentRelations");

        public IEnumerable<int> RelationIds => GetRelationIds("SomeRelations");
    }
}
