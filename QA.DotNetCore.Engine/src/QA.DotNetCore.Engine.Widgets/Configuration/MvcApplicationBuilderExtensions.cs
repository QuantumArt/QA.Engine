using Microsoft.AspNetCore.Builder;
using QA.DotNetCore.Engine.Widgets.OnScreen;

namespace QA.DotNetCore.Engine.Widgets.Configuration
{
    public static class MvcApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseOnScreenMode(this IApplicationBuilder app)
        {
            app.UseMiddleware<OnScreenModeMiddleware>();
            return app;
        }
    }
}
