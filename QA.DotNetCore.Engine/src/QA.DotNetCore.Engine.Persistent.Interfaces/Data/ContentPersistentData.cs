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
        public ContentPersistentData()
        {
            UseDefaultFiltration = true;
        }

        public string ContentNetName { get; set; }

        public int ContentId { get; set; }

        public string ContentName { get; set; }

        public string StageTableName => UseDefaultFiltration ? $"CONTENT_{ContentId}_STAGE_NEW" : "_united";

        public string LiveTableName => UseDefaultFiltration ? $"CONTENT_{ContentId}_LIVE_NEW" : "";

        public bool UseDefaultFiltration { private get; set; }

        public IEnumerable<ContentAttributePersistentData> ContentAttributes { get; set; }
    }
}
