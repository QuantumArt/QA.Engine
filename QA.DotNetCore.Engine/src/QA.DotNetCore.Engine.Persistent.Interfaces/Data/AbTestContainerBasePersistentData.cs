using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public abstract class AbTestContainerBasePersistentData
    {
        public int Id { get; set; }

        //public AbTestPersistentData Test { get; set; }

        public abstract AbTestContainerType Type { get; }

        public string[] AllowedUrlPatterns { get; set; }

        public string[] DeniedUrlPatterns { get; set; }

        public string Precondition { get; set; }
    }

    public enum AbTestContainerType
    {
        Script,
        ClientRedirect
    }
}
