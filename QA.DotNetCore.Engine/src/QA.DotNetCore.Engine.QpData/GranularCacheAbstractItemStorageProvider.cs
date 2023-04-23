using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Models;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    public class GranularCacheAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly IAbstractItemContextStorageBuilder _builder;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IAbstractItemRepository _abstractItemRepository;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly IDistributedMemoryCacheProvider _cacheProvider;

        public GranularCacheAbstractItemStorageProvider(
            IAbstractItemContextStorageBuilder builder,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSiteStructureBuildSettings buildSettings,
            QpSiteStructureCacheSettings cacheSettings,
            IAbstractItemRepository abstractItemRepository,
            IDistributedMemoryCacheProvider compositeCacheProvider)
        {
            _builder = builder;
            _abstractItemRepository = abstractItemRepository;
            _buildSettings = buildSettings;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _cacheProvider = compositeCacheProvider;
        }

        public AbstractItemStorage Get()
        {
            var extensionsWithAbsItems = GetExtensionContentsWithAbstractItemPersistentData();

            int siteId = _buildSettings.SiteId;
            bool isStage = _buildSettings.IsStage;

            var tags = new WidgetsAndPagesCacheTags
            {
                AbstractItemTag = _qpContentCacheTagNamingProvider.GetByNetName(
                    KnownNetNames.AbstractItem, siteId, isStage),
                ItemDefinitionTag = _qpContentCacheTagNamingProvider.GetByNetName(
                    KnownNetNames.ItemDefinition, siteId, isStage),
                ExtensionsTags = _qpContentCacheTagNamingProvider.GetByContentIds(
                    extensionsWithAbsItems.Keys.ToArray(), siteId, isStage)
            };

            TimeSpan expiry = _cacheSettings.SiteStructureCachePeriod;
            const string cacheKey = nameof(GranularCacheAbstractItemStorageProvider) + "." + nameof(Get);

            return _cacheProvider.GetOrAdd(
                cacheKey,
                tags.AllTags,
                expiry,
                () => BuildStorageWithCache(extensionsWithAbsItems, tags),
                _buildSettings.CacheFetchTimeoutAbstractItemStorage);
        }

        /// <summary>
        /// Формирование storage
        /// </summary>
        private AbstractItemStorage BuildStorageWithCache(
            IDictionary<int, AbstractItemPersistentData[]> extensionsWithAbsItems,
            WidgetsAndPagesCacheTags cacheTags)
        {
            _builder.Init(extensionsWithAbsItems, true);
            var abstractItems = GetCachedAbstractItems(extensionsWithAbsItems, cacheTags);
            _builder.SetRelationsBetweenAbstractItems(abstractItems);
            return _builder.BuildStorage(abstractItems);
        }

        /// <summary>
        /// Получение всех AbstractItem
        /// </summary>
        private AbstractItem[] GetCachedAbstractItems(
            IDictionary<int, AbstractItemPersistentData[]> extensionsWithAbsItems,
            WidgetsAndPagesCacheTags cacheTags)
        {
            var result = new List<AbstractItem>();
            foreach (var extension in extensionsWithAbsItems)
            {
                var extensionContentId = extension.Key;
                var plainAbstractItems = extension.Value;

                var cacheKey = $"{nameof(GranularCacheAbstractItemStorageProvider)}.{nameof(GetCachedAbstractItems)}({extensionContentId})";

                var tags = cacheTags.ExtensionsTags.TryGetValue(extensionContentId, out var extCacheTag)
                    ? new[] { cacheTags.ItemDefinitionTag, cacheTags.AbstractItemTag, extCacheTag }
                    : new[] { cacheTags.ItemDefinitionTag, cacheTags.AbstractItemTag };

                AbstractItem[] abstractItems = _cacheProvider.GetOrAdd(
                    cacheKey,
                    tags,
                    _cacheSettings.SiteStructureCachePeriod,
                    () => BuildAbstractItems(extensionContentId, plainAbstractItems),
                    _buildSettings.CacheFetchTimeoutAbstractItemStorage);

                result.AddRange(abstractItems);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Формирование AbstractItem
        /// </summary>
        private AbstractItem[] BuildAbstractItems(int extensionContentId, AbstractItemPersistentData[] plainAbstractItems)
            => _builder.BuildAbstractItems(extensionContentId, plainAbstractItems);

        private IDictionary<int, AbstractItemPersistentData[]> GetExtensionContentsWithAbstractItemPersistentData()
        {
            var abstractItemsPlain = _abstractItemRepository.GetPlainAllAbstractItems(
                _buildSettings.SiteId,
                _buildSettings.IsStage);

            //сгруппируем AbsractItem-ы по extensionId
            return abstractItemsPlain
                .GroupBy(x => x.ExtensionId.GetValueOrDefault(0))
                .ToDictionary(x => x.Key, x => x.ToArray());
        }
    }
}
