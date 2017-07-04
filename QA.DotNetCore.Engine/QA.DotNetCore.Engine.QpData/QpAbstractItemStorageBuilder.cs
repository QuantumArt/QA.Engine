using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
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
        IServiceProvider _serviceProvider;
        IAbstractItemFactory _itemFactory;
        IQpUrlResolver _qpUrlResolver;
        IAbstractItemRepository _abstractItemRepository;
        QpSiteStructureSettings _settings;
        QpSettings _qpSettings;

        public QpAbstractItemStorageBuilder(
            IServiceProvider serviceProvider,
            IAbstractItemFactory itemFactory,
            IQpUrlResolver qpUrlResolver,
            IAbstractItemRepository abstractItemRepository,
            QpSiteStructureSettings settings,
            QpSettings qpSettings)
        {
            _serviceProvider = serviceProvider;
            _itemFactory = itemFactory;
            _qpUrlResolver = qpUrlResolver;
            _abstractItemRepository = abstractItemRepository;
            _settings = settings;
            _qpSettings = qpSettings;
        }

        public AbstractItemStorage Build()
        {
            var plainList = _abstractItemRepository.GetPlainAllAbstractItems(_qpSettings.SiteId, _qpSettings.IsStage);//плоский список dto
            var parentMapping = new Dictionary<int, List<int>>();//соответсвие id - parentId
            var activated = new Dictionary<int, AbstractItem>();
            AbstractItem root = null;

            //первый проход списка - активируем, т.е. создаём AbsractItem-ы с правильным типом, запоминаем родительские связи и root
            foreach (var persistentItem in plainList)
            {
                var activatedItem = _itemFactory.Create(persistentItem.Discriminator);
                if (activatedItem == null)
                    continue;

                MapAbstractItem(activatedItem as AbstractItem, persistentItem);
                activated.Add(persistentItem.Id, activatedItem);

                var parentId = persistentItem.ParentId ?? 0;
                if (!parentMapping.ContainsKey(parentId))
                    parentMapping[parentId] = new List<int>();
                parentMapping[parentId].Add(persistentItem.Id);

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
                var needLoadM2MDict = new Dictionary<int, AbstractItem>();

                //догрузим доп поля
                foreach (var group in groupsByExtensions)
                {
                    var extensionId = group.Key;
                    var ids = group.Select(_ => _.Id).ToArray();

                    var extensions = _abstractItemRepository.GetAbstractItemExtensionData(extensionId, ids, _qpSettings.IsStage);

                    //словарь соответствий полей и атрибутов ILoaderOption
                    ILookup<string, ILoaderOption> optionsMap = null;
                    foreach (var item in group)
                    {
                        if (extensions.ContainsKey(item.Id))
                        {
                            if (optionsMap == null)
                            {
                                optionsMap = ProcessLoadOptions(item.GetContentType(), extensionId)?.ToLookup(x => x.PropertyName);
                            }

                            var needLoadM2m = NeedManyToManyLoad(item.GetContentType());
                            var details = extensions[item.Id];

                            //проведём замены в некоторых значениях доп полей
                            foreach (var key in details.Keys)
                            {
                                if (needLoadM2m && key == "CONTENT_ITEM_ID")
                                {
                                    needLoadM2MDict[Convert.ToInt32(details[key])] = item;
                                }

                                if (details[key] is string stringValue)
                                {
                                    //1) надо заменить плейсхолдер <%=upload_url%> на реальный урл
                                    details.Set(key, stringValue.Replace("<%=upload_url%>", _qpUrlResolver.UploadUrl()));

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

                //догрузим связи m2m
                if (needLoadM2MDict.Any())
                {
                    var m2mData = _abstractItemRepository.GetAbstractItemManyToManyData(needLoadM2MDict.Keys.ToArray(), _qpSettings.IsStage);
                    foreach (var key in m2mData.Keys)
                    {
                        if (!needLoadM2MDict.ContainsKey(key))
                            continue;

                        needLoadM2MDict[key].M2mRelations = m2mData[key];
                    }
                }

                //рекурсивно, начиная от корня, заполняем Children
                FillChildrenRecursive(root, activated, parentMapping);
                return new AbstractItemStorage(root);
            }
            else
            {
                return null;
            }
        }

        private void FillChildrenRecursive(AbstractItem root, Dictionary<int, AbstractItem> activated, Dictionary<int, List<int>> parentMapping)
        {
            if (parentMapping.ContainsKey(root.Id))
            {
                foreach (var childId in parentMapping[root.Id])
                {
                    var child = activated[childId];
                    root.AddChild(child);
                    FillChildrenRecursive(child, activated, parentMapping);
                }
            }

        }

        private void MapAbstractItem(AbstractItem item, AbstractItemPersistentData persistentItem)
        {
            item.Id = persistentItem.Id;
            item.Alias = persistentItem.Alias;
            item.Title = persistentItem.Title;
            item.ExtensionId = persistentItem.ExtensionId;
            if (!item.IsPage)
            {
                if (item is AbstractWidget wigdet)
                {
                    wigdet.ZoneName = persistentItem.ZoneName;
                }
            }
        }

        private readonly ConcurrentDictionary<Type, IReadOnlyList<ILoaderOption>> _loadOptions = new ConcurrentDictionary<Type, IReadOnlyList<ILoaderOption>>();
        private IReadOnlyList<ILoaderOption> ProcessLoadOptions(Type t, int contentId)
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
                        attr.AttachTo(t, prop.Name, contentId);
                        lst.Add(attr);
                    }
                }

                return lst;
            });
        }

        private readonly ConcurrentDictionary<Type, bool> _m2mOptions = new ConcurrentDictionary<Type, bool>();
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
