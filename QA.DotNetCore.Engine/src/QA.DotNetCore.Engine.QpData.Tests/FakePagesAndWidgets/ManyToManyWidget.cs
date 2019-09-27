using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets
{
    public class ManyToManyWidget : AbstractWidget
    {
        [LoadManyToManyRelations]
        public IEnumerable<int> BaseContentRelationIds => GetRelationIds("BaseContentRelations");

        [LoadManyToManyRelations]
        public IEnumerable<int> RelationIds => GetRelationIds("SomeRelations");
    }
}
