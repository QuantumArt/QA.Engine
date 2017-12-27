using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.OnScreen;

namespace QA.DotNetCore.Engine.OnScreen
{
    public class OnScreenModeMiddleware
    {
        private readonly RequestDelegate _next;

        public OnScreenModeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, IOnScreenContextProvider contextProvider)
        {
            //установим для запроса контекст OnScreen
            contextProvider.SetContext();

            // Call the next delegate/middleware in the pipeline
            return _next(httpContext);
        }
    }
}
