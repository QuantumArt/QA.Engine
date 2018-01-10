using Microsoft.AspNetCore.Builder;

namespace QA.DotNetCore.Engine.OnScreen.Configuration
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
