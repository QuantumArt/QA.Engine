using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Abstractions.Wildcard;
using System;
using System.Collections.Generic;
using System.Linq;

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
            SetStartPages();
        }

        public AbstractItemStorage(IAbstractItem root, IEnumerable<IAbstractItem> abstractItems)
        {
            Root = root;
            AddItems(abstractItems);
            SetStartPages();
        }

        private void SetStartPages()
        {
            foreach (var startPage in Root.GetChildren().OfType<IStartPage>())
            {
                var dns = startPage.GetDNSBindings();
                Array.ForEach(dns, x => _startPageByDnsPatternMappings[x] = startPage);
            }
        }

        /// <summary>
        /// Создание элементов в словаре
        /// </summary>
        /// <param name="abstractItems"></param>
        private void AddItems(IEnumerable<IAbstractItem> abstractItems)
        {
            foreach (var item in abstractItems)
            {
                _items[item.Id] = item;
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
            var pattern = GetBindingPattern(host, filter);
            return pattern != null ? _startPageByDnsPatternMappings[pattern] : null;
        }

        /// <summary>
        /// Получить коллекцию нод по фильтрам
        /// </summary>
        /// <param name="host">Фильтр домена</param>
        /// <param name="startPageFilter">Фильтр определения стартовой страницы</param>
        /// <param name="nodeFilter">Фильтр для плоской структуры нод</param>
        /// <returns>Коллекция нод || null</returns>
        public T[] GetNodes<T>(string host, ITargetingFilter startPageFilter = null,
            ITargetingFilter nodeFilter = null)
            where T : class, IAbstractItem
        {
            var pattern = GetBindingPattern(host, startPageFilter);
            var startPage = pattern != null ? _startPageByDnsPatternMappings[pattern] : null;

            if (startPage == null)
                return null;

            var flatten = Flatten<T>(startPage);
            return flatten.Pipe(nodeFilter).ToArray();
        }

        private static IEnumerable<T> Flatten<T>(IAbstractItem item)
            where T : class, IAbstractItem
        {
            if (item is T targetItem)
                yield return targetItem;

            var children = item.GetChildren();

            if (children == null)
                yield break;

            foreach (var child in children)
            {
                foreach (var childItem in Flatten<T>(child))
                {
                    yield return childItem;
                }
            }
        }

        public TAbstractItem GetStartPage<TAbstractItem>(string host, ITargetingFilter filter = null) where TAbstractItem : class, IAbstractItem
        {
            var startPage = GetStartPage(host, filter);
            return startPage != null ?
                startPage as TAbstractItem :
                null;
        }

        private string GetBindingPattern(string host, ITargetingFilter filter)
        {
            var bindings = Root
                .GetChildren(filter)
                .OfType<IStartPage>()
                .SelectMany(startPage => startPage.GetDNSBindings());

            var matcher = new WildcardMatcher(WildcardMatchingOption.FullMatch, bindings);
            return matcher.MatchLongest(host);
        }
    }
}
