using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Logging;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    public class SimpleCacheAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly ILogger _logger;
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
            QpSiteStructureCacheSettings cacheSettings,
            ILogger<SimpleCacheAbstractItemStorageProvider> logger)
        {
            _builder = builder;
            _memoryCacheProvider = memoryCacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _buildSettings = buildSettings;
            _logger = logger;
        }

        public AbstractItemStorage Get()
        {
            if (_cacheSettings.SiteStructureCachePeriod <= TimeSpan.Zero)
            {
                _logger.LogInformation("Building storage");
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
                    using var _ = _logger.BeginScopeWith(("cacheKey", cacheKey),
                        ("cacheTags", cacheTags),
                        ("expiry", _cacheSettings.SiteStructureCachePeriod));
                    _logger.LogInformation("Building storage");

                    return _builder.Build();
                },
                _buildSettings.CacheFetchTimeoutAbstractItemStorage);
        }

    }
}
