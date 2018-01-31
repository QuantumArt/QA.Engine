using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Caching.Utils.Configuration;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Caching.Utils
{
    public class CacheTrackersAccessor : ICacheTrackersAccessor
    {
        private readonly CacheTagsInvalidationConfigurator _cfg;

        public CacheTrackersAccessor(CacheTagsInvalidationConfigurator cfg)
        {
            _cfg = cfg;
        }

        public IEnumerable<ICacheTagTracker> Get()
        {
            return _cfg.GetTrackers();
        }
    }
}
