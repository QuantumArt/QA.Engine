using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    public class UniversalAbstractItemFactory : IAbstractItemFactory
    {
        readonly IItemDefinitionRepository _repository;
        readonly ICacheProvider _cacheProvider;
        readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        readonly QpSiteStructureCacheSettings _cacheSettings;
        readonly QpSiteStructureBuildSettings _buildSettings;

        public UniversalAbstractItemFactory(ICacheProvider cacheProvider,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            IItemDefinitionRepository repository,
            QpSiteStructureCacheSettings cacheSettings,
            QpSiteStructureBuildSettings buildSettings)
        {
            _repository = repository;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _buildSettings = buildSettings;
        }


        public AbstractItem Create(string discriminator)
        {
            var itemDefinition = GetItemDefinitionByDiscriminator(discriminator);
            if (itemDefinition == null)
                return null;//элементов без ItemDefinition для структуры сайта не существует

            AbstractItem newItem;
            if (itemDefinition.IsPage)
                newItem = new UniversalPage(discriminator);
            else
                newItem = new UniversalWidget(discriminator);

            return newItem;
        }

        private ItemDefinitionPersistentData GetItemDefinitionByDiscriminator(string discriminator)
        {
            var all = GetCachedItemDefinitions();
            if (!all.ContainsKey(discriminator))
                return null;
            return all[discriminator];
        }

        private Dictionary<string, ItemDefinitionPersistentData> GetCachedItemDefinitions()
        {
            var cacheTags = new string[1] { _qpContentCacheTagNamingProvider.GetByNetName(KnownNetNames.ItemDefinition, _buildSettings.SiteId, _buildSettings.IsStage) }
                .Where(t => t != null)
                .ToArray();
            return _cacheProvider.GetOrAdd("UniversalAbstractItemFactory.GetCachedItemDefinitions",
                cacheTags,
                _cacheSettings.ItemDefinitionCachePeriod,
                () => _repository.GetAllItemDefinitions(_buildSettings.SiteId, _buildSettings.IsStage).ToDictionary(def => def.Discriminator));
        }
    }
}
