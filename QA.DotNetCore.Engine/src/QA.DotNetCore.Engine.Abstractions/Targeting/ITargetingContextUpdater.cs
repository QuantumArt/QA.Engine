using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingContextUpdater
    {
        Task Update(HttpContext context, IDictionary<string, object> values);
    }
}
