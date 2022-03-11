using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    public static class ControllerEndpointRouteBuilderExtensions
    {
        public static void MapSiteStructureControllerRoute(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDynamicControllerRoute<SiteStructureRouteValueTransformer>("{**wpfullpath}");
        }
    }
}
