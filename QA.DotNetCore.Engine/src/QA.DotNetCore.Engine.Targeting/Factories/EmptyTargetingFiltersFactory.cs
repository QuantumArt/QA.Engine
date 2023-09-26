using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Targeting.Filters;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Targeting.Factories
{
    public class EmptyTargetingFiltersFactory : ITargetingFiltersFactory
    {
        private static readonly ITargetingFilter EmptyFilter = new EmptyFilter();

        public ITargetingFilter StructureFilter(IDictionary<string, string> targeting)
            => EmptyFilter;

        public ITargetingFilter FlattenNodesFilter(IDictionary<string, string> targeting)
            => EmptyFilter;
    }
}
