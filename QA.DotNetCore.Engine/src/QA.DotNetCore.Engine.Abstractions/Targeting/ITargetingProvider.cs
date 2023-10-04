using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingProvider : ITargetingProviderSource
    {
        IDictionary<string, object> GetValues();
    }
}
