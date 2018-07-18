using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class TargetingFilterAccessor : ITargetingFilterAccessor
    {
        readonly ServiceSetConfigurator<ITargetingFilter> _cfg;

        public TargetingFilterAccessor(ServiceSetConfigurator<ITargetingFilter> cfg)
        {
            _cfg = cfg;
        }

        public ITargetingFilter Get()
        {
            return new UnitedFilter(_cfg.GetServices());
        }
    }
}
