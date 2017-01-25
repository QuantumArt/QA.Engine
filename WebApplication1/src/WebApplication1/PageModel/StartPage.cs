using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.PageModel
{
    public class StartPage : TextPage, IStartPage
    {
        public StartPage(int id, string alias, string title, params AbstractItem[] children):base(id, alias,title, children)
        {
        }

    }
}
