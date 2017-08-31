using Microsoft.AspNetCore.Mvc.Rendering;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing
{
    public static class ViewContextExtensions
    {
        public static IAbstractItem GetCurrentItem(this ViewContext context)
        {
            var item = context.ViewData[RoutingKeys.CurrentItem] as IAbstractItem;
            if (item == null)
            {
                item = context.RouteData.DataTokens[RoutingKeys.CurrentItem] as IAbstractItem;
            }
            return item;
        }

        public static T GetCurrentItem<T>(this ViewContext context)
            where T : class, IAbstractItem
        {
            var item = context.ViewData[RoutingKeys.CurrentItem] as T;
            if (item == null)
            {
                item = context.RouteData.DataTokens[RoutingKeys.CurrentItem] as T;
            }
            return item;
        }

        public static IStartPage GetStartPage(this ViewContext context)
        {
            return context.HttpContext.GetStartPage();
        }

        public static T GetStartPage<T>(this ViewContext context)
            where T : class, IStartPage
        {
            return context.HttpContext.GetStartPage<T>();
        }
    }
}
