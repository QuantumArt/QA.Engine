using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QA.DotNetCore.OnScreenAdmin.Web.Auth
{
    public class QpAuthHandler : AuthenticationHandler<QpAuthOptions>
    {
        readonly Quantumart.QPublishing.Authentication.AuthenticationService _authenticationService;

        public QpAuthHandler(Quantumart.QPublishing.Authentication.AuthenticationService authenticationService, IOptionsMonitor<QpAuthOptions> options, ILoggerFactory logger)
            : base(options, logger, null, null)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Options.Settings.Enabled)
            {
                //если аутентификация отключена, то токен игнорируем и считаем, что доступ есть из под дефолтного юзера
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

    public class QpIdentity : ClaimsIdentity
    {
        public QpIdentity(int userId, DateTime expiration)
        {
            AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
            AddClaim(new Claim(ClaimTypes.Expiration, expiration.ToString()));
        }

        public int UserId
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (claim == null)
                    return 0;
                Int32.TryParse(claim.Value, out int id);
                return id;
            }
        }

        public DateTime ExpirationDate
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Expiration);
                if (claim == null)
                    return DateTime.MinValue;
                DateTime.TryParse(claim.Value, out DateTime dt);
                return dt;
            }
        }
    }
}
