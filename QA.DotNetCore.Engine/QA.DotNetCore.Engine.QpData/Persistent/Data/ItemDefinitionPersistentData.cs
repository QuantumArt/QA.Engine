using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.QpData.Persistent.Data
{
    public class ItemDefinitionPersistentData
    {
        public int Id { get; set; }

        public string Discriminator { get; set; }

        public string TypeName { get; set; }
    }
}
