using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingFilter
    {
        bool Match(IAbstractItem item);

        IEnumerable<T> Pipe<T>(IEnumerable<T> items) where T : IAbstractItem;

        void Filter<T>(IList<T> items) where T : IAbstractItem;
    }

    public static class TargetingFilterExtensions
    {
        public static IEnumerable<T> Pipe<T>(this IEnumerable<T> items, ITargetingFilter filter) where T : IAbstractItem
        {
            return filter.Pipe(items);
        }

        /// <summary>
        /// Создание нового фильтра из 2х
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static ITargetingFilter AddFilter(this ITargetingFilter f1, ITargetingFilter f2)
        {
            return new UnitedFilter(f1, f2);
        }

        /// <summary>
        /// Создание нового фильтра из переданных
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Новый фильтр || null</returns>
        public static ITargetingFilter Combine(this IEnumerable<ITargetingFilter> filters)
        {
            ITargetingFilter result = null;

            foreach (var filter in filters.Where(x => x != null))
            {
                if (result == null)
                {
                    result = filter;
                    continue;
                }

                result = result.AddFilter(filter);
            }

            return result;
        }
    }
}
