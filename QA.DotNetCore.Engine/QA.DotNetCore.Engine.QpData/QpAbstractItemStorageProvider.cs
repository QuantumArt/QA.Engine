using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching;

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
        QpSiteStructureSettings _settings;

        public QpAbstractItemStorageProvider(IServiceProvider serviceProvider, IAbstractItemFactory itemFactory, IOptions<QpSiteStructureSettings> settings, ICacheProvider cacheProvider)
        {
            _serviceProvider = serviceProvider;
            _itemFactory = itemFactory;
            _settings = settings.Value;
            _cacheProvider = cacheProvider;
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
            var _unitOfWork = _serviceProvider.GetService<IUnitOfWork>();//lifetime подключения к базе может быть короче, чем lifetime этого класса, поэтому достаём из контейнера, а не делаем DI
            var plainList = _unitOfWork.AbstractItemRepository.GetPlainAllAbstractItems();//плоский список dto
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

                    var extensions = _unitOfWork.AbstractItemRepository.GetAbstractItemExtensionData(extensionId, ids);
                    foreach (var item in group)
                    {
                        if (extensions.ContainsKey(item.Id))
                            item.Details = extensions[item.Id];
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
    }
}
