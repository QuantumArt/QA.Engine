using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Models;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    public class GranularCacheAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly IAbstractItemStorageBuilder _builder;
        private readonly ICacheProvider _cacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IAbstractItemRepository _abstractItemRepository;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;

        public GranularCacheAbstractItemStorageProvider(
            ICacheProvider cacheProvider,
            IAbstractItemStorageBuilder builder,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSiteStructureBuildSettings buildSettings,
            QpSiteStructureCacheSettings cacheSettings,
            IAbstractItemRepository abstractItemRepository)
        {
            _builder = builder;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _abstractItemRepository = abstractItemRepository;
            _buildSettings = buildSettings;
        }

        public AbstractItemStorage Get()
        {
            if (_cacheSettings.SiteStructureCachePeriod <= TimeSpan.Zero)
            {
                return BuildStorageWithoutCache();
            }

            var extensionsWithAbsItems = GetExtensionContentsWithAbstractItemPersistents();

            // Соберем все теги для общего кэша
            var cacheTags = new WidgetsAndPagesCacheTags
            {
                AbstractItemTag = _qpContentCacheTagNamingProvider.GetByNetName(KnownNetNames.AbstractItem,
                    _buildSettings.SiteId, _buildSettings.IsStage),
                ItemDefinitionTag = _qpContentCacheTagNamingProvider.GetByNetName(KnownNetNames.ItemDefinition,
                    _buildSettings.SiteId, _buildSettings.IsStage),
                ExtensionsTags = _qpContentCacheTagNamingProvider.GetByContentIds(extensionsWithAbsItems.Keys.ToArray(),
                    _buildSettings.SiteId, _buildSettings.IsStage)
            };

            const string cacheKey = "QpAbstractItemStorageProvider.Get";
            return _cacheProvider.GetOrAdd(cacheKey,
                cacheTags.AllTags,
                _cacheSettings.SiteStructureCachePeriod,
                () => BuildStorageWithCache(extensionsWithAbsItems, cacheTags),
                _buildSettings.CacheFetchTimeoutAbstractItemStorage);
        }

        /// <summary>
        /// Формирование storage
        /// </summary>
        /// <param name="extensionsWithAbsItems"></param>
        /// <param name="cacheTags">Кэш теги</param>
        /// <remarks>с кэш</remarks>
        /// <returns></returns>
        private AbstractItemStorage BuildStorageWithCache(
            IDictionary<int, AbstractItemPersistentData[]> extensionsWithAbsItems,
            WidgetsAndPagesCacheTags cacheTags)
        {
            _builder.Init(extensionsWithAbsItems);
            return BuildStorage(GetCachedAbstractItems(extensionsWithAbsItems, cacheTags));
        }

        /// <summary>
        /// Формирование storage
        /// </summary>
        /// <remarks>Без кэш</remarks>
        /// <returns></returns>
        private AbstractItemStorage BuildStorageWithoutCache()
            => _builder.Build();

        /// <summary>
        /// Формирование storage
        /// </summary>
        /// <param name="abstractItems"></param>
        /// <returns></returns>
        private AbstractItemStorage BuildStorage(AbstractItem[] abstractItems)
        {
            _builder.SetRelationsBetweenAbstractItems(abstractItems);
            return _builder.BuildStorage(abstractItems);
        }

        /// <summary>
        /// Получение всех AbstractItem
        /// </summary>
        /// <param name="extensionsWithAbsItems"></param>
        /// <param name="cacheTags">Кэш теги</param>
        /// <remarks>С кэш</remarks>
        /// <returns></returns>
        private AbstractItem[] GetCachedAbstractItems(
            IDictionary<int, AbstractItemPersistentData[]> extensionsWithAbsItems,
            WidgetsAndPagesCacheTags cacheTags)
        {
            var result = new List<AbstractItem>();
            foreach (var extension in extensionsWithAbsItems)
            {
                var extensionContentId = extension.Key;
                var plainAbstractItems = extension.Value;
                var cacheKey = $"QpAbstractItemStorageProvider.Get#{extensionContentId.ToString()}";

                var tags = cacheTags.ExtensionsTags.TryGetValue(extensionContentId, out var extCacheTag)
                    ? new[] {cacheTags.ItemDefinitionTag, extCacheTag}
                    : new[] {cacheTags.AbstractItemTag, cacheTags.ItemDefinitionTag};

                var abstractItems = _cacheProvider.GetOrAdd(cacheKey,
                    tags.ToArray(),
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
        /// <param name="extensionContentId"></param>
        /// <param name="plainAbstractItems"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private AbstractItem[] BuildAbstractItems(int extensionContentId,
            AbstractItemPersistentData[] plainAbstractItems)
            => _builder.BuildAbstractItems(extensionContentId, plainAbstractItems);

        private IDictionary<int, AbstractItemPersistentData[]> GetExtensionContentsWithAbstractItemPersistents()
        {
            var abstractItemsPlain =
                _abstractItemRepository.GetPlainAllAbstractItems(_buildSettings.SiteId,
                    _buildSettings.IsStage);

            //сгруппируем AbsractItem-ы по extensionId
            return abstractItemsPlain
                .GroupBy(x => x.ExtensionId.GetValueOrDefault(0))
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray());
        }
    }
}
