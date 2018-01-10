using Microsoft.AspNetCore.Authentication;

namespace QA.DotNetCore.OnScreenAdmin.Web.Auth
{
    public class QpAuthOptions : AuthenticationSchemeOptions
    {
        public QpAuthSettings Settings { get; set; } = new QpAuthSettings { WorkAsAdministrator = false, ApplicationNameInQp = "onscreen-api" };
    }

    public class QpAuthSettings
    {
        public bool WorkAsAdministrator { get; set; }
        public string ApplicationNameInQp { get; set; }
    }
}
