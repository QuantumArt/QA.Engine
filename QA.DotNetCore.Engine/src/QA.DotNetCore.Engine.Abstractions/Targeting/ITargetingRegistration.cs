using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingRegistration
    {
        void RegisterTargetingServices(IServiceCollection services, IConfiguration configuration);
        void ConfigureTargeting(IApplicationBuilder app);
    }
}
