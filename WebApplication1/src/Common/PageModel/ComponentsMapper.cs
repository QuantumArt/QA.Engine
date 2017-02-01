using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public class ComponentsMapper
    {
        public string Map(AbstractItem page)
        {
            var name = page.GetType().Name;
            switch (name)
            {
                case "TextPart": return name;

                default:
                    break;
            }

            return null;
        }
    }
}
