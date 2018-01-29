using System;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class QpContentModificationPersistentData
    {
        public int ContentId { get; set; }
        public string ContentName { get; set; }
        public DateTime LiveModified { get; set; }
        public DateTime StageModified { get; set; }
    }
}
