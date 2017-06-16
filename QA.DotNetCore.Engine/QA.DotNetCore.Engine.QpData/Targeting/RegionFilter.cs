using QA.DotNetCore.Engine.Abstractions;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData.Targeting
{
    public class RegionFilter : BaseTargetingFilter
    {
        public const string Key = "region";

        int[] _currentRegionKeys;

        public RegionFilter(int[] currentRegionKeys)
        {
            _currentRegionKeys = currentRegionKeys;
        }

        public override bool Match(IAbstractItem item)
        {
            var val = item.GetTargetingValue(Key);
            if (val == null || !(val is int[]))
                return true;

            var regions = val as int[];
            //если у страницы нет регионов, значит подходит всем
            //если есть, значит должно быть пересечение хотя бы по одному
            return !regions.Any() ? true : regions.Intersect(_currentRegionKeys).Any();
        }
    }
}
