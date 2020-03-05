using System;

namespace QA.DotNetCore.Caching.Interfaces
{
    public interface ICacheTagWatcher
    {
        void TrackChanges(IServiceProvider provider);
    }
}
