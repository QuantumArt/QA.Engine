using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.QpData
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
