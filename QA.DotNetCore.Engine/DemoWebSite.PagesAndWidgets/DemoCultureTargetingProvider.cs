using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoCultureTargetingProvider : ITargetingProvider
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public DemoCultureTargetingProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IDictionary<string, object> GetValues()
        {
            var cultureStr = _httpContextAccessor.HttpContext.Request.Query["culture"].FirstOrDefault();
            return new Dictionary<string, object> { { "culture", cultureStr ?? "ru-ru" } };
        }
    }
}
