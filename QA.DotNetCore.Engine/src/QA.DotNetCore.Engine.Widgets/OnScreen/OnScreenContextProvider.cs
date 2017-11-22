using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QA.DotNetCore.Engine.Widgets.OnScreen
{
    public class OnScreenContextProvider : IOnScreenContextProvider
    {
        public OnScreenContext GetContext(HttpContext httpContext)
        {
            var queryOnscreen = httpContext.Request.Query.ContainsKey("onscreen") && httpContext.Request.Query["onscreen"].Equals(new string[] { "true" });
            var cookieOnscreen = httpContext.Request.Cookies.ContainsKey("onscreen") && httpContext.Request.Cookies["onscreen"].Equals("true");
            var isOnscreen = queryOnscreen || cookieOnscreen;
            return isOnscreen
                ?
            new OnScreenContext
            {
                IsAuthorised = true,
                IsEditMode = true,
                User = new OnScreenUser
                {
                    UserId = 1
                }
            }
            : null;
        }
    }
}
