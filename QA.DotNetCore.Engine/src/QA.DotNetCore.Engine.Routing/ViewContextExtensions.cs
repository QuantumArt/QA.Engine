using Microsoft.AspNetCore.Mvc.Rendering;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing
{
    public static class ViewContextExtensions
    {
        public static IAbstractItem GetCurrentItem(this ViewContext context)
        {
            return context.ViewData[RoutingKeys.CurrentItem] as IAbstractItem;
        }

        public static T GetCurrentItem<T>(this ViewContext context)
            where T : class, IAbstractItem
        {
            return context.ViewData[RoutingKeys.CurrentItem] as T;
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
