using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.PageModel
{
    public class ControllerMapper
    {
        public string Map(AbstractItem page)
        {
            var name = page.GetType().Name;
            switch (name)
            {
                case "StartPage": return name;
                case "TextPage": return name;

                default:
                    break;
            }

            return null;
        }
    }
}
