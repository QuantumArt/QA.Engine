using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Строитель структуры сайта из базы QP
    /// </summary>
    public class QpAbstractItemStorageBuilder : IAbstractItemStorageBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAbstractItemFactory _itemFactory;
        private readonly IQpUrlResolver _qpUrlResolver;
        private readonly IAbstractItemRepository _abstractItemRepository;
        private readonly IMetaInfoRepository _metaInfoRepository;
        private readonly QpSiteStructureSettings _settings;
        private readonly QpSettings _qpSettings;
        private readonly string[] _usedContentNetNames;

        public QpAbstractItemStorageBuilder(
            IServiceProvider serviceProvider,
            IAbstractItemFactory itemFactory,
            IQpUrlResolver qpUrlResolver,
            IAbstractItemRepository abstractItemRepository,
            IMetaInfoRepository metaInfoRepository,
            IItemDefinitionRepository itemDefinitionRepository,
            QpSiteStructureSettings settings,
            QpSettings qpSettings)
        {
            _serviceProvider = serviceProvider;
            _itemFactory = itemFactory;
            _qpUrlResolver = qpUrlResolver;
            _abstractItemRepository = abstractItemRepository;
            _metaInfoRepository = metaInfoRepository;
            _settings = settings;
            _qpSettings = qpSettings;
            _usedContentNetNames = new string[2] { abstractItemRepository.AbstractItemNetName, itemDefinitionRepository.ItemDefinitionNetName };
        }

        public AbstractItemStorage Build()
        {
            var plainList = _abstractItemRepository.GetPlainAllAbstractItems(_qpSettings.SiteId, _qpSettings.IsStage);//плоский список dto
            var activated = new Dictionary<int, AbstractItem>();
            AbstractItem root = null;

            //первый проход списка - активируем, т.е. создаём AbsractItem-ы с правильным типом и набором заполненных полей, запоминаем root
            foreach (var persistentItem in plainList)
            {
                var activatedItem = _itemFactory.Create(persistentItem.Discriminator);
                if (activatedItem == null)
                    continue;

                activatedItem.MapPersistent(persistentItem);
                activated.Add(persistentItem.Id, activatedItem);

                if (persistentItem.Discriminator == _settings.RootPageDiscriminator)
                    root = activatedItem;
            }

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
                var baseContent = _metaInfoRepository.GetContent("QPAbstractItem", _qpSettings.SiteId);

                //догрузим доп поля
                foreach (var group in groupsByExtensions)
                {
                    var extensionId = group.Key;
                    var ids = group.Select(_ => _.Id).ToArray();

                    var extensions = _abstractItemRepository.GetAbstractItemExtensionData(extensionId, ids, _settings.LoadAbstractItemFieldsToDetailsCollection, _qpSettings.IsStage);

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
                                    details.Set(key, stringValue.Replace(_settings.UploadUrlPlaceholder, _qpUrlResolver.UploadUrl(_qpSettings.SiteId)));

                                    //2) если поле помечено атрибутом интерфейса ILoaderOption, то нужно преобразовать значение этого поля согласно внутренней логике атрибута
                                    //пример: атрибут LibraryUrl значит, что поле является файлом в библиотеке сайта, нужно чтобы в значении этого поля был полный урл до этого файла
                                    if (optionsMap != null)
                                    {
                                        foreach (var option in optionsMap[key])
                                        {
                                            details.Set(key, option.Process(_serviceProvider, stringValue));
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
                    var m2mData = _abstractItemRepository.GetManyToManyData(needLoadM2mInExtensionDict.Keys.ToArray(), _qpSettings.IsStage);
                    foreach (var key in m2mData.Keys)
                    {
                        if (!needLoadM2mInExtensionDict.ContainsKey(key))
                            continue;

                        needLoadM2mInExtensionDict[key].M2mRelations.Merge(m2mData[key]);
                    }
                }

                //догрузим связи m2m в основном контенте
                if (_settings.LoadM2mForAbstractItem)
                {
                    var m2mData = _abstractItemRepository.GetManyToManyData(activated.Keys.ToArray(), _qpSettings.IsStage);
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

                return new AbstractItemStorage(root, _serviceProvider);
            }
            else
            {
                return null;
            }
        }

        public string[] UsedContentNetNames
        {
            get { return _usedContentNetNames; }
        }

        private readonly ConcurrentDictionary<Type, IReadOnlyList<ILoaderOption>> _loadOptions = new ConcurrentDictionary<Type, IReadOnlyList<ILoaderOption>>();

        /// <summary>
        /// Получить список опций загрузки для типа
        /// </summary>
        /// <param name="t"></param>
        /// <param name="contentId"></param>
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
                        attr.AttachTo(t, attr.PropertyName ?? prop.Name, contentId, baseContentId);
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
