using QA.DotNetCore.Engine.Abstractions.Finder.Algorithms;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Linq;

namespace QA.DotNetCore.Engine.Abstractions.Finder
{
    public class ItemFinder : IItemFinder
    {
        public ItemFinder(ITargetingFilterAccessor targetingFilterAccessor)
        {
            TargetingFilterAccessor = targetingFilterAccessor;
        }

        public ITargetingFilterAccessor TargetingFilterAccessor { get; }

        /// <summary>
        /// Поиск элементов по условию. Применяется рекурсивный поиск по поддереву начальной страницы
        /// </summary>
        /// <param name="startPage">начальная страница для поиска</param>
        /// <param name="matchCriteria">условие поиска</param>
        /// <param name="depth">максимальная глубина поиска</param>
        /// <returns></returns>
        public IAbstractItem Find(IAbstractItem startPage,
            Func<IAbstractItem, bool> matchCriteria,
            int depth = 5)
        {
            var filter = TargetingFilterAccessor.Get() ?? new NullFilter();

            var alg = new RecursiveVisitorAlgorithm<object>((_, __) => null, filter, new DelegateFilter(matchCriteria))
            {
                IsFlatteringMode = true,
                Depth = depth
            };

            var result = alg.Run(startPage).Select(x => x.Item);

            return result.FirstOrDefault();
        }

        public TAbstractItem Find<TAbstractItem>(IAbstractItem startPage, Func<TAbstractItem, bool> matchCriteria, int depth = 5) where TAbstractItem : IAbstractItem
        {
            return (TAbstractItem)Find(startPage, ai => ai is TAbstractItem && matchCriteria((TAbstractItem)ai), depth);
        }
    }
}
