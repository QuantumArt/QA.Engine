using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QA.DotNetCore.Engine.Abstractions.Finder.Algorithms
{
    /// <summary>
    /// Алгоритм рекурсивного обхода
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class RecursiveVisitorAlgorithm<V> : IFlatAlgorithm<V>
    {
        private readonly Func<IAbstractItem, int, V> _factory;
        private readonly ITargetingFilter _filter;
        private readonly ITargetingFilter _matchFilter;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="factory">фабрика дополнительных параметров</param>
        /// <param name="filter">общий фильтр</param>
        /// <param name="matchFilter">фильтр для поискового условия</param>
        public RecursiveVisitorAlgorithm(
            Func<IAbstractItem, int, V> factory,
            ITargetingFilter filter,
            ITargetingFilter matchFilter)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _filter = filter;
            _matchFilter = matchFilter;
        }

        /// <summary>
        /// Макс. глубина рекурсии
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Преобразовывать иерархию в плоский список
        /// </summary>
        public bool IsFlatteringMode { get; set; }

        /// <summary>
        /// Порядок сортировки.
        /// </summary>
        public bool IsReverseOrder { get; set; }

        /// <summary>
        /// Запуск алгоритма
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public IReadOnlyList<TreeNode<IAbstractItem, V>> Run(IAbstractItem root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            if (Depth <= 0)
                throw new ArgumentException("The value of depth should be greater than 0.", nameof(Depth));

            return ProcessRecursive(root, Depth, 0)
                .OrderBy(x => x.Item.SortOrder)
                .ToList();
        }

        protected IEnumerable<TreeNode<IAbstractItem, V>> ProcessRecursive(IAbstractItem item, int maxDepth, int currentDepth)
        {
            if (currentDepth <= maxDepth)
            {
                foreach (var child in item.GetChildren(_filter))
                {
                    var ch = TreeNode<IAbstractItem, V>
                        .CreateNode(child, _factory(child, currentDepth));

                    var isMatch = _matchFilter.Match(child);

                    if (IsFlatteringMode)
                    {
                        if (!IsReverseOrder && isMatch)
                            yield return ch;

                        foreach (var subChild in ProcessRecursive(child, maxDepth, currentDepth + 1)
                            .OrderBy(x => x.Item.SortOrder))
                        {
                            yield return subChild;
                        }

                        if (IsReverseOrder && isMatch)
                            yield return ch;
                    }
                    else
                    {
                        yield return ch.AppendRange(
                            ProcessRecursive(ch.Item, maxDepth, currentDepth + 1)
                            .OrderBy(x => x.Item.SortOrder)
                            .ToArray());
                    }
                }
            }
        }
    }
}
