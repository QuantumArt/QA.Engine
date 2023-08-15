using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using Tests.CommonUtils.Helpers;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure
{
    internal static class TestUtils
    {
        private static readonly TimeSpan _generalExpiry = TimeSpan.Parse("23:59:59");

        public static QpSiteStructureCacheSettings CreateDefaultCacheSettings()
        {
            return new QpSiteStructureCacheSettings
            {
                QpSchemeCachePeriod = _generalExpiry,
                ItemDefinitionCachePeriod = _generalExpiry,
                SiteStructureCachePeriod = _generalExpiry
            };
        }
    }
}
