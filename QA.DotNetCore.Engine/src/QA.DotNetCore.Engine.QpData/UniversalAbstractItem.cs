using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    public class UniversalAbstractItem : AbstractItem
    {
        public UniversalAbstractItem(string discriminator) : base()
        {
            Type = discriminator;
        }

        public string Type { get; private set; }

        public Dictionary<string, object> UntypedFields
        {
            get
            {
                return Details.Keys.ToDictionary(fieldName => fieldName, fieldName => GetUntypedDetail(fieldName));
            }
        }

        public ICollection<IAbstractItem> ChildItems { get { return Children; } }

        private object GetUntypedDetail(string fieldName)
        {
            if (M2mFieldNames.Any(fn => fn.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)))
                return GetRelationIds(fieldName);
            return Details.Get(fieldName, typeof(object));
        }
    }
}
