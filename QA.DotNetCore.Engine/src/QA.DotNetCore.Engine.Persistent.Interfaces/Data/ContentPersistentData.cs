using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    /// <summary>
    /// Контент QP. (Таблица CONTENT)
    /// </summary>
    public class ContentPersistentData
    {
        public string ContentNetName { get; set; }

        public int ContentId { get; set; }

        public string ContentName { get; set; }

        public string StageTableName => $"CONTENT_{ContentId}_STAGE_NEW";
        public string LiveTableName => $"CONTENT_{ContentId}_LIVE_NEW";
        public string UnitedTableName => $"CONTENT_{ContentId}_united";
        public IEnumerable<ContentAttributePersistentData> ContentAttributes { get; set; }
    }
}
