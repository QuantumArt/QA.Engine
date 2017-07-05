using Microsoft.AspNetCore.Builder;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    public static class MvcApplicationBuilderExtensions
    {
        /// <summary>
        /// Добавляем работу со структурой сайта в pipeline. Должно вызываться до UseMvc.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSiteSctructure(this IApplicationBuilder app)
        {
            app.UseMiddleware<RoutingMiddleware>();
            return app;
        }
    }
}
