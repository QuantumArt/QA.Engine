using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class AbTestClientRedirectContainerPersistentData : AbTestContainerBasePersistentData
    {
        public override AbTestContainerType Type => AbTestContainerType.ClientRedirect;

        public List<AbTestClientRedirectPersistentData> Redirects { get; set; }
    }
}
