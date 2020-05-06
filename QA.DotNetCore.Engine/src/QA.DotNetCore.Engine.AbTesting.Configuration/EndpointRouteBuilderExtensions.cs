using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace QA.DotNetCore.Engine.AbTesting.Configuration
{
    public static class EndpointRouteBuilderExtensions
    {
#if NETCOREAPP3_1
        public static void MapAbtestEndpointRoute(this IEndpointRouteBuilder endpoints, string pattern = "/abtest/inlinescript")
        {
            endpoints.MapControllerRoute("widget platform abtesting", pattern, defaults: new { controller = "AbTest", action = "InlineScript" });
        }
#endif
    }
}
