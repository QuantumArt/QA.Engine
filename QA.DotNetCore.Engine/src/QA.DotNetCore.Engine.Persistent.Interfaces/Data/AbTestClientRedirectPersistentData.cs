namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class AbTestClientRedirectPersistentData
    {
        public int Id { get; set; }

        public int ContainerId { get; set; }

        public int VersionNumber { get; set; }

        public string RedirectUrl { get; set; }
    }
}
