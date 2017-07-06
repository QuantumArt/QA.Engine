using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing
{
    public static class HttpContextExtensions
    {
        public static IStartPage GetStartPage(this HttpContext context)
        {
            return context.Items[RoutingKeys.StartPage] as IStartPage;
        }

        public static T GetStartPage<T>(this HttpContext context)
            where T : class, IStartPage
        {
            return context.Items[RoutingKeys.StartPage] as T;
        }
    }
}
