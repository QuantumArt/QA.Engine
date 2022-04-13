using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.CacheTags
{
    public class CacheTrackersAccessor : ICacheTrackersAccessor
    {
        private readonly ServiceSetConfigurator<ICacheTagTracker> _cfg;
        private readonly IServiceProvider _sp;

        public CacheTrackersAccessor(ServiceSetConfigurator<ICacheTagTracker> cfg, IServiceProvider sp)
        {
            _cfg = cfg;
            _sp = sp;
        }

        public IEnumerable<ICacheTagTracker> Get(IServiceProvider provider)
        {
            return _cfg.GetServices(provider ?? _sp);
        }
    }
}
