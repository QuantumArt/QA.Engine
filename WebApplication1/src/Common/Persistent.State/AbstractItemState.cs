using System.Collections.Generic;
using System.Linq;

namespace Common.Persistent.State
{
    public class AbstractItemState
    {

        public AbstractItemState()
        {
            Children = Enumerable.Empty<AbstractItemState>();
        }

        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Title { get; set; }

        public bool Visible { get; set; }

        public IEnumerable<AbstractItemState> Children { get; set; }

    }
}
