using Microsoft.AspNetCore.Http;

namespace QA.DotNetCore.Engine.Widgets.OnScreen
{
    public interface IOnScreenContextProvider
    {
        OnScreenContext GetContext(HttpContext httpContext);
    }
}
