using System.Collections.Generic;
using QA.DotNetCore.Engine.Persistent.Interfaces;

namespace QA.DotNetCore.Caching.Interfaces
{
    /// <summary>
    /// Интерфейс, задающий правила именования кештегов для контентов qp
    /// </summary>
    public interface IQpContentCacheTagNamingProvider
    {
        string Get(string contentName, int siteId, bool isStage);
        string GetByNetName(string contentNetName, int siteId, bool isStage);
        Dictionary<int, string> GetByContentIds(int[] contentIds, bool isStage);
        void SetUnitOfWork(IUnitOfWork unitOfWork);
    }
}
