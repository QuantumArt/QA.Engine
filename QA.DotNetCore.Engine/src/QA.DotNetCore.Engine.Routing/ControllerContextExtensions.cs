using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing
{
    public static class ControllerContextExtensions
    {
        public static IAbstractItem GetCurrentItem(this ControllerContext context)
        {
            return context.RouteData.DataTokens[RoutingKeys.CurrentItem] as IAbstractItem;
        }

        public static T GetCurrentItem<T>(this ControllerContext context)
            where T : class, IAbstractItem
        {
            return context.RouteData.DataTokens[RoutingKeys.CurrentItem] as T;
        }

        public static IStartPage GetStartPage(this ControllerContext context)
        {
            return context.HttpContext.GetStartPage();
        }

        public static T GetStartPage<T>(this ControllerContext context)
            where T : class, IStartPage
        {
            return context.HttpContext.GetStartPage<T>();
        }
    }

    
}
