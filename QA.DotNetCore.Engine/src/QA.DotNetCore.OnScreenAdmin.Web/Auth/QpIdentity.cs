using System;
using System.Linq;
using System.Security.Claims;

namespace QA.DotNetCore.OnScreenAdmin.Web.Auth
{
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
