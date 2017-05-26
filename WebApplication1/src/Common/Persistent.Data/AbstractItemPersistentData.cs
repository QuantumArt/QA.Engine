using System.Collections.Generic;
using System.Linq;

namespace Common.Persistent.Data
{
    public class AbstractItemPersistentData
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Alias { get; set; }

        public string Title { get; set; }

        public bool Visible { get; set; }

        public int Discriminator { get; set; }

    }
}
