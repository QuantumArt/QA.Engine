using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    public class NullTargetingContext : ITargetingContext
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
