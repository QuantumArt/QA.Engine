using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting.TargetingProviders
{
    /// <summary>
    /// Получает данные таргетинга из параметров query string
    /// </summary>
    public class QueryTargetingProvider : ITargetingProvider
    {
        private const string TargetingPrefix = "t.";
        readonly IHttpContextAccessor _httpContextAccessor;

        public QueryTargetingProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IDictionary<string, object> GetValues() => _httpContextAccessor.HttpContext?.Request.Query
            .Where(item => item.Key.StartsWith(TargetingPrefix))
            .ToDictionary(
                item => item.Key.Replace(TargetingPrefix, string.Empty),
                item => (object)item.Value.ToString());
    }
}
