using Common.PageModel;
using Common.Persistent.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Persistent.Dapper
{
    public class QpAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        IUnitOfWork _unitOfWork;
        AbstractItemActivator _activator;

        const int RootPageDiscriminator = 231087;

        public QpAbstractItemStorageProvider(IUnitOfWork uow, AbstractItemActivator activator)
        {
            _unitOfWork = uow;
            _activator = activator;
        }

        public AbstractItemStorage Get(int? rootPageId = null)
        {
            var plainList = _unitOfWork.AbstractItemRepository.GetPlainAllAbstractItems();//плоский список dto
            var parentMapping = new Dictionary<int, List<int>>();//соответсвие id - parentId
            var activated = new Dictionary<int, AbstractItem>();
            AbstractItem root = null;

            //первый проход списка - активируем, т.е. создаём AbsractItem-ы с правильным типом, запоминаем родительские связи и root
            foreach (var persistentItem in plainList)
            {
                var activatedItem = _activator.Activate(persistentItem);
                activated.Add(persistentItem.Id, activatedItem);

                var parentId = persistentItem.ParentId ?? 0;
                if (!parentMapping.ContainsKey(parentId))
                    parentMapping[parentId] = new List<int>();
                parentMapping[parentId].Add(persistentItem.Id);

                if (rootPageId.HasValue)
                {
                    if (persistentItem.Id == rootPageId.Value)
                        root = activatedItem;
                }
                else if (persistentItem.Discriminator == RootPageDiscriminator)
                    root = activatedItem;
            }

            if (root != null)
            {
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
    }
}
