using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class TargetingFilterAccessor : ITargetingFilterAccessor
    {
        readonly ITargetingFiltersConfigurator _cfg;

        public TargetingFilterAccessor(ITargetingFiltersConfigurator cfg)
        {
            _cfg = cfg;
        }

        public ITargetingFilter Get()
        {
            return _cfg.ResultFilter;
        }
    }
}
