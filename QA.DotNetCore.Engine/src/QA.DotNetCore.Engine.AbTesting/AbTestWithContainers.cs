using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.AbTesting
{
    public class AbTestWithContainers
    {
        public AbTestWithContainers()
        {
            ScriptContainers = new AbTestScriptContainerPersistentData[] { };
        }

        public AbTestPersistentData Test { get; set; }

        public AbTestClientRedirectContainerPersistentData ClientRedirectContainer { get; set; }

        public AbTestScriptContainerPersistentData[] ScriptContainers { get; set; }
    }
}
