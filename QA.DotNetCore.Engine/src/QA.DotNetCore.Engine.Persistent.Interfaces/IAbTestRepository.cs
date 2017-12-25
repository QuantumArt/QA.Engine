using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IAbTestRepository
    {
        IEnumerable<AbTestPersistentData> GetActiveTests(int siteId, bool isStage);
        IEnumerable<AbTestContainerBasePersistentData> GetActiveTestsContainers(int siteId, bool isStage);
    }
}
