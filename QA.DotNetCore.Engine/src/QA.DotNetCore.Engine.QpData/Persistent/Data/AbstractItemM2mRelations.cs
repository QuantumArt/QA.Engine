using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Persistent.Data
{
    public class AbstractItemM2mRelations
    {
        Dictionary<int, List<int>> _relations = new Dictionary<int, List<int>>();

        public void AddRelation(int relationId, int value)
        {
            if (!_relations.ContainsKey(relationId))
                _relations[relationId] = new List<int>();

            _relations[relationId].Add(value);
        }

        public IEnumerable<int> GetRelationValue(int relationId)
        {
            return _relations.ContainsKey(relationId) ? _relations[relationId] : new List<int>();
        }
    }
}
