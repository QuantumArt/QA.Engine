using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QA.DotNetCore.Engine.Widgets.OnScreen
{
    public class OnScreenModeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOnScreenContextProvider _contextProvider;
        


        public OnScreenModeMiddleware(RequestDelegate next, IOnScreenContextProvider contextProvider)
        {
            _next = next;
            _contextProvider = contextProvider;
        }

        public Task Invoke(HttpContext context)
        {
            var onScreenContext = _contextProvider.GetContext(context);
            if (!context.Items.ContainsKey(OnScreenModeKeys.OnScreenContext))
                context.Items.Add(OnScreenModeKeys.OnScreenContext, onScreenContext);
            else
                context.Items[OnScreenModeKeys.OnScreenContext] = onScreenContext;
            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
