using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    /// <summary>
    /// Фильтр по relation полю
    /// </summary>
    public class RelationFilter : BaseTargetingFilter
    {
        private readonly Func<AbstractItem, IEnumerable<int>> _getRelationIds;
        private readonly ICollection<int> _ids;
        private readonly ILogger _logger;

        public RelationFilter(Func<AbstractItem, IEnumerable<int>> getRelationIds, ICollection<int> ids, ILogger logger)
        {
            _getRelationIds = getRelationIds;
            _ids = ids;
            _logger = logger;
        }

        public override bool Match(IAbstractItem item)
        {
            if (_ids.Count == 0)
            {
                return true;
            }

            if (item is not UniversalAbstractItem uai)
            {
                _logger.LogWarning(
                    "Фильтр не поддерживает AbstractItem данного типа ({ItemType}). Следует использовать " + nameof(UniversalAbstractItemFactory),
                    item.GetType().Name);
                return false;
            }

            var _relationIds = _getRelationIds(uai);
            return !_relationIds.Any() || _relationIds.Any(_ids.Contains);
        }
    }
}
