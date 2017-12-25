using Microsoft.AspNetCore.Authentication;
using System;

namespace QA.DotNetCore.OnScreenAdmin.Web.Auth
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddQpAuth(this AuthenticationBuilder builder, Action<QpAuthOptions> configureOptions)
        {
            return builder.AddScheme<QpAuthOptions, QpAuthHandler>(QpAuthDefaults.AuthenticationScheme, "Qp Auth via access tokens", configureOptions);
        }
    }
}
