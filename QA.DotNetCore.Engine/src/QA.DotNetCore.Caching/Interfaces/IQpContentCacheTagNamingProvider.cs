using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Interfaces
{
    /// <summary>
    /// Интерфейс, задающий правила именования кештегов для контентов qp
    /// </summary>
    public interface IQpContentCacheTagNamingProvider
    {
        string Get(string contentName, int siteId, bool isStage);
        string GetByNetName(string contentNetName, int siteId, bool isStage);

        /// <summary>
        /// Получить словарь кэштегов для контентов
        /// </summary>
        /// <param name="contentIds"></param>
        /// <param name="siteId"></param>
        /// <param name="isStage"></param>
        /// <returns></returns>
        Dictionary<int, string> GetByContentIds(int[] contentIds, int siteId, bool isStage);
    }
}
