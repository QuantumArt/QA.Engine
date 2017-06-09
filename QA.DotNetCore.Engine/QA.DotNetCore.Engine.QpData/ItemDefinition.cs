using System;

namespace QA.DotNetCore.Engine.QpData
{
    public class ItemDefinition
    {
        public int Id { get; set; }

        public string Discriminator { get; set; }

        public string TypeName { get; set; }

        public Type Type { get; set; }
    }
}
