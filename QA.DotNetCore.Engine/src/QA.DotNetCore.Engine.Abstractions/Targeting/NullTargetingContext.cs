using System;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public sealed class NullTargetingContext : ITargetingContext
    {
        public string[] GetTargetingKeys()
        {
            return Array.Empty<string>();
        }

        public object GetTargetingValue(string key)
        {
            return null;
        }
    }
}
