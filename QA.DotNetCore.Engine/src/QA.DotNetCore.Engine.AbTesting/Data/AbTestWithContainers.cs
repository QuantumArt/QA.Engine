using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.AbTesting.Data
{
    public class AbTestWithContainers
    {
        public AbTestWithContainers()
        {
            ScriptContainers = new List<AbTestScriptContainerPersistentData>();
        }

        public AbTestPersistentData Test { get; set; }

        public AbTestClientRedirectContainerPersistentData ClientRedirectContainer { get; set; }

        public List<AbTestScriptContainerPersistentData> ScriptContainers { get; set; }
    }
}
