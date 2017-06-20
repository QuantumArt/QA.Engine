using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Abstractions
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
            foreach (var child in item.GetChildren())
            {
                AddItemRecursive(child);
            }
        }

        public IAbstractItem Get(int id)
        {
            return _items[id];
        }

        public IAbstractItem GetStartPage(string host, ITargetingFilter filter = null)
        {
            //тривиальная реализация
            foreach (var startPage in Root.GetChildren(filter).OfType<IStartPage>())
            {
                var bindings = startPage.GetDNSBindings();
                if (bindings.Contains(host) || bindings.Contains("*"))
                    return startPage;
            }

            return null;
        }
    }
}
