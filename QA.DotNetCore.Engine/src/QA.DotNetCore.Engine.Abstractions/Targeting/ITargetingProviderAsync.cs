using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingProviderAsync
    {
        Task<IDictionary<string, object>> GetValues();
    }
}
