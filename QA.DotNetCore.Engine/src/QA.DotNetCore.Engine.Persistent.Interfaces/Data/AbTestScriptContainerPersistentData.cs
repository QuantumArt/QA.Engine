using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class AbTestScriptContainerPersistentData : AbTestContainerBasePersistentData
    {
        public AbTestScriptContainerPersistentData()
        {
            Scripts = new List<AbTestScriptPersistentData>();
        }

        public override AbTestContainerType Type => AbTestContainerType.Script;

        public List<AbTestScriptPersistentData> Scripts { get; set; }
    }
}
