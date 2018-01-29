using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Interfaces
{
    public interface ICacheTagTracker
    {
        IEnumerable<CacheTagModification> TrackChanges();
    }
}
