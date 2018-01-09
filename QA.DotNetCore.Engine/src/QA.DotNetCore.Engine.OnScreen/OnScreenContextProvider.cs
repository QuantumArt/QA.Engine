using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using System;

namespace QA.DotNetCore.Engine.OnScreen
{
    public class OnScreenContextProvider : IOnScreenContextProvider
    {
        IHttpContextAccessor _httpContextAccessor;
        OnScreenSettings _onScreenSettings;
        Quantumart.QPublishing.Authentication.IAuthenticationService _authenticationService;

        public OnScreenContextProvider(IHttpContextAccessor httpContextAccessor, OnScreenSettings onScreenSettings, Quantumart.QPublishing.Authentication.IAuthenticationService authenticationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _onScreenSettings = onScreenSettings;
            _authenticationService = authenticationService;
        }

        public void SetContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var context = new OnScreenContext { Features = _onScreenSettings.AvailableFeatures };
            if (_onScreenSettings.AvailableFeatures > OnScreenFeatures.None)
            {
                //если предполагается наличие хотя бы одной фичи OnScreen, нужно аутентифицировать пользователя QP и авторизовать для него API OnScreen

                //для аутентификации пользователя нам нужна его связь с QP: это или наличие backend_sid в query, или токен авторизации уже ранее сохранённый в куки
                var sid = httpContext.Request.Query[_onScreenSettings.BackendSidQueryKey].ToString();
                var token = httpContext.Request.Cookies[_onScreenSettings.AuthCookieName];
                var tokenInvalid = false;

                if (token != null)
                {
                    //если в куках есть токен авторизации - проверим его
                    try
                    {
                        var auth = _authenticationService.Authenticate(new Guid(token), _onScreenSettings.ApiApplicationNameInQp);
                        tokenInvalid = auth == null;
                        if (auth != null) //null может быть в случае несуществующего/просроченного токена
                        {
                            context.User = new OnScreenUser { UserId = auth.UserId, ExpirationDate = auth.ExpirationDate };
                        }
                    }
                    finally //пока что никак не обрабатываем возможные исключения при аутентификации в QP
                    { }
                }

                if (!String.IsNullOrEmpty(sid) && (token == null || tokenInvalid))
                {
                    //если в query есть backend_sid, а токен авторизации в куки не сохранен или мы проверили его и он невалиден
                    //проведем аутентификацию по backend_sid. сохраним результат в куки
                    try
                    {
                        var auth = _authenticationService.Authenticate(sid, _onScreenSettings.AuthCookieLifetime, _onScreenSettings.ApiApplicationNameInQp);
                        if (auth != null) //null может быть в случае некорректного/несуществующего sid
                        {
                            httpContext.Response.Cookies.Append(_onScreenSettings.AuthCookieName, auth.Token.ToString(), new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.Add(_onScreenSettings.AuthCookieLifetime)), SameSite = SameSiteMode.None });
                            context.User = new OnScreenUser { UserId = auth.UserId, ExpirationDate = auth.ExpirationDate };
                        }
                    }
                    finally //пока что никак не обрабатываем возможные исключения при аутентификации в QP
                    { }
                }

                //если аутентифицировать юзера QP не удалось
                if (context.User == null)
                    context.Features = OnScreenFeatures.None;
            }

            if (!httpContext.Items.ContainsKey(OnScreenModeKeys.OnScreenContext))
                httpContext.Items.Add(OnScreenModeKeys.OnScreenContext, context);
            else
                httpContext.Items[OnScreenModeKeys.OnScreenContext] = context;
        }

        public OnScreenContext GetContext()
        {
            return _httpContextAccessor.HttpContext.Items[OnScreenModeKeys.OnScreenContext] as OnScreenContext;
        }
    }
}
