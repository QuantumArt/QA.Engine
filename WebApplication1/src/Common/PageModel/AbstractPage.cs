using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public abstract class AbstractPage : AbstractItem
    {
        public override bool IsPage
        {
            get
            {
                return true;
            }
        }
    }
}
