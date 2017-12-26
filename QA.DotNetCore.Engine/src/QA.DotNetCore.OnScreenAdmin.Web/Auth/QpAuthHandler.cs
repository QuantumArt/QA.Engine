using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QA.DotNetCore.OnScreenAdmin.Web.Auth
{
    public class QpAuthHandler : AuthenticationHandler<QpAuthOptions>
    {
        readonly Quantumart.QPublishing.Authentication.IAuthenticationService _authenticationService;

        public QpAuthHandler(Quantumart.QPublishing.Authentication.IAuthenticationService authenticationService, IOptionsMonitor<QpAuthOptions> options, ILoggerFactory logger)
            : base(options, logger, null, null)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Options.Settings.WorkAsAdministrator)
            {
                //если опция WorkAsAdministrator включена, значит аутентификация по токену отключена
                //аутентифицируемся как админ
                var principal = CreatePrincipal(1, DateTime.Now.AddDays(1));
                return AuthenticateResult.Success(new AuthenticationTicket(principal, QpAuthDefaults.AuthenticationScheme));
            }

            var accessToken = GetAccessToken();
            if (accessToken == null)
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                var authToken = _authenticationService.Authenticate(new Guid(accessToken), Options.Settings.ApplicationNameInQp);
                var principal = CreatePrincipal(authToken.UserId, authToken.ExpirationDate);
                return AuthenticateResult.Success(new AuthenticationTicket(principal, QpAuthDefaults.AuthenticationScheme));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private string GetAccessToken()
        {
            return Request.Query["token"];
        }

        private ClaimsPrincipal CreatePrincipal(int userId, DateTime expiration)
        {
            return new ClaimsPrincipal(new QpIdentity(userId, expiration));
        }
    }
}
