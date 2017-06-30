using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing
{
    public static class MvcApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSiteSctructure(this IApplicationBuilder app, Action<IRouteBuilder> routes)
        {
            app.UseMiddleware<RoutingMiddleware>();
            app.UseMvc(routes);
            return app;
        }
    }
}
