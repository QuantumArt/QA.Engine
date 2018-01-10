using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.OnScreen;

namespace QA.DotNetCore.Engine.OnScreen
{
    public class OnScreenHttpContextProvider : IOnScreenContextProvider
    {
        IHttpContextAccessor _httpContextAccessor;

        public OnScreenHttpContextProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public OnScreenContext GetContext()
        {
            return _httpContextAccessor.HttpContext.Items[OnScreenModeKeys.OnScreenContext] as OnScreenContext;
        }
    }
}
