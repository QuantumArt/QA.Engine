using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Linq;
using QA.DotNetCore.Caching;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    public class SimpleCacheAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly IAbstractItemStorageBuilder _builder;
        private readonly VersionedCacheCoreProvider _memoryCacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;

        public SimpleCacheAbstractItemStorageProvider(
            VersionedCacheCoreProvider memoryCacheProvider,
            IAbstractItemStorageBuilder builder,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSiteStructureBuildSettings buildSettings,
            QpSiteStructureCacheSettings cacheSettings)
        {
            _builder = builder;
            _memoryCacheProvider = memoryCacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _buildSettings = buildSettings;
        }

        public AbstractItemStorage Get()
        {
            if (_cacheSettings.SiteStructureCachePeriod <= TimeSpan.Zero)
            {
                return BuildStorage();
            }

            string cacheKey = $"{nameof(SimpleCacheAbstractItemStorageProvider)}.{nameof(Get)}";
            var cacheTags = new[] { KnownNetNames.AbstractItem, KnownNetNames.ItemDefinition }
                .Select(c => _qpContentCacheTagNamingProvider.GetByNetName(c, _buildSettings.SiteId, _buildSettings.IsStage))
                .Where(t => t != null)
                .ToArray();

            return _memoryCacheProvider.GetOrAdd(
                cacheKey,
                cacheTags,
                _cacheSettings.SiteStructureCachePeriod,
                BuildStorage,
                _buildSettings.CacheFetchTimeoutAbstractItemStorage);
        }

        private AbstractItemStorage BuildStorage()
        {
            return _builder.Build();
        }
    }
}
