using System.Collections.Generic;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching
{
    /// <summary>
    /// Null-реализация для именования кештегов контентов QP. Используется, если кештеги не нужны.
    /// </summary>
    public class NullQpContentCacheTagNamingProvider : IQpContentCacheTagNamingProvider
    {
        public string Get(string contentName, int siteId, bool isStage)
        {
            return null;
        }

        public string GetByNetName(string contentNetName, int siteId, bool isStage)
        {
            return null;
        }

        public Dictionary<int, string> GetByContentIds(int[] contentIds, int siteId, bool isStage)
        {
            return new Dictionary<int, string>();
        }
    }
}
