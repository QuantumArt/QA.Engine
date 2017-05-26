using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public class TextPage : AbstractItem
    {
        public TextPage():base()
        {

        }
        public TextPage(int id, string alias, string title, params AbstractItem[] children):base(id, alias, title, children)
        {
        }

        public string Text { get; set; }
    }
}
