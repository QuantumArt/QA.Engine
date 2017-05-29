using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public class ControllerMapper
    {
        public string Map(IAbstractItem page)
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
