using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Targeting.Filters;
using System.Linq;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoRegionFilter : BaseTargetingFilter
    {
        ITargetingContext _ctx;

        public DemoRegionFilter(ITargetingContext ctx)
        {
            _ctx = ctx;
        }

        public override bool Match(IAbstractItem item)
        {
            var ctxVal = _ctx.GetTargetingValue(TargetingKeys.Region);
            if (ctxVal == null || !(ctxVal is int[]))
                return true;

            var val = item.GetTargetingValue(TargetingKeys.Region);
            if (val == null || !(val is int[]))
                return true;

            var regions = val as int[];
            var currentRegionKeys = ctxVal as int[];
            //если у страницы нет регионов, значит подходит всем
            //если есть, значит должно быть пересечение хотя бы по одному
            return !regions.Any() ? true : regions.Intersect(currentRegionKeys).Any();
        }
    }
}
