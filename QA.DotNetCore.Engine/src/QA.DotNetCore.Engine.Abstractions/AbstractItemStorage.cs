using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.Abstractions
{
    public class AbstractItemStorage
    {
        private Dictionary<int, IAbstractItem> _items = new Dictionary<int, IAbstractItem>();
        private IServiceProvider ServiceProvider { get; }

        public IAbstractItem Root { get; }
        public ITargetingUrlResolver UrlResolver { get { return ServiceProvider.GetService<ITargetingUrlResolver>(); }  }

        public AbstractItemStorage(IAbstractItem root, IServiceProvider serviceProvider)
        {
            Root = root;
            ServiceProvider = serviceProvider;
            AddItemRecursive(root);
        }

        private void AddItemRecursive(IAbstractItem item)
        {
            item.Storage = this;
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

        public virtual IAbstractItem GetStartPage(string host, ITargetingFilter filter = null)
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
