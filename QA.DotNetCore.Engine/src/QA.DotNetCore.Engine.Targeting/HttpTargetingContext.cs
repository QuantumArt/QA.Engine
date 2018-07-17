using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Targeting
{
    /// <summary>
    /// Контекст, предоставляющий данные о параметрах таргетирования из текущего Http запроса
    /// </summary>
    public class HttpTargetingContext : ITargetingContext
    {
        public const string TargetingKeysContextKey = "HttpTargetingContext.Keys";

        readonly IHttpContextAccessor _httpContextAccessor;
        public HttpTargetingContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public object[] GetPossibleValues(string key)
        {
            var httpContextKey = TargetingPossibleValuesMiddleware.HttpContextKeyPrefix + key;
            return _httpContextAccessor.HttpContext.Items.ContainsKey(httpContextKey) ?
                _httpContextAccessor.HttpContext.Items[httpContextKey] as object[] :
                new object[0];
        }

        public string[] GetTargetingKeys()
        {
            return _httpContextAccessor.HttpContext.Items[TargetingKeysContextKey] as string[];
        }

        public object GetTargetingValue(string key)
        {
            return _httpContextAccessor.HttpContext.Items[key];
        }
    }
}
