namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public abstract class AbTestContainerBasePersistentData
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        public abstract AbTestContainerType Type { get; }

        public string Description { get; set; }

        public string AllowedUrlPatternsStr { get; set; }

        public string DeniedUrlPatternsStr { get; set; }

        public string[] AllowedUrlPatterns { get { return AllowedUrlPatternsStr != null ? AllowedUrlPatternsStr.Split('\n') : new string[0]; } }

        public string[] DeniedUrlPatterns { get { return DeniedUrlPatternsStr != null ? DeniedUrlPatternsStr.Split('\n') : new string[0]; } }

        public string Domain { get; set; }

        public string Precondition { get; set; }

        public string Arguments { get; set; }
    }

    public enum AbTestContainerType
    {
        Script,
        ClientRedirect
    }
}
