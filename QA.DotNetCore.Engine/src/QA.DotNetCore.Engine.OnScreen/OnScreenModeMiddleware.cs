using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.OnScreen.Configuration;

namespace QA.DotNetCore.Engine.OnScreen
{
    public class OnScreenModeMiddleware
    {
        private readonly RequestDelegate _next;

        public OnScreenModeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, OnScreenSettings onScreenSettings, Quantumart.QPublishing.Authentication.IAuthenticationService authenticationService)
        {
            //установим для запроса контекст OnScreen
            SetContext(httpContext, onScreenSettings, authenticationService);

            // Call the next delegate/middleware in the pipeline
            return _next(httpContext);
        }

        private void SetContext(HttpContext httpContext, OnScreenSettings onScreenSettings, Quantumart.QPublishing.Authentication.IAuthenticationService authenticationService)
        {
            var context = new OnScreenContext { Features = onScreenSettings.AvailableFeatures };
            if (onScreenSettings.AvailableFeatures > OnScreenFeatures.None)
            {
                //если предполагается наличие хотя бы одной фичи OnScreen, нужно аутентифицировать пользователя QP и авторизовать для него API OnScreen

                //для аутентификации пользователя нам нужна его связь с QP: это или наличие backend_sid в query, или токен авторизации уже ранее сохранённый в куки
                var sid = httpContext.Request.Query[onScreenSettings.BackendSidQueryKey].ToString();
                var token = httpContext.Request.Cookies[onScreenSettings.AuthCookieName];
                var tokenInvalid = false;

                if (token != null)
                {
                    //если в куках есть токен авторизации - проверим его
                    var auth = authenticationService.Authenticate(new Guid(token), onScreenSettings.ApiApplicationNameInQp); //пока что никак не обрабатываем возможные исключения при аутентификации в QP
                    tokenInvalid = auth == null;
                    if (auth != null) //null может быть в случае несуществующего/просроченного токена
                    {
                        context.User = new OnScreenUser { UserId = auth.UserId, ExpirationDate = auth.ExpirationDate };
                    }
                }

                if (!String.IsNullOrEmpty(sid) && (token == null || tokenInvalid))
                {
                    //если в query есть backend_sid, а токен авторизации в куки не сохранен или мы проверили его и он невалиден
                    //проведем аутентификацию по backend_sid. сохраним результат в куки
                    var auth = authenticationService.Authenticate(sid, onScreenSettings.AuthCookieLifetime, onScreenSettings.ApiApplicationNameInQp); //пока что никак не обрабатываем возможные исключения при аутентификации в QP
                    if (auth != null) //null может быть в случае некорректного/несуществующего sid
                    {
                        httpContext.Response.Cookies.Append(onScreenSettings.AuthCookieName, auth.Token.ToString(), new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.Add(onScreenSettings.AuthCookieLifetime)), SameSite = SameSiteMode.None });
                        context.User = new OnScreenUser { UserId = auth.UserId, ExpirationDate = auth.ExpirationDate };
                    }
                }

                //если аутентифицировать юзера QP не удалось
                if (context.User == null)
                    context.Features = OnScreenFeatures.None;
            }

            if (!httpContext.Items.ContainsKey(OnScreenModeKeys.OnScreenContext))
            {
                httpContext.Items.Add(OnScreenModeKeys.OnScreenContext, context);
            }
            else
            {
                httpContext.Items[OnScreenModeKeys.OnScreenContext] = context;
            }
        }
    }
}