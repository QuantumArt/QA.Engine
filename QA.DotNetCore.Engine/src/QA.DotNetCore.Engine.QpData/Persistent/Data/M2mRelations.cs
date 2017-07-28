using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Persistent.Data
{
    public class M2mRelations
    {
        Dictionary<int, HashSet<int>> _relations = new Dictionary<int, HashSet<int>>();

        public void AddRelation(int relationId, int value)
        {
            if (!_relations.ContainsKey(relationId))
                _relations[relationId] = new HashSet<int>();

            _relations[relationId].Add(value);
        }

        public IEnumerable<int> GetRelationValue(int relationId)
        {
            return _relations.ContainsKey(relationId) ? _relations[relationId] : new HashSet<int>();
        }

        public IEnumerable<int> GetRelations()
        {
            return _relations.Keys;
        }

        public void Merge(M2mRelations other)
        {
            foreach (var rel in other.GetRelations())
                foreach (var link in other.GetRelationValue(rel))
                    AddRelation(rel, link);
        }
    }
}
