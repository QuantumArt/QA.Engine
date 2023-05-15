using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    public class UniversalAbstractItemFactory : IAbstractItemFactory
    {
        private readonly IItemDefinitionRepository _repository;
        private readonly ICacheProvider _cacheProvider;
        private readonly IMemoryCacheProvider _memoryCacheProvider;        
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly QpSiteStructureBuildSettings _buildSettings;

        public UniversalAbstractItemFactory(
            ICacheProvider cacheProvider,
            IMemoryCacheProvider memoryCacheProvider,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            IItemDefinitionRepository repository,
            QpSiteStructureCacheSettings cacheSettings,
            QpSiteStructureBuildSettings buildSettings)
        {
            _repository = repository;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _buildSettings = buildSettings;
            _cacheProvider = cacheProvider;
            _memoryCacheProvider = memoryCacheProvider;
        }

        public AbstractItem Create(string discriminator)
        {
            var itemDefinition = GetItemDefinitionByDiscriminator(discriminator);
            if (itemDefinition == null)
            {
                return null;//элементов без ItemDefinition для структуры сайта не существует
            }

            var definition = new ItemDefinitionDetails
            {
                FrontModuleName = itemDefinition.FrontModuleName,
                FrontModuleUrl = itemDefinition.FrontModuleUrl
            };

            AbstractItem newItem;
            if (itemDefinition.IsPage)
            {
                newItem = new UniversalPage(discriminator, definition);
            }
            else
            {
                newItem = new UniversalWidget(discriminator, definition);
            }

            return newItem;
        }

        private ItemDefinitionPersistentData GetItemDefinitionByDiscriminator(string discriminator)
        {
            var itemDefinitionAcceptedStaleTime = TimeSpan.FromSeconds(5);

            return _memoryCacheProvider.GetOrAdd(
                $"{nameof(UniversalAbstractItemFactory)}.{nameof(GetItemDefinitionByDiscriminator)}({discriminator})",
                itemDefinitionAcceptedStaleTime,
                () =>
                {
                    var all = GetCachedItemDefinitions();
                    _ = all.TryGetValue(discriminator, out ItemDefinitionPersistentData result);
                    return result;
                });
        }

        private Dictionary<string, ItemDefinitionPersistentData> GetCachedItemDefinitions()
        {
            var cacheTags = new string[1] { _qpContentCacheTagNamingProvider.GetByNetName(KnownNetNames.ItemDefinition, _buildSettings.SiteId, _buildSettings.IsStage) }
                .Where(t => t != null)
                .ToArray();

            var result = _cacheProvider.GetOrAdd("UniversalAbstractItemFactory.GetCachedItemDefinitions",
                cacheTags,
                _cacheSettings.ItemDefinitionCachePeriod,
                () => _repository.GetAllItemDefinitions(_buildSettings.SiteId, _buildSettings.IsStage).ToDictionary(def => def.Discriminator));

            return result;
        }
    }
}
