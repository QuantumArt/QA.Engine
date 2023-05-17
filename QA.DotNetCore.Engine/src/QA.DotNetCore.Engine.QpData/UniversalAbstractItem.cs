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

        public IReadOnlyDictionary<string, object> GetUntypedFields()
        {
                VerifyDetailsLoaded();
                
                if (Details == null)
                {
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }

                Dictionary<string, object> details = new(Details.Count);

                foreach (var fieldName in Details.Keys)
                {
                    var fieldValue = GetUntypedDetail(fieldName);
                    if (fieldValue is not null)
                    {
                        details.Add(fieldName, fieldValue);
                    }
                }

                return details;

        }

        public ICollection<IAbstractItem> ChildItems { get { return Children; } }

        private object GetUntypedDetail(string fieldName) =>
            M2MFieldNameMapToLinkIds.ContainsKey(fieldName.ToLowerInvariant()) ? 
                GetRelationIds(fieldName) : 
                GetDetail<object>(fieldName, null);
    }
}
