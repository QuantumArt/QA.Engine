using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.CacheTags
{
    public class CacheTrackersAccessor : ICacheTrackersAccessor
    {
        private readonly ServiceSetConfigurator<ICacheTagTracker> _cfg;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CacheTrackersAccessor(ServiceSetConfigurator<ICacheTagTracker> cfg, IHttpContextAccessor httpContextAccessor)
        {
            _cfg = cfg;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<ICacheTagTracker> Get()
        {
            return _cfg.GetServices(_httpContextAccessor.HttpContext.RequestServices);
        }
    }
}
