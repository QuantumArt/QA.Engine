using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Data;

namespace QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IAbTestRepository
    {
        IEnumerable<AbTestPersistentData> GetActiveTests(int siteId, bool isStage, IDbTransaction transaction = null);
        IEnumerable<AbTestPersistentData> GetAllTests(int siteId, bool isStage, IDbTransaction transaction = null);
        IEnumerable<AbTestContainerBasePersistentData> GetActiveTestsContainers(int siteId, bool isStage, IDbTransaction transaction = null);
        IEnumerable<AbTestContainerBasePersistentData> GetAllTestsContainers(int siteId, bool isStage, IDbTransaction transaction = null);
        string AbTestNetName { get; }
        string AbTestContainerNetName { get; }
        string AbTestScriptNetName { get; }
        string AbTestRedirectNetName { get; }
    }
}
