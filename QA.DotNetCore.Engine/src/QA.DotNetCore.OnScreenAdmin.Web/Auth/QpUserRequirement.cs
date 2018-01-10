using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace QA.DotNetCore.OnScreenAdmin.Web.Auth
{
    /// <summary>
    /// Policy, проверяющее наличие юзера с Id > 0
    /// </summary>
    public class QpUserRequirement : AuthorizationHandler<QpUserRequirement>, IAuthorizationRequirement
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, QpUserRequirement requirement)
        {
            if (context.User.Identity is QpIdentity)
            {
                var identity = context.User.Identity as QpIdentity;
                if (identity.UserId > 0)
                { 
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
        }
    }
}
