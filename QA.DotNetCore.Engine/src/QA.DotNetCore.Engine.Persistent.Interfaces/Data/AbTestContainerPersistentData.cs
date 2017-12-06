using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class AbTestContainerPersistentData
    {
        public int Id { get; set; }

        //public AbTestPersistentData Test { get; set; }

        //public AbTestContainerType Type { get; set; }

        public string[] AllowedUrlPatterns { get; set; }

        public string[] DeniedUrlPatterns { get; set; }

        public string Precondition { get; set; }

        public List<AbTestScriptPersistentData> Scripts { get; set; }
    }

    //public enum AbTestContainerType
    //{
    //    Script,
    //    PageRewrite
    //}
}
