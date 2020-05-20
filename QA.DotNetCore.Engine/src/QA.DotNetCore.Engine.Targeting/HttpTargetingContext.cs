using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;

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

        public string[] GetTargetingKeys()
        {
            return (_httpContextAccessor.HttpContext.Items[TargetingKeysContextKey] as string[]) ?? new string[0];
        }

        public object GetTargetingValue(string key)
        {
            return _httpContextAccessor.HttpContext.Items[key];
        }
    }
}
