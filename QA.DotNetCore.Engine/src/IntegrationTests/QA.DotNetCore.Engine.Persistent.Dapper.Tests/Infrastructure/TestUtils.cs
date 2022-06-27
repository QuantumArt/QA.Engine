using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.QpData.Settings;
using System;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure
{
    internal static class TestUtils
    {
        private static readonly TimeSpan s_generalExpiry = TimeSpan.Parse("23:59:59");

        public static QpSiteStructureCacheSettings CreateDefaultCacheSettings()
        {
            return new QpSiteStructureCacheSettings
            {
                QpSchemeCachePeriod = s_generalExpiry,
                ItemDefinitionCachePeriod = s_generalExpiry,
                SiteStructureCachePeriod = s_generalExpiry
            };
        }

        public static VersionedCacheCoreProvider CreateDefaultMemoryCacheProvider()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            return new VersionedCacheCoreProvider(memoryCache);
        }
    }
}
