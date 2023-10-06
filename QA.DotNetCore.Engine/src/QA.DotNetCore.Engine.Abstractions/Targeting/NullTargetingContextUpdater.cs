using System.Collections.Generic;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public class NullTargetingContextUpdater : ITargetingContextUpdater
    {
        public Task Update(IDictionary<string, string> values) => Task.CompletedTask;
    }
}
