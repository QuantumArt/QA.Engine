using QA.DotNetCore.Caching.Interfaces;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests;

internal class StubNamingProvider : IQpContentCacheTagNamingProvider
{
    public string Get(string contentName, int siteId, bool isStage) =>
        string.Empty;

    public Dictionary<int, string> GetByContentIds(int[] contentIds, int siteId, bool isStage) =>
        new Dictionary<int, string>();

    public string GetByNetName(string contentNetName, int siteId, bool isStage) =>
        string.Empty;
}
