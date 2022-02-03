using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Models;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Строитель структуры сайта из базы QP
    /// </summary>
    public class QpAbstractItemStorageBuilder : IAbstractItemStorageBuilder
    {
        private AbstractItemStorageBuilderContext _context;

        private readonly IAbstractItemFactory _itemFactory;
        private readonly IQpUrlResolver _qpUrlResolver;
        private readonly IAbstractItemRepository _abstractItemRepository;
        private readonly IMetaInfoRepository _metaInfoRepository;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly ILogger<QpAbstractItemStorageBuilder> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public QpAbstractItemStorageBuilder(
            IAbstractItemFactory itemFactory,
            IQpUrlResolver qpUrlResolver,
            IAbstractItemRepository abstractItemRepository,
            IMetaInfoRepository metaInfoRepository,
            QpSiteStructureBuildSettings buildSettings,
            ILogger<QpAbstractItemStorageBuilder> logger,
            IServiceScopeFactory scopeFactory)
        {
            _itemFactory = itemFactory;
            _qpUrlResolver = qpUrlResolver;
            _abstractItemRepository = abstractItemRepository;
            _metaInfoRepository = metaInfoRepository;
            _buildSettings = buildSettings;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }


        public AbstractItemStorage BuildStorage(AbstractItem[] abstractItems)
        {
            _logger.LogDebug(
                "AbstractItemStorage build via AbstractItems collection started. Build id: {0}, SiteId: {0}, IsStage: {1}",
                _context.LogId, _buildSettings.SiteId, _buildSettings.IsStage);
            var root = abstractItems.First(x => x.Discriminator == _buildSettings.RootPageDiscriminator);
            return new AbstractItemStorage(root, abstractItems);
        }

        private static int Counter = 0;
        /// <summary>
        /// Формирование AbstractItem
        /// </summary>
        /// <param name="extensionContentId">Идентификатор контента расширения</param>
        /// <param name="abstractItemPersistentDatas">Идентификаторы связанный AbstractItem</param>
        /// <returns></returns>
        public AbstractItem[] BuildAbstractItems(int extensionContentId, AbstractItemPersistentData[] abstractItemPersistentDatas)
        {
            Counter++;
            if (_context == null)
                throw new ArgumentNullException(nameof(_context));

            _logger.LogDebug("AbstractItem build via QP started. Build id: {0}, SiteId: {0}, IsStage: {1}",
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


            _logger.LogDebug("Activated abstract items: {0}. Build id: {1}", activatedAbstractItems.Count, _context.LogId);

            if (extensionContentId > 0 || _buildSettings.LoadAbstractItemFieldsToDetailsCollection)
            {
                foreach (var abstractItem in activatedAbstractItems.Values)
                {
                    abstractItem.Details = new Lazy<AbstractItemExtensionCollection>(() =>
                        BuildDetails(abstractItem,
                            _context.ExtensionDataLazy[extensionContentId],
                            _context.ExtensionContents,
                            _context.BaseContent,
                            _context.ExtensionsM2MData,
                            _context.LogId));
                }
            }
            else
            {
                _logger.LogDebug(
                    "Skip load data for extension-less elements (LoadAbstractItemFieldsToDetailsCollection = false). Build id: {1}",
                    _context.LogId);
            }

            //догрузим связи m2m в основном контенте, если это нужно

            if (_context.NeedLoadM2mInAbstractItem)
            {
                _logger.LogDebug("Load data for many-to-many fields in main content (QPAbstractItem). Build id: {1}",
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
            var extensionsWithAbsItems = _abstractItemRepository.GetExtensionContentsWithPlainAbstractItems(
                _buildSettings.SiteId,
                _buildSettings.IsStage);
            BuildContext(extensionsWithAbsItems);

            var abstractItems = extensionsWithAbsItems
                .SelectMany(x => BuildAbstractItems(x.Key, x.Value))
                .ToArray();
            SetRelationsBetweenAbstractItems(abstractItems);
            return BuildStorage(abstractItems);
        }

        /// <summary>
        /// Формирование контекста
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public void BuildContext(IDictionary<int, AbstractItemPersistentData[]> extensions)
        {
            _context = new AbstractItemStorageBuilderContext
            {
                BaseContent = _metaInfoRepository.GetContent(KnownNetNames.AbstractItem, _buildSettings.SiteId),
            };
            _context.NeedLoadM2mInAbstractItem = _buildSettings.LoadAbstractItemFieldsToDetailsCollection &&
                                                 _context.BaseContent.ContentAttributes.Any(ca => ca.IsManyToManyField);

            if (extensions != null)
            {
                //получим инфу обо всех контентах-расширениях, которые используются
                _context.ExtensionContents = _metaInfoRepository
                    .GetContentsById(extensions
                        .Select(g => g.Key)
                        .Where(extId => extId > 0)
                        .ToArray(), _buildSettings.SiteId)
                    .ToDictionary(c => c.ContentId);


                _context.ExtensionDataLazy = extensions.ToDictionary(x => x.Key,
                    x => new Lazy<IDictionary<int, AbstractItemExtensionCollection>>(() =>
                        GetAbstractItemExtensionData(x.Key, x.Value.Select(i => i.Id), _context.BaseContent,
                            _context.LogId)));

                // m2m для базового AbstractItem
                _context.AbstractItemsM2MData = _abstractItemRepository.GetManyToManyData(
                    extensions.Values
                        .SelectMany(x=>x)
                        .Select(x=>x.Id),
                    _buildSettings.IsStage);

                // m2m для расширений
                var allExtensionContentItemIds = _abstractItemRepository.GetAbstractItemExtensionIds(
                    extensions.ToDictionary(ext => ext.Key, ext => ext.Value.Select(ai => ai.Id)),
                    _buildSettings.IsStage);
                _context.ExtensionsM2MData =
                    _abstractItemRepository.GetManyToManyData(allExtensionContentItemIds, _buildSettings.IsStage);
            }
        }

        private IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionId,
            IEnumerable<int> abstractItemIds, ContentPersistentData baseContent, string logId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var abstractItemRepository = scope.ServiceProvider.GetRequiredService<IAbstractItemRepository>();
                var buildSettings = scope.ServiceProvider.GetRequiredService<QpSiteStructureBuildSettings>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<QpAbstractItemStorageBuilder>>();
                return GetAbstractItemExtensionData(abstractItemRepository,buildSettings,logger,
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
        /// <param name="item">AbstractItem</param>
        /// <param name="extensionDataLazy">Словарь, где ключ - это CID расширения, в значение - это данные расширения</param>
        /// <param name="extensionContents"></param>
        /// <param name="baseContent"></param>
        /// <param name="extensionsM2MData">Данные о связях m2m у расширения</param>
        /// <param name="logId"></param>
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
                        if (!String.IsNullOrEmpty(baseUrl))
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
