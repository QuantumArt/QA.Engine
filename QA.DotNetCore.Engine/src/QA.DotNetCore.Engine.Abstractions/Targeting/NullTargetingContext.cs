using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public sealed class NullTargetingContext : ITargetingContext
    {
        public string[] GetTargetingKeys()
        {
            return new string[0];
        }

        public object GetTargetingValue(string key)
        {
            return null;
        }
    }
}
