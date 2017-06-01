using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Строитель структуры сайта из базы QP
    /// </summary>
    public class QpAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        IUnitOfWork _unitOfWork;
        IAbstractItemFactory _itemFactory;

        const string RootPageDiscriminator = "root_page";

        public QpAbstractItemStorageProvider(IUnitOfWork uow, IAbstractItemFactory itemFactory)
        {
            _unitOfWork = uow;
            _itemFactory = itemFactory;
        }

        public AbstractItemStorage Get()
        {
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

                if (persistentItem.Discriminator == RootPageDiscriminator)
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
