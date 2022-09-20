using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public abstract class BaseTargetingFilter : ITargetingFilter
    {
        public abstract bool Match(IAbstractItem item);

        public virtual void Filter<T>(IList<T> items)
            where T : IAbstractItem
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (!Match(items[i]))
                {
                    items.RemoveAt(i);
                }
            }
        }

        public virtual IEnumerable<T> Pipe<T>(IEnumerable<T> items)
            where T : IAbstractItem
        {
            foreach (T item in items)
            {
                if (Match(item))
                {
                    yield return item;
                }
            }
        }

        public static BaseTargetingFilter operator &(BaseTargetingFilter f1, BaseTargetingFilter f2)
        {
            return new UnitedFilter(f1, f2);
        }

        public static BaseTargetingFilter operator |(BaseTargetingFilter f1, BaseTargetingFilter f2)
        {
            return new DelegateFilter(i => f1.Match(i) || f2.Match(i));
        }

        public static BaseTargetingFilter operator +(BaseTargetingFilter f1, BaseTargetingFilter f2)
        {
            return new UnitedFilter(f1, f2);
        }
    }

    public class DelegateFilter : BaseTargetingFilter
    {
        private readonly Func<IAbstractItem, bool> _isPositiveMatch;

        public DelegateFilter(Func<IAbstractItem, bool> isPositiveMatch)
        {
            _isPositiveMatch = isPositiveMatch ?? throw new ArgumentNullException("isPositiveMatch");
        }

        public override bool Match(IAbstractItem item)
        {
            return _isPositiveMatch(item);
        }
    }

    public class UnitedFilter : BaseTargetingFilter
    {
        private readonly ITargetingFilter[] filters;

        public UnitedFilter(params ITargetingFilter[] filters)
        {
            this.filters = filters ?? Array.Empty<ITargetingFilter>();
        }

        public UnitedFilter(IEnumerable<ITargetingFilter> filters)
        {
            this.filters = new List<ITargetingFilter>(filters).ToArray();
        }

        public override bool Match(IAbstractItem item)
        {
            foreach (var filter in filters)
                if (!filter.Match(item))
                    return false;
            return true;
        }

        public override IEnumerable<T> Pipe<T>(IEnumerable<T> items)
        {
            IEnumerable<T> filtered = items;
            foreach (var filter in filters)
            {
                var filteredArray = filter
                    .Pipe(filtered)
                    .ToArray();

                if (filteredArray.Length == 0)
                {
                    break;
                }

                filtered = filteredArray;
            }

            return filtered;
        }
    }

    public class NullFilter : BaseTargetingFilter
    {
        public override bool Match(IAbstractItem item)
        {
            return true;
        }
    }
}
