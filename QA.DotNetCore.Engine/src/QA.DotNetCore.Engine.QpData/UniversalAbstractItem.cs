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

        public IReadOnlyDictionary<string, object> UntypedFields
        {
            get
            {
                if (LazyDetails?.Value is null)
                {
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }

                Dictionary<string, object> details = new(LazyDetails.Value.Count);

                foreach (var fieldName in LazyDetails.Value.Keys)
                {
                    var fieldValue = GetUntypedDetail(fieldName);
                    if (fieldValue is not null)
                    {
                        details.Add(fieldName, fieldValue);
                    }
                }

                return details;
            }
        }

        public ICollection<IAbstractItem> ChildItems { get { return Children; } }

        private object GetUntypedDetail(string fieldName)
        {
            if (M2mFieldNames.Any(fn => fn.Equals(fieldName, StringComparison.OrdinalIgnoreCase)))
                return GetRelationIds(fieldName);

            return LazyDetails?.Value?.Get(fieldName, typeof(object));
        }
    }
}
