using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Interfaces
{
    /// <summary>
    /// Интерфейс компонента, который отслеживает изменения кештегов (что можно будет потом использовать для последующей инвалидации по изменившимся кештегам)
    /// </summary>
    public interface ICacheTagTracker
    {
        IEnumerable<CacheTagModification> TrackChanges();
    }
}
