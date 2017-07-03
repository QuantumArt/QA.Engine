using QA.DotNetCore.Engine.Abstractions;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    public class SimpleAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        IAbstractItemStorageBuilder _builder;
        ICacheProvider _cacheProvider;
        QpSiteStructureSettings _settings;

        public SimpleAbstractItemStorageProvider(
            ICacheProvider cacheProvider,
            IAbstractItemStorageBuilder builder,
            QpSiteStructureSettings settings)
        {
            _builder = builder;
            _cacheProvider = cacheProvider;
            _settings = settings;
        }

        public AbstractItemStorage Get()
        {
            if (!_settings.UseCache)
                return BuildStorage();

            var cacheKey = "QpAbstractItemStorageProvider.Get";
            return _cacheProvider.GetOrAdd(cacheKey, _settings.CachePeriod, BuildStorage);
        }

        private AbstractItemStorage BuildStorage()
        {
            return _builder.Build();
        }
    }
}
