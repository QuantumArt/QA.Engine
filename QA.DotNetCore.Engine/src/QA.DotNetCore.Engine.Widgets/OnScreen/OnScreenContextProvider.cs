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
            var isOnscreen = httpContext.Request.Query.ContainsKey("onscreen") && httpContext.Request.Query["onscreen"].Equals(new string[] { "true" });
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
