using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    public class CacheTagsTrackersConfigurator
    {
        readonly IServiceProvider _serviceProvider;
        readonly IList<ICacheTagTracker> _trackers = new List<ICacheTagTracker>();

        public CacheTagsTrackersConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void AddTracker<T>() where T : ICacheTagTracker
        {
            var tracker = (T)_serviceProvider.GetRequiredService(typeof(T));
            if (tracker == null)
                throw new Exception($"CacheTagsInvalidationConfigurator: Type {typeof(T).Name} not found in IoC! ");
            _trackers.Add(tracker);
        }

        public void AddTracker(ICacheTagTracker tracker)
        {
            _trackers.Add(tracker);
        }

        public IEnumerable<ICacheTagTracker> GetTrackers()
        {
            return _trackers;
        }
    }
}
