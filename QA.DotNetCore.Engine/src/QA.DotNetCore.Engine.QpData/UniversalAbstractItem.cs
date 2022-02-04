using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
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

        internal override void MapPersistent(AbstractItemPersistentData persistentItem)
        {
            IsPage = persistentItem.IsPage;
            base.MapPersistent(persistentItem);
        }

        public string Type { get; private set; }

        public Dictionary<string, object> UntypedFields
        {
            get
            {
                if (LazyDetails?.Value == null)
                    return new Dictionary<string, object>();

                return LazyDetails.Value.Keys.ToDictionary(fieldName => fieldName,
                    fieldName => GetUntypedDetail(fieldName));
            }
        }

        public ICollection<IAbstractItem> ChildItems { get { return Children; } }

        private object GetUntypedDetail(string fieldName)
        {
            if (M2mFieldNames.Any(fn => fn.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)))
                return GetRelationIds(fieldName);

            return LazyDetails?.Value?.Get(fieldName, typeof(object));
        }
    }
}
