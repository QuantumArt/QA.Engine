using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    public class SimpleAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly IAbstractItemStorageBuilder _builder;
        private readonly ICacheProvider _cacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;

        public SimpleAbstractItemStorageProvider(
            ICacheProvider cacheProvider,
            IAbstractItemStorageBuilder builder,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSiteStructureBuildSettings buildSettings,
            QpSiteStructureCacheSettings cacheSettings)
        {
            _builder = builder;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _buildSettings = buildSettings;
        }

        public AbstractItemStorage Get()
        {
            if (_cacheSettings.SiteStructureCachePeriod <= TimeSpan.Zero)
                return BuildStorage();

            var cacheKey = "QpAbstractItemStorageProvider.Get";
            var cacheTags = _builder.UsedContentNetNames.Select(c => _qpContentCacheTagNamingProvider.GetByNetName(c, _buildSettings.SiteId, _buildSettings.IsStage))
                .Where(t => t != null)
                .ToArray();

            return _cacheProvider.GetOrAdd(cacheKey, cacheTags, _cacheSettings.SiteStructureCachePeriod, BuildStorage, _buildSettings.CacheFetchTimeoutAbstractItemStorage);
        }

        private AbstractItemStorage BuildStorage()
        {
            return _builder.Build();
        }
    }
}
