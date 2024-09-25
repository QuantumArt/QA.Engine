using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class TargetingFilterAccessor : ITargetingFilterAccessor
    {
        private readonly KeyedServiceSetConfigurator<TargetingDestination, ITargetingFilter> _cfg;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TargetingFilterAccessor(KeyedServiceSetConfigurator<TargetingDestination, ITargetingFilter> cfg, IHttpContextAccessor httpContextAccessor)
        {
            _cfg = cfg;
            _httpContextAccessor = httpContextAccessor;
        }

        public ITargetingFilter Get()
        {
            var filters = _cfg.GetServices(_httpContextAccessor.HttpContext.RequestServices);

            return GetResultFilter(filters);
        }

        public ITargetingFilter Get(TargetingDestination key)
        {
            var filters = _cfg.GetServices(_httpContextAccessor.HttpContext.RequestServices, key);

            return GetResultFilter(filters);
        }

        private static ITargetingFilter GetResultFilter(IEnumerable<ITargetingFilter> filters)
        {
            return (filters?.Any() ?? false)
                ? new UnitedFilter(filters)
                : new NullFilter();
        }
    }
}
