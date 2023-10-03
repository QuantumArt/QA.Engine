using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public abstract class ManyToManyFilter<TargetingItem> : RelationFilter<TargetingItem>
        where TargetingItem : IAbstractItem
    {
        /// <summary>
        /// Предикат выбора ManyToMany поля, по которму осуществляется таргетинг
        /// </summary>
        /// <param name="item">AbstractItem у которого выбираем ManyToMany поле</param>
        /// <returns>Идентификаторы статей с которыми связано поле</returns>
        protected abstract IEnumerable<int> GetRelationIds(TargetingItem item);

        public ManyToManyFilter(ITargetingContext context) : base(context)
        {
        }

        protected override bool Match(TargetingItem targetingItem, int[] targetingIds)
        {
            var relationIds = GetRelationIds(targetingItem);
            return MatchIfNoRelation && !relationIds.Any() || relationIds.Intersect(targetingIds).Any();
        }
    }
}
