using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Linq;
using NLog;
using QA.DotNetCore.Caching;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    public class SimpleCacheAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
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
                _logger.Info("Building storage");
                return _builder.Build();
            }

            string cacheKey = $"{nameof(SimpleCacheAbstractItemStorageProvider)}.{nameof(Get)}";
            var cacheTags = _qpContentCacheTagNamingProvider
                .GetByContentNetNames(
                    new[] { KnownNetNames.AbstractItem, KnownNetNames.ItemDefinition },
                    _buildSettings.SiteId,
                    _buildSettings.IsStage)
                .Select(n => n.Value)
                .Where(t => t != null)
                .ToArray();

            return _memoryCacheProvider.GetOrAdd(
                cacheKey,
                cacheTags,
                _cacheSettings.SiteStructureCachePeriod,
                () =>
                {
                    _logger.ForInfoEvent().Message("Building storage")
                        .Property("cacheKey", cacheKey)
                        .Property("cacheTags", cacheTags)
                        .Property("expiry", _cacheSettings.SiteStructureCachePeriod)
                        .Log();
                    return _builder.Build();
                },
                _buildSettings.CacheFetchTimeoutAbstractItemStorage);
        }

    }
}
