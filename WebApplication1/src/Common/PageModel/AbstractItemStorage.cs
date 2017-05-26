using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public class AbstractItemStorage
    {
        private Dictionary<int, AbstractItem> _items = new Dictionary<int, AbstractItem>();
        public AbstractItem Root { get; private set; }

        public AbstractItemStorage(AbstractItem root)
        {
            Root = root;
            AddItemRecursive(root);
        }

        private void AddItemRecursive(AbstractItem item)
        {
            _items[item.Id] = item;
            foreach (var child in item.Children)
            {
                AddItemRecursive(child);
            }
        }


        public AbstractItem Get(int id)
        {
            return _items[id];
        }
    }
}
