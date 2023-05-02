using QA.DotNetCore.Engine.Persistent.Interfaces.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    [Serializable]
    public class M2MRelations : ISerializable
    {
        private readonly Dictionary<int, HashSet<int>> _relations = new Dictionary<int, HashSet<int>>();

        public M2MRelations()
        {
        }

        protected M2MRelations(SerializationInfo info, StreamingContext context)
        {
            _relations = info.GetValue<Dictionary<int, HashSet<int>>>(nameof(_relations));
        }

        public void AddRelation(int relationId, int value)
        {
            if (!_relations.ContainsKey(relationId))
            {
                _relations[relationId] = new HashSet<int>();
            }

            _ = _relations[relationId].Add(value);
        }

        public IEnumerable<int> GetRelationValue(int relationId)
        {
            return _relations.ContainsKey(relationId) ? _relations[relationId] : new HashSet<int>();
        }

        public IEnumerable<int> GetRelations()
        {
            return _relations.Keys;
        }

        public void Merge(M2MRelations other)
        {
            foreach (var rel in other.GetRelations())
            {
                foreach (var link in other.GetRelationValue(rel))
                {
                    AddRelation(rel, link);
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(_relations), _relations);
        }
    }
}
