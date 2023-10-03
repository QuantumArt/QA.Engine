using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class RegionFilter : ManyToManyFilter<AbstractItem>
    {
        public RegionFilter(ITargetingContext context) : base(context) { }
        protected override string TargetingKey => "region";
        protected override bool MatchIfNoRelation => true;
        protected override bool MatchIfNoTargetingType => false;
        protected override IEnumerable<int> GetRelationIds(AbstractItem item) => item.RegionIds;
    }
}
