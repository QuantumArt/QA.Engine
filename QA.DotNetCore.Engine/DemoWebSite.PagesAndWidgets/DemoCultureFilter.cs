using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class DemoCultureFilter : BaseTargetingFilter
    {
        ITargetingContext _ctx;

        public DemoCultureFilter(ITargetingContext ctx)
        {
            _ctx = ctx;
        }

        public override bool Match(IAbstractItem item)
        {
            var ctxVal = _ctx.GetTargetingValue(TargetingKeys.Culture);
            if (ctxVal == null || !(ctxVal is string))
                return true;

            var val = item.GetTargetingValue(TargetingKeys.Culture);
            if (val == null || !(val is string))
                return true;

            var ctxStrVal = ctxVal as string;
            var stringVal = val as string;
            return String.IsNullOrWhiteSpace(stringVal) ? true : stringVal.Equals(ctxStrVal, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
