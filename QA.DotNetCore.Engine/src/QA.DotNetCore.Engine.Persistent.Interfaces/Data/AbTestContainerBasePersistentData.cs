using System;

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

        public string[] AllowedUrlPatterns =>
            AllowedUrlPatternsStr != null ? AllowedUrlPatternsStr.Split('\n') : Array.Empty<string>();

        public string[] DeniedUrlPatterns =>
            DeniedUrlPatternsStr != null ? DeniedUrlPatternsStr.Split('\n') : Array.Empty<string>();

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
