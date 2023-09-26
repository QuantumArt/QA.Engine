using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class OnlyPagesFilter : BaseTargetingFilter
    {
        public override bool Match(IAbstractItem item)
        {
            return item.IsPage;
        }
    }
}
