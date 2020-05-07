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
                return Details.Keys.ToDictionary(k => k, k => Details.Get(k, typeof(object)));
            }
        }
    }
}
