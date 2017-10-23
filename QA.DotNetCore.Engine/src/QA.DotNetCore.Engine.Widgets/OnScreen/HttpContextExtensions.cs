using Microsoft.AspNetCore.Http;

namespace QA.DotNetCore.Engine.Widgets.OnScreen
{
    public static class HttpContextExtensions
    {
        public static bool OnScreenEditEnabled(this HttpContext httpContext)
        {
            if (!httpContext.Items.ContainsKey(OnScreenModeKeys.OnScreenContext))
                return false;
            if (!(httpContext.Items[OnScreenModeKeys.OnScreenContext] is OnScreenContext context))
                return false;
            return context.IsAuthorised && context.IsEditMode;
        }
    }
}
