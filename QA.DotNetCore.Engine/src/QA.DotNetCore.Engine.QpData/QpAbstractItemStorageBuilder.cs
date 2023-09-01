using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Models;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Строитель структуры сайта из базы QP
    /// </summary>
    public class QpAbstractItemStorageBuilder : IAbstractItemStorageBuilder, IAbstractItemContextStorageBuilder
    {
        private AbstractItemStorageBuilderContext _context;

        private readonly IAbstractItemFactory _itemFactory;
        private readonly IAbstractItemRepository _abstractItemRepository;
        private readonly IMetaInfoRepository _metaInfoRepository;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly ILogger<QpAbstractItemStorageBuilder> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;

        public QpAbstractItemStorageBuilder(
            IAbstractItemFactory itemFactory,
            IAbstractItemRepository abstractItemRepository,
            IMetaInfoRepository metaInfoRepository,
            QpSiteStructureBuildSettings buildSettings,
            ILogger<QpAbstractItemStorageBuilder> logger,
            IServiceScopeFactory scopeFactory,
            IServiceProvider serviceProvider,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            ICacheProvider cacheProvider,
            QpSiteStructureCacheSettings cacheSettings)
        {
            _itemFactory = itemFactory;
            _abstractItemRepository = abstractItemRepository;
            _metaInfoRepository = metaInfoRepository;
            _buildSettings = buildSettings;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _serviceProvider = serviceProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheProvider = cacheProvider;
            _cacheSettings = cacheSettings;
        }

        public AbstractItemStorage BuildStorage(AbstractItem[] abstractItems)
        {
            _logger.LogDebug(
                "AbstractItemStorage build via AbstractItems collection started. Build id: {LogId}, SiteId: {SiteId}, IsStage: {IsStage}",
                _context.LogId, _buildSettings.SiteId, _buildSettings.IsStage);
            var root = abstractItems.First(x => x.Discriminator == _buildSettings.RootPageDiscriminator);
            return new AbstractItemStorage(root, abstractItems);
        }

        /// <summary>
        /// Формирование AbstractItem
        /// </summary>
        /// <param name="extensionContentId">Идентификатор контента расширения</param>
        /// <param name="abstractItemPersistentData">Идентификаторы связанный AbstractItem</param>
        /// <param name="lazyLoad"></param>
        /// <returns></returns>
        public AbstractItem[] BuildAbstractItems(int extensionContentId,
            AbstractItemPersistentData[] abstractItemPersistentData, bool lazyLoad)
        {
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(_context));
            }

            _logger.LogTrace(
                "AbstractItem build via QP started. Build id: {LogId}, SiteId: {SiteId}, IsStage: {IsStage}",
                _context.LogId, _buildSettings.SiteId, _buildSettings.IsStage);

            var activatedAbstractItems = new Dictionary<int, AbstractItem>();
            //первый проход списка - активируем, т.е. создаём AbsractItem-ы с правильным типом и набором заполненных полей, запоминаем root
            foreach (var persistentItem in abstractItemPersistentData)
            {
                var activatedItem = _itemFactory.Create(persistentItem.Discriminator);
                if (activatedItem == null)
                {
                    continue;
                }

                activatedItem.MapPersistent(persistentItem);
                activatedAbstractItems.Add(persistentItem.Id, activatedItem);
            }

            _logger.LogTrace("Activated abstract items: {AbstractItemsCount}. Build id: {LogId}",
                activatedAbstractItems.Count, _context.LogId);

            if (extensionContentId > 0 || _buildSettings.LoadAbstractItemFieldsToDetailsCollection)
            {
                foreach (var abstractItem in activatedAbstractItems.Values)
                {
                    abstractItem.SetBuilder(this);
                    if (!lazyLoad)
                    {
                        abstractItem.VerifyDetailsLoaded();
                    }
                }
            }
            else
            {
                _logger.LogTrace(
                    "Skip load data for extension-less elements (LoadAbstractItemFieldsToDetailsCollection = false). Build id: {LogId}",
                    _context.LogId);
            }

            //догрузим связи m2m в основном контенте, если это нужно

            if (_context.NeedLoadM2MInAbstractItem)
            {
                _logger.LogTrace(
                    "Load data for many-to-many fields in main content (QPAbstractItem). Build id: {LogId}",
                    _context.LogId);

                if (_context.AbstractItemsM2MData != null)
                {
                    var m2MFields = _context.M2MFields[extensionContentId];
                    foreach (var key in _context.AbstractItemsM2MData.Keys)
                    {
                        if (activatedAbstractItems.TryGetValue(key, out var item))
                        {
                            item.M2MRelations.Merge(_context.AbstractItemsM2MData[key]);
                            item.M2MFieldNameMapToLinkIds = m2MFields;
                        }
                    }
                }
            }

            return activatedAbstractItems.Values.ToArray();
        }

        /// <summary>
        /// Заполняем поля иерархии Parent-Children, на основании ParentId. Заполняем VersionOf
        /// </summary>
        /// <param name="abstractItems"></param>
        public void SetRelationsBetweenAbstractItems(AbstractItem[] abstractItems)
        {
            CleanRelationsBetweenAbstractItems(abstractItems);

            var activatedAbstractItems = abstractItems.ToDictionary(x => x.Id, x => x);
            foreach (var item in activatedAbstractItems.Values)
            {
                if (item.VersionOfId.HasValue &&
                    activatedAbstractItems.TryGetValue(item.VersionOfId.Value, out var main))
                {
                    item.MapVersionOf(main);

                    if (main.ParentId.HasValue &&
                        activatedAbstractItems.TryGetValue(main.ParentId.Value, out var abstractItem))
                    {
                        abstractItem.AddChild(item);
                    }
                }
                else if (item.ParentId.HasValue &&
                         activatedAbstractItems.TryGetValue(item.ParentId.Value, out var abstractItem))
                {
                    abstractItem.AddChild(item);
                }
            }
        }

        public AbstractItemExtensionCollection BuildDetails(AbstractItem item, bool createScope) =>
            BuildDetails(item,
                _context.GetExtensionData(item.ExtensionId ?? 0),
                _context.ExtensionContents,
                _context.BaseContent,
                _context.ExtensionsM2MData,
                _context.M2MFields,
                _context.LogId,
                createScope
            );

        /// <summary>
        /// Очищаем поля иерархии Parent-Children, VersionOf
        /// </summary>
        /// <param name="abstractItems"></param>
        private static void CleanRelationsBetweenAbstractItems(AbstractItem[] abstractItems)
        {
            foreach (var abstractItem in abstractItems)
            {
                _ = abstractItem
                    .CleanVersionOf()
                    .CleanChildren();
            }
        }

        public AbstractItemStorage Build()
        {
            var abstractItemsPlain =
                _abstractItemRepository.GetPlainAllAbstractItems(_buildSettings.SiteId,
                    _buildSettings.IsStage);

            //сгруппируем AbsractItem-ы по extensionId
            var extensionsWithAbsItems = abstractItemsPlain
                .GroupBy(x => x.ExtensionId.GetValueOrDefault(0))
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray());

            Init(extensionsWithAbsItems, false);

            var abstractItems = extensionsWithAbsItems
                .SelectMany(x => BuildAbstractItems(x.Key, x.Value, false))
                .ToArray();
            SetRelationsBetweenAbstractItems(abstractItems);
            return BuildStorage(abstractItems);
        }

        /// <summary>
        /// Формирование контекста
        /// </summary>
        /// <param name="extensions">Content items from AbstractItems content grouped by extension type.</param>
        /// <param name="lazyLoad"></param>
        public void Init(IDictionary<int, AbstractItemPersistentData[]> extensions, bool lazyLoad)
        {
            _context = new AbstractItemStorageBuilderContext
            {
                BaseContent = _metaInfoRepository.GetContent(KnownNetNames.AbstractItem, _buildSettings.SiteId),
            };

            _context.NeedLoadM2MInAbstractItem =
                _buildSettings.LoadAbstractItemFieldsToDetailsCollection
                && _context.BaseContent.ContentAttributes.Any(ca => ca.IsManyToManyField);

            if (extensions != null)
            {
                //получим инфу обо всех контентах-расширениях, которые используются
                _context.ExtensionContents = _metaInfoRepository
                    .GetContentsById(
                        extensions.Select(group => group.Key).Where(key => key > 0).ToArray())
                    .ToDictionary(c => c.ContentId);

                var allTags = GetTags(extensions.Keys);
                if (lazyLoad)
                {
                    _context.LazyExtensionData = extensions.ToDictionary(x => x.Key,
                        x => new Lazy<IDictionary<int, AbstractItemExtensionCollection>>(
                            () => GetAbstractItemExtensionData(
                                x.Key,
                                x.Value.Select(i => i.Id),
                                new[] {x.Key == 0 ? allTags.AbstractItemTag : allTags.ExtensionsTags[x.Key]},
                                _context.BaseContent,
                                _context.LogId,
                                true
                            )));
                }
                else
                {
                    _context.ExtensionData = extensions.ToDictionary(x => x.Key,
                        x => GetAbstractItemExtensionData(
                            x.Key,
                            x.Value.Select(i => i.Id),
                            new[] {x.Key == 0 ? allTags.AbstractItemTag : allTags.ExtensionsTags[x.Key]},
                            _context.BaseContent,
                            _context.LogId,
                            false
                        )
                    );
                }


                var abstractItemTags = ConcatTags(allTags.AbstractItemTag, allTags.ItemDefinitionTag);
                _context.AbstractItemsM2MData =
                    GetAbstractItemsManyToManyRelations(extensions, abstractItemTags, _context.LogId);
                _context.ExtensionsM2MData =
                    GetExtensionsManyToManyRelations(extensions, allTags.AllTags, _context.LogId);
                var m2MBase = _context.BaseContent.ContentAttributes.Where(ca => ca.IsManyToManyField).ToList();
                _context.M2MFields = _context.ExtensionContents.Select(n => new
                {
                    n.Key, Value = n.Value.ContentAttributes
                        .Where(ca => ca.IsManyToManyField)
                        .Union(m2MBase)
                        .ToDictionary(
                            k => k.ColumnName.ToLowerInvariant(),
                            v => v.M2MLinkId ?? 0)
                }).ToDictionary(k => k.Key, v => v.Value);
                _context.M2MFields.Add(0, m2MBase.ToDictionary(
                    k => k.ColumnName.ToLowerInvariant(),
                    v => v.M2MLinkId ?? 0)
                );
            }
        }

        private IDictionary<int, M2MRelations> GetAbstractItemsManyToManyRelations(
            IDictionary<int, AbstractItemPersistentData[]> extensions,
            string[] abstractItemTags,
            string logId) =>
            _cacheProvider.GetOrAdd(
                $"{nameof(Init)}.{nameof(GetAbstractItemsManyToManyRelations)}",
                abstractItemTags,
                _cacheSettings.SiteStructureCachePeriod,
                () =>
                {
                    var extensionKeys = String.Join(", ", extensions.Keys);
                    _logger.LogInformation(
                        "Load M2M data for abstract items. Build id: {LogId}",
                        logId);
                    return _abstractItemRepository.GetManyToManyData(
                        extensions.Values
                            .SelectMany(x => x)
                            .Select(item => item.Id),
                        _buildSettings.IsStage);
                },
                _buildSettings.CacheFetchTimeoutAbstractItemStorage);

        private IDictionary<int, M2MRelations> GetExtensionsManyToManyRelations(
            IDictionary<int, AbstractItemPersistentData[]> extensions,
            string[] tags,
            string logId)
        {
            return _cacheProvider.GetOrAdd(
                $"{nameof(Init)}.{nameof(GetExtensionsManyToManyRelations)}",
                tags,
                _cacheSettings.SiteStructureCachePeriod,
                () =>
                {
                    var extensionKeys = extensions.Keys.Where(k => k != 0).ToArray();
                    _logger.LogInformation(
                        "Load M2M data for extension tables: {ExtensionId}. Build id: {LogId}",
                        String.Join(", ", extensionKeys),
                        logId);
                    return _abstractItemRepository.GetManyToManyDataByContents(extensionKeys, _buildSettings.IsStage);
                },
                _buildSettings.CacheFetchTimeoutAbstractItemStorage);
        }

        private string[] GetExtensionTags(WidgetsAndPagesCacheTags tags, int extensionId)
        {
            if (!tags.ExtensionsTags.TryGetValue(extensionId, out string extensionTag))
            {
                _logger.LogError("Cache tag for extension {ExtensionId} not found.", extensionId);
                return Array.Empty<string>();
            }

            return new[] {extensionTag};
        }

        private WidgetsAndPagesCacheTags GetTags(IEnumerable<int> extensions)
        {
            string abstractItemTag = _qpContentCacheTagNamingProvider
                .GetByNetName(KnownNetNames.AbstractItem, _buildSettings.SiteId, _buildSettings.IsStage);

            string itemDefinitionTag = _qpContentCacheTagNamingProvider
                .GetByNetName(KnownNetNames.ItemDefinition, _buildSettings.SiteId, _buildSettings.IsStage);

            Dictionary<int, string> extensionsTags = _qpContentCacheTagNamingProvider
                .GetByContentIds(extensions.ToArray(), _buildSettings.IsStage);

            return new WidgetsAndPagesCacheTags
            {
                AbstractItemTag = abstractItemTag,
                ItemDefinitionTag = itemDefinitionTag,
                ExtensionsTags = extensionsTags
            };
        }

        private static string[] ConcatTags(params string[] tags) =>
            tags is null ? Array.Empty<string>() : tags.Where(tag => !string.IsNullOrEmpty(tag)).ToArray();

        private IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionId,
            IEnumerable<int> abstractItemIds, string[] tags, ContentPersistentData baseContent, string logId,
            bool createScope)
        {
            using var scope = createScope ? _scopeFactory.CreateScope() : null;
            var scopeString = scope != null ? "new" : "existing";
            var provider = scope != null ? scope.ServiceProvider : _serviceProvider;

            var abstractItemRepository = provider.GetRequiredService<IAbstractItemRepository>();
            var buildSettings = provider.GetRequiredService<QpSiteStructureBuildSettings>();
            var logger = provider.GetRequiredService<ILogger<QpAbstractItemStorageBuilder>>();

            IDictionary<int, AbstractItemExtensionCollection> result;

            if (extensionId == 0)
            {
                result = _cacheProvider.GetOrAdd(
                    $"GetAbstractItemExtensionlessData",
                    tags,
                    _cacheSettings.SiteStructureCachePeriod,
                    () =>
                    {
                        logger.LogInformation(
                            "Load data from base abstract item table in {scopeString} scope. Build id: {LogId}",
                            scopeString, logId);
                        return abstractItemRepository.GetAbstractItemExtensionlessData(
                            abstractItemIds,
                            baseContent,
                            buildSettings.IsStage
                        );
                    },
                    _buildSettings.CacheFetchTimeoutAbstractItemStorage);
            }
            else
            {
                result = _cacheProvider.GetOrAdd(
                    $"GetAbstractItemExtensionData({extensionId})",
                    tags,
                    _cacheSettings.SiteStructureCachePeriod,
                    () =>
                    {
                        logger.LogInformation(
                            "Load data from extension table {ExtensionId} in {scopeString} scope. Build id: {LogId}",
                            extensionId, scopeString, logId);
                        return abstractItemRepository.GetAbstractItemExtensionData(
                            extensionId,
                            baseContent,
                            buildSettings.LoadAbstractItemFieldsToDetailsCollection,
                            buildSettings.IsStage
                        );
                    },
                    _buildSettings.CacheFetchTimeoutAbstractItemStorage);
            }

            return result;
        }

        /// <summary>
        /// Возвращает данные контента расширения для AbstractItem <paramref name="item"/>
        /// </summary>
        /// <param name="item">AbstractItem</param>
        /// <param name="extensionData"></param>
        /// <param name="extensionContents"></param>
        /// <param name="baseContent"></param>
        /// <param name="extensionsM2MData">Данные о связях m2m у расширения</param>
        /// <param name="logId"></param>
        /// <param name="createScope"></param>
        /// <returns></returns>
        private AbstractItemExtensionCollection BuildDetails(AbstractItem item,
            IDictionary<int, AbstractItemExtensionCollection> extensionData,
            IDictionary<int, ContentPersistentData> extensionContents,
            ContentPersistentData baseContent,
            IDictionary<int, M2MRelations> extensionsM2MData,
            IDictionary<int, Dictionary<string, int>> m2mFields,
            string logId,
            bool createScope
        )
        {
            using var scope = createScope ? _scopeFactory.CreateScope() : null;
            var provider = scope != null ? scope.ServiceProvider : _serviceProvider;

            var qpUrlResolver = provider.GetRequiredService<IQpUrlResolver>();
            var buildSettings = provider.GetRequiredService<QpSiteStructureBuildSettings>();
            var logger = provider.GetRequiredService<ILogger<QpAbstractItemStorageBuilder>>();

            return BuildDetails(qpUrlResolver, buildSettings, logger,
                item, extensionData, extensionContents, baseContent, extensionsM2MData, m2mFields, logId);
        }

        /// <summary>
        /// Возвращает данные контента расширения для AbstractItem <paramref name="item"/>
        /// </summary>
        /// <param name="buildSettings"></param>
        /// <param name="logger"></param>
        /// <param name="item">AbstractItem</param>
        /// <param name="extensionData">Словарь, где ключ - это CID расширения, в значение - это данные расширения</param>
        /// <param name="extensionContents"></param>
        /// <param name="baseContent"></param>
        /// <param name="extensionsM2MData">Данные о связях m2m у расширения</param>
        /// <param name="logId"></param>
        /// <param name="qpUrlResolver"></param>
        /// <returns></returns>
        private static AbstractItemExtensionCollection BuildDetails(
            IQpUrlResolver qpUrlResolver,
            QpSiteStructureBuildSettings buildSettings,
            ILogger logger,
            AbstractItem item,
            IDictionary<int, AbstractItemExtensionCollection> extensionData,
            IDictionary<int, ContentPersistentData> extensionContents,
            ContentPersistentData baseContent,
            IDictionary<int, M2MRelations> extensionsM2MData,
            IDictionary<int, Dictionary<string, int>> m2mFields,
            string logId)
        {
            var extensionContentId = item.ExtensionId.GetValueOrDefault(0);

            if (!extensionData.TryGetValue(item.Id, out var details))
            {
                logger.LogTrace(
                    "Not found data for extension {ExtensionId}. Build id: {LogId}",
                    extensionContentId,
                    logId);
                return null;
            }

            var extensionContent = extensionContents.TryGetValue(extensionContentId, out var content) ? content : null;

            var fields = new List<ContentAttributePersistentData>();
            int? extensionContentItemId = null;

            if (extensionContent?.ContentAttributes != null)
            {
                fields.AddRange(extensionContent.ContentAttributes);
            }

            if (buildSettings.LoadAbstractItemFieldsToDetailsCollection)
            {
                fields.AddRange(baseContent.ContentAttributes);
            }

            var fileFields = new Dictionary<string, ContentAttributePersistentData>();
            fields.Where(ca => ca.IsFileField).ToList().ForEach(
                ca => fileFields.TryAdd(ca.ColumnName.ToLowerInvariant(), ca)
            );

            //проведём замены в некоторых значениях доп полей
            foreach (var key in details.Keys)
            {
                if (key.Equals("CONTENT_ITEM_ID", StringComparison.OrdinalIgnoreCase))
                {
                    extensionContentItemId = Convert.ToInt32(details[key]);
                }

                if (details[key] is string stringValue)
                {
                    //1) надо заменить плейсхолдер <%=upload_url%> на реальный урл
                    details.Set(key,
                        stringValue.Replace(buildSettings.UploadUrlPlaceholder,
                            qpUrlResolver.UploadUrl(buildSettings.SiteId)));

                    //2) проверим, является ли это поле ссылкой на файл, тогда нужно преобразовать его в полный урл
                    if (fileFields.TryGetValue(key.ToLowerInvariant(), out var fileField))
                    {
                        var baseUrl = qpUrlResolver.UrlForImage(buildSettings.SiteId, fileField);
                        if (!string.IsNullOrEmpty(baseUrl) && !stringValue.StartsWith(baseUrl))
                        {
                            details.Set(key, baseUrl + "/" + stringValue);
                        }
                    }
                }
            }

            //установим связи m2m в контентах расширений, в которых они есть
            if (extensionContentItemId.HasValue)
            {
                if (extensionsM2MData != null && extensionsM2MData.TryGetValue(extensionContentItemId.Value, out var relations))
                {
                    item.M2MRelations.Merge(relations);
                }
            }

            if (m2mFields != null && m2mFields.TryGetValue(extensionContentId, out var mapping))
            {
                item.M2MFieldNameMapToLinkIds = mapping;
            }

            return details;
        }
    }
}
