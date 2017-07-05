using System.Collections.Generic;

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
    }
}
