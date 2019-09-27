using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Строитель структуры сайта из базы QP
    /// </summary>
    public class QpAbstractItemStorageBuilder : IAbstractItemStorageBuilder
    {
        private readonly IAbstractItemFactory _itemFactory;
        private readonly IQpUrlResolver _qpUrlResolver;
        private readonly IAbstractItemRepository _abstractItemRepository;
        private readonly IMetaInfoRepository _metaInfoRepository;
        private readonly QpSiteStructureBuildSettings _buildSettings;
        private readonly ILogger<QpAbstractItemStorageBuilder> _logger;

        public QpAbstractItemStorageBuilder(
            IAbstractItemFactory itemFactory,
            IQpUrlResolver qpUrlResolver,
            IAbstractItemRepository abstractItemRepository,
            IMetaInfoRepository metaInfoRepository,
            IItemDefinitionRepository itemDefinitionRepository,
            QpSiteStructureBuildSettings buildSettings,
            ILogger<QpAbstractItemStorageBuilder> logger)
        {
            _itemFactory = itemFactory;
            _qpUrlResolver = qpUrlResolver;
            _abstractItemRepository = abstractItemRepository;
            _metaInfoRepository = metaInfoRepository;
            _buildSettings = buildSettings;
            _logger = logger;
            UsedContentNetNames = new string[2] { abstractItemRepository.AbstractItemNetName, itemDefinitionRepository.ItemDefinitionNetName };
        }

        public AbstractItemStorage Build()
        {
            var logBuildId = Guid.NewGuid();
            _logger.LogDebug("AbstractItemStorage build via QP started. Build id: {0}, SiteId: {0}, IsStage: {1}", logBuildId, _buildSettings.SiteId, _buildSettings.IsStage);

            var plainList = _abstractItemRepository.GetPlainAllAbstractItems(_buildSettings.SiteId, _buildSettings.IsStage).ToList();//плоский список dto
            var activated = new Dictionary<int, AbstractItem>();
            AbstractItem root = null;

            _logger.LogDebug("Found abstract items: {0}. Build id: {1}", plainList.Count(), logBuildId);

            //первый проход списка - активируем, т.е. создаём AbsractItem-ы с правильным типом и набором заполненных полей, запоминаем root
            foreach (var persistentItem in plainList)
            {
                var activatedItem = _itemFactory.Create(persistentItem.Discriminator);
                if (activatedItem == null)
                    continue;

                activatedItem.MapPersistent(persistentItem);
                activated.Add(persistentItem.Id, activatedItem);

                if (persistentItem.Discriminator == _buildSettings.RootPageDiscriminator)
                    root = activatedItem;
            }

            _logger.LogDebug("Activated abstract items: {0}. Build id: {1}", activated.Count, logBuildId);

            if (root != null)
            {
                //сгруппируем AbsractItem-ы по extensionId
                var groupsByExtensions = activated
                    .Where(_ => _.Value.ExtensionId.HasValue)
                    .GroupBy(_ => _.Value.ExtensionId.Value, _ => _.Value)
                    .ToList();

                //словарь, каким элементам нужна загрузка связи m2m
                var needLoadM2mInExtensionDict = new Dictionary<int, AbstractItem>();

                //получим инфу об основном контенте (AbstractItem), она нам пригодится. Мы зашиваемся на netName контента - это легально
                var baseContent = _metaInfoRepository.GetContent(_abstractItemRepository.AbstractItemNetName, _buildSettings.SiteId);
                if (baseContent == null)
                {
                    _logger.LogWarning($"Failed to obtain content definition for {1}. Build id: {0}", logBuildId, _abstractItemRepository.AbstractItemNetName);
                }

                //догрузим доп поля
                foreach (var group in groupsByExtensions)
                {
                    var extensionId = group.Key;
                    var ids = group.Select(_ => _.Id).ToArray();

                    _logger.LogDebug("Load data from extension table {0}. Build id: {1}", extensionId, logBuildId);

                    var extensions = _abstractItemRepository.GetAbstractItemExtensionData(extensionId, ids, baseContent, _buildSettings.LoadAbstractItemFieldsToDetailsCollection, _buildSettings.IsStage);

                    //словарь соответствий полей и атрибутов ILoaderOption
                    ILookup<string, ILoaderOption> optionsMap = null;
                    foreach (var item in group)
                    {
                        if (extensions.ContainsKey(item.Id))
                        {
                            if (optionsMap == null)
                            {
                                optionsMap = ProcessLoadOptions(item.GetContentType(), extensionId, baseContent?.ContentId ?? 0)?.ToLookup(x => x.PropertyName);
                            }

                            var needLoadM2m = NeedManyToManyLoad(item.GetContentType());
                            var details = extensions[item.Id];

                            //проведём замены в некоторых значениях доп полей
                            foreach (var key in details.Keys)
                            {
                                if (needLoadM2m && key == "CONTENT_ITEM_ID")
                                {
                                    needLoadM2mInExtensionDict[Convert.ToInt32(details[key])] = item;
                                }

                                if (details[key] is string stringValue)
                                {
                                    //1) надо заменить плейсхолдер <%=upload_url%> на реальный урл
                                    details.Set(key, stringValue.Replace(_buildSettings.UploadUrlPlaceholder, _qpUrlResolver.UploadUrl(_buildSettings.SiteId)));

                                    //2) если поле помечено атрибутом интерфейса ILoaderOption, то нужно преобразовать значение этого поля согласно внутренней логике атрибута
                                    //пример: атрибут LibraryUrl значит, что поле является файлом в библиотеке сайта, нужно чтобы в значении этого поля был полный урл до этого файла
                                    if (optionsMap != null)
                                    {
                                        foreach (var option in optionsMap[key])
                                        {
                                            details.Set(key, option.Process(_qpUrlResolver, stringValue, _buildSettings.SiteId));
                                        }
                                    }
                                }
                            }

                            item.Details = details;
                        }

                    }
                }

                //догрузим связи m2m в контентах расширений
                if (needLoadM2mInExtensionDict.Any())
                {
                    _logger.LogDebug("Load data for many-to-many fields in extensions. Build id: {1}", logBuildId);
                    var m2mData = _abstractItemRepository.GetManyToManyData(needLoadM2mInExtensionDict.Keys.ToArray(), _buildSettings.IsStage);
                    foreach (var key in m2mData.Keys)
                    {
                        if (!needLoadM2mInExtensionDict.ContainsKey(key))
                            continue;

                        needLoadM2mInExtensionDict[key].M2mRelations.Merge(m2mData[key]);
                    }
                }

                //догрузим связи m2m в основном контенте
                if (_buildSettings.LoadM2mForAbstractItem)
                {
                    _logger.LogDebug("Load data for many-to-many fields in main content (QPAbstractItem). Build id: {1}", logBuildId);
                    var m2mData = _abstractItemRepository.GetManyToManyData(activated.Keys.ToArray(), _buildSettings.IsStage);
                    foreach (var key in m2mData.Keys)
                    {
                        activated[key].M2mRelations.Merge(m2mData[key]);
                    }
                }

                //второй проход списка: заполняем поля иерархии Parent-Children, на основании ParentId. Заполняем VersionOf
                foreach (var item in activated.Values)
                {
                    if (item.VersionOfId.HasValue && activated.ContainsKey(item.VersionOfId.Value))
                    {
                        var main = activated[item.VersionOfId.Value];
                        item.MapVersionOf(main);

                        if (main.ParentId.HasValue && activated.ContainsKey(main.ParentId.Value))
                            activated[main.ParentId.Value].AddChild(item);
                    }
                    else if (item.ParentId.HasValue && activated.ContainsKey(item.ParentId.Value))
                        activated[item.ParentId.Value].AddChild(item);
                }

                return new AbstractItemStorage(root);
            }
            else
            {
                _logger.LogWarning(@"Root was not found! Possible causes: 1) not found item with discriminator = {0} 2) not found item definition for root discriminator 3) not found .net class matching with root page item definition. Build id: {1}", _buildSettings.RootPageDiscriminator, logBuildId);
                return null;
            }
        }

        /// <summary>
        /// Нужно для определения списка кеш-тегов. Чтобы понимать обновления в каких контентах должны приводить к сбрасыванию закешированной структуры сайта
        /// </summary>
        public string[] UsedContentNetNames { get; private set; }

        private readonly ConcurrentDictionary<Type, IReadOnlyList<ILoaderOption>> _loadOptions = new ConcurrentDictionary<Type, IReadOnlyList<ILoaderOption>>();

        /// <summary>
        /// Получить список опций загрузки для типа
        /// </summary>
        /// <param name="t"></param>
        /// <param name="contentId"></param>
        /// <param name="baseContentId"></param>
        /// <returns></returns>
        private IReadOnlyList<ILoaderOption> ProcessLoadOptions(Type t, int contentId, int baseContentId)
        {
            return _loadOptions.GetOrAdd(t, key =>
            {
                var lst = new List<ILoaderOption>();
                var properties = t.GetProperties();

                foreach (var prop in properties)
                {
                    var attrs = prop.GetCustomAttributes(typeof(ILoaderOption), false)
                        .Cast<ILoaderOption>();

                    foreach (var attr in attrs)
                    {
                        attr.AttachTo(t, (attr.PropertyName ?? prop.Name).ToUpper(), contentId, baseContentId);
                        lst.Add(attr);
                    }
                }

                return lst;
            });
        }

        private readonly ConcurrentDictionary<Type, bool> _m2mOptions = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// Нужно ли грузить M2M для экстеншна, соответствующего типу
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool NeedManyToManyLoad(Type t)
        {
            return _m2mOptions.GetOrAdd(t, key =>
            {
                //если атрибутом LoadManyToManyRelations помечен сам класс или какое-то из его полей
                return t.GetCustomAttributes(typeof(LoadManyToManyRelationsAttribute), false).Any()
                    || t.GetProperties().Any(prop => prop.GetCustomAttributes(typeof(LoadManyToManyRelationsAttribute), false).Any());
            });
        }
    }
}
