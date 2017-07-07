using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.QpData.Persistent.Data
{
    /// <summary>
    /// Контент QP. (Таблица CONTENT)
    /// </summary>
    public class ContentPersistentData
    {
        public string ContentNetName { get; set; }

        public int ContentId { get; set; }

        public string StageTableName { get { return $"CONTENT_{ContentId}_STAGE_NEW"; } }

        public string LiveTableName { get { return $"CONTENT_{ContentId}_LIVE_NEW"; } }

        public IEnumerable<ContentAttributePersistentData> ContentAttributes { get; set; }
    }
}
