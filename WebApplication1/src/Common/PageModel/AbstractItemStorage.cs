using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public class AbstractItemStorage
    {
        private Dictionary<int, IAbstractItem> _items = new Dictionary<int, IAbstractItem>();
        public IAbstractItem Root { get; private set; }

        public AbstractItemStorage(IAbstractItem root)
        {
            Root = root;
            AddItemRecursive(root);
        }

        private void AddItemRecursive(IAbstractItem item)
        {
            _items[item.Id] = item;
            foreach (var child in item.Children)
            {
                AddItemRecursive(child);
            }
        }

        public IAbstractItem Get(int id)
        {
            return _items[id];
        }

        public IAbstractItem GetStartPage(string host)
        {
            //тривиальная реализация
            foreach (var startPage in Root.Children.OfType<IStartPage>())
            {
                var bindings = startPage.GetDNSBindings();
                if (bindings.Contains(host) || bindings.Contains("*"))
                    return startPage;
            }

            return null;
        }
    }
}
