using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.AbTesting.Data;

namespace QA.DotNetCore.Engine.AbTesting
{
    public interface IAbTestService
    {
        AbTestPersistentData GetTestById(int testId);
        bool HasContainersForPage(string domain, string pagePath);
        AbTestWithContainers[] GetTestsWithContainersForPage(string domain, string pagePath);
    }
}
