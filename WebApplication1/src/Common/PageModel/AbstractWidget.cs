using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public abstract class AbstractWidget : AbstractItem
    {
        public string ZoneName { get; set; }

        public AbstractWidget()
        {

        }
        public AbstractWidget(int id, string alias, string title, string zoneName, params AbstractItem[] children)
            : base(id, alias, title, children)
        {
            ZoneName = zoneName;
        }
    }
}
