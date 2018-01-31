using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Interfaces
{
    public interface ICacheTrackersAccessor
    {
        IEnumerable<ICacheTagTracker> Get();
    }
}
