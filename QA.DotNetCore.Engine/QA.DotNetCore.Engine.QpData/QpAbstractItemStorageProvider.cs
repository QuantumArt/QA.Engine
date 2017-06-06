using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.QpData.Replacements;
using System.Collections.Concurrent;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Строитель структуры сайта из базы QP
    /// </summary>
    public class QpAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        IServiceProvider _serviceProvider;
        IAbstractItemFactory _itemFactory;
        ICacheProvider _cacheProvider;
        IQpUrlResolver _qpUrlResolver;
        IAbstractItemRepository _abstractItemRepository;
        QpSiteStructureSettings _settings;
        QpSettings _qpSettings;
        SiteMode _siteMode;

        public QpAbstractItemStorageProvider(
            IServiceProvider serviceProvider,
            IAbstractItemFactory itemFactory,
            ICacheProvider cacheProvider,
            IQpUrlResolver qpUrlResolver,
            IAbstractItemRepository abstractItemRepository,
            IOptions<QpSiteStructureSettings> settings,
            IOptions<QpSettings> qpSettings,
            IOptions<SiteMode> siteMode)
        {
            _serviceProvider = serviceProvider;
            _itemFactory = itemFactory;
            _cacheProvider = cacheProvider;
            _qpUrlResolver = qpUrlResolver;
            _abstractItemRepository = abstractItemRepository;
            _settings = settings.Value;
            _qpSettings = qpSettings.Value;
            _siteMode = siteMode.Value;
        }

        public AbstractItemStorage Get()
        {
            if (!_settings.UseCache)
                return GetInternal();

            var cacheKey = "QpAbstractItemStorageProvider.Get";
            var result = _cacheProvider.Get(cacheKey) as AbstractItemStorage;
            if (result == null)
            {
                result = GetInternal();
                if (result != null)
                {
                    _cacheProvider.Set(cacheKey, result, _settings.CachePeriod);
                }
            }
            return result;
        }

        private AbstractItemStorage GetInternal()
        {
            var plainList = _abstractItemRepository.GetPlainAllAbstractItems(_qpSettings.SiteId, _siteMode.IsStage);//плоский список dto
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

                //догрузим доп поля
                foreach (var group in groupsByExtensions)
                {
                    var extensionId = group.Key;
                    var ids = group.Select(_ => _.Id).ToArray();

                    var extensions = _abstractItemRepository.GetAbstractItemExtensionData(extensionId, ids, _siteMode.IsStage);

                    //словарь соответствий полей и атрибутов ILoaderOption
                    ILookup<string, ILoaderOption> optionsMap = null;
                    foreach (var item in group)
                    {
                        if (extensions.ContainsKey(item.Id))
                        {
                            if (optionsMap == null)
                            {
                                optionsMap = ProcessType(item.GetContentType(), extensionId)?.ToLookup(x => x.PropertyName);
                            }

                            var details = extensions[item.Id];

                            //проведём замены в некоторых значениях доп полей
                            foreach (var key in details.Keys)
                            {
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

        private readonly ConcurrentDictionary<Type, IReadOnlyList<ILoaderOption>> _needToResolve = new ConcurrentDictionary<Type, IReadOnlyList<ILoaderOption>>();
        private IReadOnlyList<ILoaderOption> ProcessType(Type t, int contentId)
        {
            return _needToResolve.GetOrAdd(t, key =>
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
    }
}
