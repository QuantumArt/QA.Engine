namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class AbTestScriptPersistentData
    {
        public int ContainerId { get; set; }

        public int VersionNumber { get; set; }

        public string ScriptText { get; set; }

        public string Description { get; set; }
    }
}
