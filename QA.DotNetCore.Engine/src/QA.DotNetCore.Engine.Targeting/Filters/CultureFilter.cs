using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class CultureFilter : OneToManyFilter<AbstractItem>
    {
        public CultureFilter(ITargetingContext context) : base(context) { }
        protected override string TargetingKey => "culture";
        protected override bool MatchIfNoRelation => true;
        protected override bool MatchIfNoTargetingType => false;
        protected override int? GetRelationId(AbstractItem item) => item.CultureId;
    }
}
