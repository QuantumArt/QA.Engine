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
using System.Threading;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию. Может кешировать.
    /// </summary>
    // TODO: Change name: not as granular as it claims to be.
    public class GranularCacheAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private static readonly SemaphoreSlim _cacheStorageSemaphore = new SemaphoreSlim(1, 1);

        private readonly IAbstractItemContextStorageBuilder _builder;
        private readonly ICacheProvider _cacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IAbstractItemRepository _abstractItemRepository;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly INodeIdentifier _nodeIdentifier;
        private readonly IMemoryCacheProvider _memoryCacheProvider;

        public GranularCacheAbstractItemStorageProvider(
            ICacheProvider cacheProvider,
            IAbstractItemContextStorageBuilder builder,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSiteStructureBuildSettings buildSettings,
            QpSiteStructureCacheSettings cacheSettings,
            IAbstractItemRepository abstractItemRepository,
            INodeIdentifier nodeIdentifier,
            IMemoryCacheProvider memoryCacheProvider)
        {
            _builder = builder;
            _abstractItemRepository = abstractItemRepository;
            _buildSettings = buildSettings;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheProvider = cacheProvider;
            _cacheSettings = cacheSettings;
            _nodeIdentifier = nodeIdentifier;
            _memoryCacheProvider = memoryCacheProvider;
        }

        public AbstractItemStorage Get()
        {
            var extensionsWithAbsItems = GetExtensionContentsWithAbstractItemPersistentData();

            int siteId = _buildSettings.SiteId;
            bool isStage = _buildSettings.IsStage;

            var allTags = new WidgetsAndPagesCacheTags
            {
                AbstractItemTag = _qpContentCacheTagNamingProvider.GetByNetName(
                    KnownNetNames.AbstractItem, siteId, isStage),
                ItemDefinitionTag = _qpContentCacheTagNamingProvider.GetByNetName(
                    KnownNetNames.ItemDefinition, siteId, isStage),
                ExtensionsTags = _qpContentCacheTagNamingProvider.GetByContentIds(
                    extensionsWithAbsItems.Keys.ToArray(), siteId, isStage)
            }.AllTags;

            TimeSpan expiry = _cacheSettings.SiteStructureCachePeriod;
            const string localCacheKey = nameof(AbstractItemStorage) + "." + nameof(Get);
            string globalCacheKey = $"{_nodeIdentifier.GetUniqueId()}:{localCacheKey}";

            _cacheStorageSemaphore.Wait();

            // TODO: Extract to separate abstraction (combined cache).
            try
            {
                bool isStorageGloballyFresh = _cacheProvider.IsSet(globalCacheKey);

                if (isStorageGloballyFresh)
                {
                    return _memoryCacheProvider.GetOrAdd(localCacheKey, expiry, RebuildStorage);
                }
                else
                {
                    var storage = RebuildStorage();
                    _memoryCacheProvider.Set(localCacheKey, storage, expiry);
                    return storage;
                }
            }
            finally
            {
                _ = _cacheStorageSemaphore.Release();
            }

            AbstractItemStorage RebuildStorage()
            {
                var storage = BuildStorage(extensionsWithAbsItems);
                _cacheProvider.Add(true, globalCacheKey, allTags, expiry);
                return storage;
            }
        }

        /// <summary>
        /// Формирование storage
        /// </summary>
        private AbstractItemStorage BuildStorage(
            IDictionary<int, AbstractItemPersistentData[]> extensionsWithAbsItems)
        {
            _builder.Init(extensionsWithAbsItems);
            var abstractItems = GetAbstractItems(extensionsWithAbsItems);
            _builder.SetRelationsBetweenAbstractItems(abstractItems);
            return _builder.BuildStorage(abstractItems);
        }

        /// <summary>
        /// Получение всех AbstractItem
        /// </summary>
        private AbstractItem[] GetAbstractItems(
            IDictionary<int, AbstractItemPersistentData[]> extensionsWithAbsItems)
        {
            var result = new List<AbstractItem>();
            foreach (var extension in extensionsWithAbsItems)
            {
                var extensionContentId = extension.Key;
                var plainAbstractItems = extension.Value;

                var abstractItems = BuildAbstractItems(extensionContentId, plainAbstractItems);

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
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray());
        }
    }
}
