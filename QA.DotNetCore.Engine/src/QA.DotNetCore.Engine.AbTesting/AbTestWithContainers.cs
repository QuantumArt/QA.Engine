using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.AbTesting
{
    public class AbTestWithContainers
    {
        public AbTestWithContainers()
        {
            Containers = new AbTestContainerPersistentData[] { };
        }

        public AbTestPersistentData Test { get; set; }

        public AbTestContainerPersistentData[] Containers { get; set; }
    }
}
