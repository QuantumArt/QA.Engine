using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.PageModel;

namespace Common.Widgets
{
    public class TextPart : AbstractWidget
    {
        public TextPart() : base() { }

        public TextPart(int id, string alias, string title, string zoneName, params AbstractItem[] children)
            : base(id, alias, title, zoneName, children) { }

        public string Text { get; set; }
    }
}
