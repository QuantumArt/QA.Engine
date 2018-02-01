using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.CacheTags
{
    public class CacheTrackersAccessor : ICacheTrackersAccessor
    {
        private readonly CacheTagsTrackersConfigurator _cfg;

        public CacheTrackersAccessor(CacheTagsTrackersConfigurator cfg)
        {
            _cfg = cfg;
        }

        public IEnumerable<ICacheTagTracker> Get()
        {
            return _cfg.GetTrackers();
        }
    }
}
