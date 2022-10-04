using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.WidgetPlatform.Api.TargetingFilters;

namespace QA.WidgetPlatform.Api
{
    public class EmptyTargetingFiltersFactory : ITargetingFiltersFactory
    {
        private static readonly ITargetingFilter _emptyFilter = new EmptyFilter();

        public ITargetingFilter StructureFilter(IDictionary<string, string> targeting)
            => _emptyFilter;

        public ITargetingFilter FlattenNodesFilter(IDictionary<string, string> targeting)
            => _emptyFilter;
    }
}
