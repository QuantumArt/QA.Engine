using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public abstract class RelationFilter<TargetingItem> : BaseTargetingFilter
        where TargetingItem : IAbstractItem
    {
        private readonly ITargetingContext _context;
        /// <summary>
        /// Ключ таргетирования
        /// </summary>
        protected abstract string TargetingKey { get; }
        /// <summary>
        /// Фильтрация приметяется только к тем AbstractItem, у которых заполнено таргетинг поле
        /// </summary>
        protected abstract bool MatchIfNoRelation { get; }
        /// <summary>
        /// Фильтрация приметяется только к AbstractItem, которые являются TargetingItem
        /// </summary>
        protected abstract bool MatchIfNoTargetingType { get; }
        protected abstract bool Match(TargetingItem targetingItem, int[] targetingIds);

        public RelationFilter(ITargetingContext context)
        {
            _context = context;
        }

        public override bool Match(IAbstractItem item)
        {
            var targetingValue = _context.GetTargetingValue(TargetingKey);
            var targetingIds = GetIds(targetingValue);

            if (!targetingIds.Any())
            {
                return true;
            }

            if (item is TargetingItem targetingItem)
            {
                return Match(targetingItem, targetingIds);
            }

            return MatchIfNoTargetingType;
        }

        private int[] GetIds(object value) => value == null || value is not string ? new int[0] : ((string)value)
            .Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(item => int.TryParse(item, out int id) ? id : 0)
            .Where(id => id > 0)
            .Distinct()
            .ToArray();
    }
}
