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
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheProvider = cacheProvider;
            _cacheSettings = cacheSettings;
        }


        public AbstractItemStorage BuildStorage(AbstractItem[] abstractItems)
        {
            _logger.LogInformation(
                "AbstractItemStorage build via AbstractItems collection started. Build id: {0}, SiteId: {1}, IsStage: {2}",
                _context.LogId, _buildSettings.SiteId, _buildSettings.IsStage);
            var root = abstractItems.First(x => x.Discriminator == _buildSettings.RootPageDiscriminator);
            return new AbstractItemStorage(root, abstractItems);
        }

        /// <summary>
        /// Формирование AbstractItem
        /// </summary>
        /// <param name="extensionContentId">Идентификатор контента расширения</param>
        /// <param name="abstractItemPersistentDatas">Идентификаторы связанный AbstractItem</param>
        /// <returns></returns>
        public AbstractItem[] BuildAbstractItems(int extensionContentId, AbstractItemPersistentData[] abstractItemPersistentDatas)
        {
            if (_context == null)
                throw new ArgumentNullException(nameof(_context));

            _logger.LogInformation("AbstractItem build via QP started. Build id: {0}, SiteId: {1}, IsStage: {2}",
                _context.LogId, _buildSettings.SiteId, _buildSettings.IsStage);

            var activatedAbstractItems = new Dictionary<int, AbstractItem>();
            //первый проход списка - активируем, т.е. создаём AbsractItem-ы с правильным типом и набором заполненных полей, запоминаем root
            foreach (var persistentItem in abstractItemPersistentDatas)
            {
                var activatedItem = _itemFactory.Create(persistentItem.Discriminator);
                if (activatedItem == null)
                    continue;

                activatedItem.MapPersistent(persistentItem);
                activatedAbstractItems.Add(persistentItem.Id, activatedItem);
            }

            _logger.LogInformation("Activated abstract items: {0}. Build id: {1}", activatedAbstractItems.Count, _context.LogId);

            if (extensionContentId > 0 || _buildSettings.LoadAbstractItemFieldsToDetailsCollection)
            {
                foreach (var abstractItem in activatedAbstractItems.Values)
                {
                    abstractItem.LazyDetails = new Lazy<AbstractItemExtensionCollection>(() =>
                        BuildDetails(abstractItem,
                            _context.LazyExtensionData[extensionContentId],
                            _context.ExtensionContents,
                            _context.BaseContent,
                            _context.ExtensionsM2MData,
                            _context.LogId));
                }
            }
            else
            {
                _logger.LogInformation(
                    "Skip load data for extension-less elements (LoadAbstractItemFieldsToDetailsCollection = false). Build id: {0}",
                    _context.LogId);
            }

            //догрузим связи m2m в основном контенте, если это нужно

            if (_context.NeedLoadM2mInAbstractItem)
            {
                _logger.LogInformation("Load data for many-to-many fields in main content (QPAbstractItem). Build id: {0}",
                    _context.LogId);

                if (_context.AbstractItemsM2MData != null)
                {
                    foreach (var key in _context.AbstractItemsM2MData.Keys)
                    {
                        if (activatedAbstractItems.TryGetValue(key, out var item))
                        {
                            item.M2mRelations.Merge(_context.AbstractItemsM2MData[key]);
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
                if (item.VersionOfId.HasValue && activatedAbstractItems.ContainsKey(item.VersionOfId.Value))
                {
                    var main = activatedAbstractItems[item.VersionOfId.Value];
                    item.MapVersionOf(main);

                    if (main.ParentId.HasValue && activatedAbstractItems.ContainsKey(main.ParentId.Value))
                        activatedAbstractItems[main.ParentId.Value].AddChild(item);
                }
                else if (item.ParentId.HasValue && activatedAbstractItems.ContainsKey(item.ParentId.Value))
                    activatedAbstractItems[item.ParentId.Value].AddChild(item);
            }
        }

        /// <summary>
        /// Очищаем поля иерархии Parent-Children, VersionOf
        /// </summary>
        /// <param name="abstractItems"></param>
        private static void CleanRelationsBetweenAbstractItems(AbstractItem[] abstractItems)
        {
            foreach (var abstractItem in abstractItems)
            {
                abstractItem
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

            Init(extensionsWithAbsItems);

            var abstractItems = extensionsWithAbsItems
                .SelectMany(x => BuildAbstractItems(x.Key, x.Value))
                .ToArray();
            SetRelationsBetweenAbstractItems(abstractItems);
            return BuildStorage(abstractItems);
        }

        /// <summary>
        /// Формирование контекста
        /// </summary>
        /// <param name="extensions">Content items from AbstractItems content grouped by extension type.</param>
        public void Init(IDictionary<int, AbstractItemPersistentData[]> extensions)
        {
            _context = new AbstractItemStorageBuilderContext
            {
                BaseContent = _metaInfoRepository.GetContent(KnownNetNames.AbstractItem, _buildSettings.SiteId),
            };

            _context.NeedLoadM2mInAbstractItem =
                _buildSettings.LoadAbstractItemFieldsToDetailsCollection
                && _context.BaseContent.ContentAttributes.Any(ca => ca.IsManyToManyField);

            if (extensions != null)
            {
                //получим инфу обо всех контентах-расширениях, которые используются
                _context.ExtensionContents = _metaInfoRepository
                    .GetContentsById(
                        extensions.Select(group => group.Key).Where(key => key > 0).ToArray(),
                        _buildSettings.SiteId)
                    .ToDictionary(c => c.ContentId);

                _context.LazyExtensionData = extensions.ToDictionary(x => x.Key,
                    x => new Lazy<IDictionary<int, AbstractItemExtensionCollection>>(() =>
                        GetAbstractItemExtensionData(x.Key, x.Value.Select(i => i.Id), _context.BaseContent,
                            _context.LogId)));

                (string[] abstractItemTags, string[] extensionItemTags) = GenerateM2mContentTags(extensions.Keys);

                _context.AbstractItemsM2MData = _cacheProvider.GetOrAdd(
                    $"{nameof(Init)}.{nameof(GetAbstractItemsManyToManyRelations)}",
                    abstractItemTags,
                    _cacheSettings.SiteStructureCachePeriod,
                    GetAbstractItemsManyToManyRelations,
                    _buildSettings.CacheFetchTimeoutAbstractItemStorage);

                var allExtensionContentItemIds = _cacheProvider.GetOrAdd(
                    $"{nameof(Init)}.{nameof(GetAbstractItemExtensionIds)}",
                    extensionItemTags,
                    _cacheSettings.SiteStructureCachePeriod,
                    GetAbstractItemExtensionIds,
                    _buildSettings.CacheFetchTimeoutAbstractItemStorage);

                _context.ExtensionsM2MData = _cacheProvider.GetOrAdd(
                    $"{nameof(Init)}.{nameof(GetExtensionItemsManyToManyRelations)}",
                    extensionItemTags,
                    _cacheSettings.SiteStructureCachePeriod,
                    GetExtensionItemsManyToManyRelations,
                    _buildSettings.CacheFetchTimeoutAbstractItemStorage);

                // m2m для базового AbstractItem
                IDictionary<int, M2mRelations> GetAbstractItemsManyToManyRelations() =>
                    _abstractItemRepository.GetManyToManyData(
                        extensions.Values
                            .SelectMany(x => x)
                            .Select(item => item.Id),
                        _buildSettings.IsStage);

                // Получение идентификаторов статей расширений связанных с абстрактными статьями.
                IEnumerable<int> GetAbstractItemExtensionIds() =>
                    _abstractItemRepository.GetAbstractItemExtensionIds(
                        extensions.ToDictionary(ext => ext.Key, ext => ext.Value.Select(ai => ai.Id)),
                        _buildSettings.IsStage);

                // m2m для расширений
                IDictionary<int, M2mRelations> GetExtensionItemsManyToManyRelations() =>
                    _abstractItemRepository.GetManyToManyData(allExtensionContentItemIds, _buildSettings.IsStage);
            }
        }

        private (string[] abstractItemTags, string[] extensionItemTags) GenerateM2mContentTags(IEnumerable<int> extensions)
        {
            var abstractItemTag = _qpContentCacheTagNamingProvider.GetByNetName(
                KnownNetNames.AbstractItem,
                _buildSettings.SiteId,
                _buildSettings.IsStage);

            var itemDefinitionTag = _qpContentCacheTagNamingProvider.GetByNetName(
                KnownNetNames.ItemDefinition,
                _buildSettings.SiteId,
                _buildSettings.IsStage);

            var extensionsTags = _qpContentCacheTagNamingProvider.GetByContentIds(
                extensions.ToArray(),
                _buildSettings.SiteId,
                _buildSettings.IsStage);

            var abstractItemTags = new[] { itemDefinitionTag, abstractItemTag };
            var extensionItemTags = extensionsTags.Values.Concat(abstractItemTags).ToArray();

            return (abstractItemTags, extensionItemTags);
        }

        private IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionId,
            IEnumerable<int> abstractItemIds, ContentPersistentData baseContent, string logId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var abstractItemRepository = scope.ServiceProvider.GetRequiredService<IAbstractItemRepository>();
                var buildSettings = scope.ServiceProvider.GetRequiredService<QpSiteStructureBuildSettings>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<QpAbstractItemStorageBuilder>>();
                return GetAbstractItemExtensionData(abstractItemRepository, buildSettings, logger,
                    extensionId, abstractItemIds, baseContent, logId);
            }
        }

        private static IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(
            IAbstractItemRepository abstractItemRepository,
            QpSiteStructureBuildSettings buildSettings,
            ILogger logger,
            int extensionId,
            IEnumerable<int> abstractItemIds, ContentPersistentData baseContent, string logId)
        {
            logger.LogDebug("Load data from extension table {0}. Build id: {1}", extensionId.ToString(),
                logId);

            var extensionData = extensionId == 0
                ? abstractItemRepository.GetAbstractItemExtensionlessData(abstractItemIds, baseContent,
                    buildSettings.IsStage)
                : abstractItemRepository.GetAbstractItemExtensionData(extensionId, abstractItemIds, baseContent,
                    buildSettings.LoadAbstractItemFieldsToDetailsCollection, buildSettings.IsStage);

            return extensionData;
        }

        /// <summary>
        /// Возвращает данные контента расширения для AbstractItem <paramref name="item"/>
        /// </summary>
        /// <param name="item">AbstractItem</param>
        /// <param name="extensionDataLazy">Словарь, где ключ - это CID расширения, в значение - это данные расширения</param>
        /// <param name="extensionContents"></param>
        /// <param name="baseContent"></param>
        /// <param name="extensionsM2MData">Данные о связях m2m у расширения</param>
        /// <param name="logId"></param>
        /// <returns></returns>
        private AbstractItemExtensionCollection BuildDetails(AbstractItem item,
            Lazy<IDictionary<int, AbstractItemExtensionCollection>> extensionDataLazy,
            IDictionary<int, ContentPersistentData> extensionContents,
            ContentPersistentData baseContent,
            IDictionary<int, M2mRelations> extensionsM2MData,
            string logId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var qpUrlResolver = scope.ServiceProvider.GetRequiredService<IQpUrlResolver>();
                var buildSettings = scope.ServiceProvider.GetRequiredService<QpSiteStructureBuildSettings>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<QpAbstractItemStorageBuilder>>();
                return BuildDetails(qpUrlResolver, buildSettings, logger,
                    item, extensionDataLazy, extensionContents, baseContent, extensionsM2MData, logId);
            }
        }

        /// <summary>
        /// Возвращает данные контента расширения для AbstractItem <paramref name="item"/>
        /// </summary>
        /// <param name="buildSettings"></param>
        /// <param name="logger"></param>
        /// <param name="item">AbstractItem</param>
        /// <param name="extensionDataLazy">Словарь, где ключ - это CID расширения, в значение - это данные расширения</param>
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
            Lazy<IDictionary<int, AbstractItemExtensionCollection>> extensionDataLazy,
            IDictionary<int, ContentPersistentData> extensionContents,
            ContentPersistentData baseContent,
            IDictionary<int, M2mRelations> extensionsM2MData,
            string logId)
        {
            var extensionContentId = item.ExtensionId.GetValueOrDefault(0);

            var extensionData = extensionDataLazy.Value;
            if (!extensionData.TryGetValue(item.Id, out var details))
            {
                logger.LogDebug("Not found data for extension {0}. Build id: {1}", extensionContentId.ToString(),
                    logId);
                return null;
            }

            var extensionContent = extensionContents.ContainsKey(extensionContentId) ? extensionContents[extensionContentId] : null;

            var m2mFieldNames = new List<string>();
            var fileFields = new List<ContentAttributePersistentData>();
            int? extensionContentItemId = null;

            if (extensionContent?.ContentAttributes != null)
            {
                m2mFieldNames = extensionContent.ContentAttributes.Where(ca => ca.IsManyToManyField)
                    .Select(ca => ca.ColumnName).ToList();
                fileFields = extensionContent.ContentAttributes.Where(ca => ca.IsFileField).ToList();
            }

            if (buildSettings.LoadAbstractItemFieldsToDetailsCollection)
            {
                m2mFieldNames = m2mFieldNames.Union(baseContent.ContentAttributes.Where(ca => ca.IsManyToManyField)
                    .Select(ca => ca.ColumnName)).ToList();
                fileFields = fileFields.Union(baseContent.ContentAttributes.Where(ca => ca.IsFileField)).ToList();
            }

            item.M2mFieldNames.AddRange(m2mFieldNames);

            //проведём замены в некоторых значениях доп полей
            foreach (var key in details.Keys)
            {
                if (m2mFieldNames.Any() && key == "CONTENT_ITEM_ID")
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
                    var fileField = fileFields.FirstOrDefault(f =>
                        f.ColumnName.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                    if (fileField != null)
                    {
                        var baseUrl = qpUrlResolver.UrlForImage(buildSettings.SiteId, fileField);
                        if (!string.IsNullOrEmpty(baseUrl))
                        {
                            details.Set(key, baseUrl + "/" + stringValue);
                        }
                    }
                }
            }

            //установим связи m2m в контентах расширений, в которых они есть
            if (extensionContentItemId.HasValue &&
                extensionsM2MData != null &&
                extensionsM2MData.TryGetValue(extensionContentItemId.Value, out var relations))
            {
                item.M2mRelations.Merge(relations);
            }

            return details;
        }
    }
}
