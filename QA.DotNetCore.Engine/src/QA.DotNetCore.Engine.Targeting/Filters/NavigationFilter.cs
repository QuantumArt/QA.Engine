using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class NavigationFilter : BaseTargetingFilter
    {
        public static readonly ITargetingFilter Default = new NavigationFilter();
        
        public override bool Match(IAbstractItem item)
        {
            return item.IsPage && (item is IAbstractPage && (item as IAbstractPage).IsVisible);
        }
    }
}
