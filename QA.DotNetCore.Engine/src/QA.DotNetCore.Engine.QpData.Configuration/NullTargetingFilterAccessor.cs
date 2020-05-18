using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    public class NullTargetingFilterAccessor : ITargetingFilterAccessor
    {
        public ITargetingFilter Get()
        {
            return new NullFilter();
        }
    }
}
