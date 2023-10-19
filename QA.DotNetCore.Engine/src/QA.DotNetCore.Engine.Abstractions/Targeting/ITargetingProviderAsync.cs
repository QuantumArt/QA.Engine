using System.Collections.Generic;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingProviderAsync : ITargetingProviderSource
    {
        Task<IDictionary<string, object>> GetValues();
    }
}
