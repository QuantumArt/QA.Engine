using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    /// <summary>
    /// Контент QP. (Таблица CONTENT)
    /// </summary>
    public class ContentPersistentData
    {
        public string ContentNetName { get; set; }

        public int ContentId { get; set; }

        public int SiteId { get; set; }

        public string ContentName { get; set; }

        public IEnumerable<ContentAttributePersistentData> ContentAttributes { get; set; }
    }
}
