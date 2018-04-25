using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    public class CacheTagsTrackersConfigurator
    {
        readonly IServiceProvider _serviceProvider;
        readonly IList<ICacheTagTracker> _trackers = new List<ICacheTagTracker>();
        readonly IList<Type> _trackerTypes = new List<Type>();

        public CacheTagsTrackersConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void AddTracker<T>() where T : ICacheTagTracker
        {
            _trackerTypes.Add(typeof(T));
        }

        public void AddTracker(ICacheTagTracker tracker)
        {
            _trackers.Add(tracker);
        }

        public IEnumerable<ICacheTagTracker> GetTrackers()
        {
            return _trackers.Concat(_trackerTypes.Select(t => (ICacheTagTracker)_serviceProvider.GetRequiredService(t)));
        }
    }
}
