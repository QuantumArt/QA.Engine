using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class NavigationFilter : BaseTargetingFilter
    {
        public static readonly ITargetingFilter Default = new NavigationFilter();
        
        public override bool Match(IAbstractItem item)
        {
            return item.IsPage;
            // TODO: use Display in nagigation flag (IsVisible)
        }
    }
}
