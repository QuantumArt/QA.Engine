using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    public static class ControllerEndpointRouteBuilderExtensions
    {
#if NETCOREAPP3_1_OR_GREATER
        public static void MapSiteStructureControllerRoute(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDynamicControllerRoute<SiteStructureRouteValueTransformer>("{**wpfullpath}");
        }
#endif
    }
}
