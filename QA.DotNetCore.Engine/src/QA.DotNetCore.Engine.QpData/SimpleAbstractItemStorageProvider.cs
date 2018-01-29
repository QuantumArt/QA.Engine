using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
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
        private readonly QpSiteStructureSettings _settings;
        private readonly QpSettings _qpSettings;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;

        public SimpleAbstractItemStorageProvider(
            ICacheProvider cacheProvider,
            IAbstractItemStorageBuilder builder,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSettings qpSettings,
            QpSiteStructureSettings settings)
        {
            _builder = builder;
            _cacheProvider = cacheProvider;
            _settings = settings;
            _qpSettings = qpSettings;
        }

        public AbstractItemStorage Get()
        {
            if (!_settings.UseCache)
                return BuildStorage();

            var cacheKey = "QpAbstractItemStorageProvider.Get";
            var cacheTags = _builder.UsedContentNetNames.Select(c => _qpContentCacheTagNamingProvider.GetByNetName(c, _qpSettings.SiteId, _qpSettings.IsStage));
            return _cacheProvider.GetOrAdd(cacheKey, _settings.CachePeriod, BuildStorage);
        }

        private AbstractItemStorage BuildStorage()
        {
            return _builder.Build();
        }
    }
}
