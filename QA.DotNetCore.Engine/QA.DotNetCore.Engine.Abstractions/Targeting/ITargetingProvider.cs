using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingProvider
    {
        IDictionary<string, object> GetValues();
    }
}
