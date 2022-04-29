using QA.DotNetCore.Engine.Persistent.Interfaces.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    [Serializable]
    public class M2mRelations : ISerializable
    {
        private readonly Dictionary<int, HashSet<int>> _relations = new Dictionary<int, HashSet<int>>();

        public M2mRelations()
        {
        }

        protected M2mRelations(SerializationInfo info, StreamingContext context)
        {
            _relations = info.GetValue(() => _relations);
        }

        public void AddRelation(int relationId, int value)
        {
            if (!_relations.ContainsKey(relationId))
                _relations[relationId] = new HashSet<int>();

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

        public void Merge(M2mRelations other)
        {
            foreach (var rel in other.GetRelations())
                foreach (var link in other.GetRelationValue(rel))
                    AddRelation(rel, link);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(() => _relations);
        }
    }
}
