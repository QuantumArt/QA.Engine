using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public abstract class OneToManyFilter<TargetingItem> : RelationFilter<TargetingItem>
        where TargetingItem : IAbstractItem
    {
        /// <summary>
        /// Предикат выбора OneToMany поля, по которму осуществляется таргетинг
        /// </summary>
        /// <param name="item">AbstractItem у которого выбираем OneToMany поле</param>
        /// <returns>Идентификатор статьи на которое ссылается поле</returns>
        protected abstract int? GetRelationId(TargetingItem item);

        public OneToManyFilter(ITargetingContext context) : base(context)
        {
        }

        protected override bool Match(TargetingItem targetingItem, int[] targetingIds)
        {
            var relationId = GetRelationId(targetingItem);
            return MatchIfNoRelation && !relationId.HasValue || relationId.HasValue && targetingIds.Contains(relationId.Value);
        }
    }
}
