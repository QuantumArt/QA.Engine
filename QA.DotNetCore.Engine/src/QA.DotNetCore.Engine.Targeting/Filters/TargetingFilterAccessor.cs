using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class TargetingFilterAccessor : ITargetingFilterAccessor
    {
        readonly KeyedServiceSetConfigurator<string, ITargetingFilter> _cfg;
        readonly IHttpContextAccessor _httpContextAccessor;

        public TargetingFilterAccessor(KeyedServiceSetConfigurator<string, ITargetingFilter> cfg, IHttpContextAccessor httpContextAccessor)
        {
            _cfg = cfg;
            _httpContextAccessor = httpContextAccessor;
        }

        public ITargetingFilter Get()
        {
            return new UnitedFilter(_cfg.GetServices(_httpContextAccessor.HttpContext.RequestServices));
        }

        public ITargetingFilter Get(string key)
        {
            return new UnitedFilter(_cfg.GetServices(_httpContextAccessor.HttpContext.RequestServices, key));
        }
    }
}
