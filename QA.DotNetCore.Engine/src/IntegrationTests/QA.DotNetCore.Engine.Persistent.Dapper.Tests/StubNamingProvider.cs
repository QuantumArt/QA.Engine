using QA.DotNetCore.Caching.Interfaces;
using System.Collections.Generic;
using QA.DotNetCore.Engine.Persistent.Interfaces;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests;

internal class StubNamingProvider : IQpContentCacheTagNamingProvider
{
    public string Get(string contentName, int siteId, bool isStage) => string.Empty;

    public Dictionary<int, string> GetByContentIds(int[] contentIds, bool isStage)
    {
        return new Dictionary<int, string>();
    }

    public Dictionary<string, string> GetByContentNetNames(string[] contentNetNames, int siteId, bool isStage)
    {
        return new Dictionary<string, string>();
    }

    public void SetUnitOfWork(IUnitOfWork unitOfWork)
    {
    }

    public string GetByNetName(string contentNetName, int siteId, bool isStage) => string.Empty;
}
