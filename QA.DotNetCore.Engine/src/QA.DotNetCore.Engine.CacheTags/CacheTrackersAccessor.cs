using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.CacheTags
{
    public class CacheTrackersAccessor : ICacheTrackersAccessor
    {
        private readonly ServiceSetConfigurator<ICacheTagTracker> _cfg;

        public CacheTrackersAccessor(ServiceSetConfigurator<ICacheTagTracker> cfg)
        {
            _cfg = cfg;
        }

        public IEnumerable<ICacheTagTracker> Get()
        {
            return _cfg.GetServices();
        }
    }
}
