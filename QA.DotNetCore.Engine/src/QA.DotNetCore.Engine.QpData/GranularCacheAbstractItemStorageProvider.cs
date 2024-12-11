using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces.Logging;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Models;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    public class GranularCacheAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly ILogger _logger;
        private readonly IAbstractItemContextStorageBuilder _builder;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IAbstractItemRepository _abstractItemRepository;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly VersionedCacheCoreProvider _memoryCacheProvider;
        private readonly IServiceProvider _serviceProvider;

        public GranularCacheAbstractItemStorageProvider(
            IAbstractItemContextStorageBuilder builder,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSiteStructureBuildSettings buildSettings,
            QpSiteStructureCacheSettings cacheSettings,
            IAbstractItemRepository abstractItemRepository,
            ICacheProvider cacheProvider,
            VersionedCacheCoreProvider memoryCacheProvider,
            IServiceProvider serviceProvider,
            ILogger<GranularCacheAbstractItemStorageProvider> logger)
        {
            _builder = builder;
            _abstractItemRepository = abstractItemRepository;
            _buildSettings = buildSettings;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _cacheProvider = cacheProvider;
            _memoryCacheProvider = memoryCacheProvider;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public AbstractItemStorage Get()
        {
            var extensionsWithAbsItems = GetExtensionContentsWithAbstractItemPersistentData();

            int siteId = _buildSettings.SiteId;
            bool isStage = _buildSettings.IsStage;

            var cacheTags = _qpContentCacheTagNamingProvider.GetByContentNetNames(
                new[] { KnownNetNames.AbstractItem, KnownNetNames.ItemDefinition }, siteId, isStage);

            var tags = new WidgetsAndPagesCacheTags
            {
                AbstractItemTag = cacheTags[KnownNetNames.AbstractItem],
                ItemDefinitionTag = cacheTags[KnownNetNames.ItemDefinition],
                ExtensionsTags = _qpContentCacheTagNamingProvider.GetByContentIds(
                    extensionsWithAbsItems.Keys.Where(n => n > 0).ToArray(), isStage)
            };

            TimeSpan expiry = _cacheSettings.SiteStructureCachePeriod;
            TimeSpan waitTimeout = _buildSettings.CacheFetchTimeoutAbstractItemStorage;
            const string cacheKey = nameof(GranularCacheAbstractItemStorageProvider) + "." + nameof(Get);

            var storage = _memoryCacheProvider.GetOrAdd(
                cacheKey,
                tags.AllTags,
                expiry,
                () =>
                {
                    using var _ = _logger.BeginScopeWith(("cacheKey", cacheKey),
                        ("cacheTags", tags.AllTags),
                        ("expiry", expiry),
                        ("waitTimeout", waitTimeout));
                    _logger.LogInformation("Building storage");

                    return BuildStorageWithCache(extensionsWithAbsItems, tags);
                },
                waitTimeout,
                true);
            storage.Builder.SetServiceProvider(_serviceProvider);
            return storage;
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

                var cacheKey =
                    $"{nameof(GranularCacheAbstractItemStorageProvider)}.{nameof(GetCachedAbstractItems)}({extensionContentId})";

                var tags = cacheTags.ExtensionsTags.TryGetValue(extensionContentId, out var extensionCacheTag)
                    ? new[] {cacheTags.ItemDefinitionTag, extensionCacheTag}
                    : new[] {cacheTags.ItemDefinitionTag, cacheTags.AbstractItemTag};

                AbstractItem[] abstractItems = _cacheProvider.GetOrAdd(
                    cacheKey,
                    tags,
                    _cacheSettings.SiteStructureCachePeriod,
                    () => BuildAbstractItems(extensionContentId, plainAbstractItems),
                    _buildSettings.CacheFetchTimeoutAbstractItemStorage);

                foreach (var ai in abstractItems)
                {
                    ai.SetBuilder(_builder);
                }

                result.AddRange(abstractItems);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Формирование AbstractItem
        /// </summary>
        private AbstractItem[] BuildAbstractItems(int extensionContentId,
            AbstractItemPersistentData[] plainAbstractItems)
            => _builder.BuildAbstractItems(extensionContentId, plainAbstractItems, true);

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
