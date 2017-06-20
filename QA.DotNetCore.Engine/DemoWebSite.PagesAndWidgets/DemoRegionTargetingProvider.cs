using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoRegionTargetingProvider : ITargetingProvider
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public DemoRegionTargetingProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IDictionary<string, object> GetValues()
        {
            var targetingVal = new List<int>();
            var regionStr = _httpContextAccessor.HttpContext.Request.Query["region"].FirstOrDefault();
            if (regionStr == null)
            {
                targetingVal.AddRange(new int[4] { 77507, 77996, 77512, 78043 });
            }
            else
            {
                targetingVal.AddRange(regionStr.Split(',')
                    .Where(_ => int.TryParse(_, out int tmp))
                    .Select(_ => int.Parse(_))
                    .ToArray()
                );
            }
            return new Dictionary<string, object> { { "region", targetingVal.ToArray() } };
        }
    }
}
