using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public sealed class NullTargetingFilterAccessor : ITargetingFilterAccessor
    {
        public ITargetingFilter Get()
        {
            return new NullFilter();
        }

        public ITargetingFilter Get(string key)
        {
            return new NullFilter();
        }
    }
}
