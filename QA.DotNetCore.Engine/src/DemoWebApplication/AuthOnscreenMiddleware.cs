using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApplication
{
    public class AuthOnscreenMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthOnscreenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, Quantumart.QPublishing.Authentication.IAuthenticationService authenticationService)
        {
            var onScreenAuthCookieName = "qp_auth_token";
            var sid = context.Request.Query["backend_sid"].ToString();
            var token = context.Request.Cookies[onScreenAuthCookieName];
            if (!String.IsNullOrEmpty(sid) && token == null)
            {
                var onScreenAuthCookieLifetime = TimeSpan.FromHours(1);
                var onScreenAppNameInQp = "onscreen-api";
                try
                {
                    var auth = authenticationService.Authenticate(sid, onScreenAuthCookieLifetime, onScreenAppNameInQp);
                    context.Response.Cookies.Append(onScreenAuthCookieName, auth.Token.ToString(), new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.Add(onScreenAuthCookieLifetime)) });
                }
                finally
                { }
            }

            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
