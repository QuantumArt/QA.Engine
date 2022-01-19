using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using QA.DotNetCore.Engine.Abstractions.Wildcard;

namespace QA.DotNetCore.Engine.Abstractions
{
    public sealed class AbstractItemStorage
    {
        private readonly Dictionary<int, IAbstractItem> _items = new Dictionary<int, IAbstractItem>();

        private readonly Dictionary<string, IStartPage> _startPageByDnsPatternMappings =
            new Dictionary<string, IStartPage>();

        public IAbstractItem Root { get; }

        public AbstractItemStorage(IAbstractItem root)
        {
            Root = root;
            AddItemRecursive(root);

            foreach (var startPage in Root.GetChildren().OfType<IStartPage>())
            {
                var dns = startPage.GetDNSBindings();
                Array.ForEach(dns, x => _startPageByDnsPatternMappings[x] = startPage);
            }
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
            return _items.ContainsKey(id) ? _items[id] : null;
        }

        public TAbstractItem Get<TAbstractItem>(int id) where TAbstractItem : class, IAbstractItem
        {
            return _items.ContainsKey(id) ? _items[id] as TAbstractItem : null;
        }

        public IAbstractItem GetStartPage(string host, ITargetingFilter filter = null)
        {
            var bindings = new List<string>();

            foreach (var startPage in Root.GetChildren(filter).OfType<IStartPage>())
            {
                var dns = startPage.GetDNSBindings();
                bindings.AddRange(dns);
            }

            var matcher = new WildcardMatcher(WildcardMatchingOption.FullMatch, bindings);
            var pattern = matcher.MatchLongest(host);
            return pattern != null ? _startPageByDnsPatternMappings[pattern] : null;
        }

        public TAbstractItem GetStartPage<TAbstractItem>(string host, ITargetingFilter filter = null) where TAbstractItem : class, IAbstractItem
        {
            var startPage = GetStartPage(host, filter);
            return startPage != null ?
                startPage as TAbstractItem :
                null;
        }
    }
}
