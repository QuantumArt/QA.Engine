using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Caching.Utils.Configuration
{
    public class CacheTagsInvalidationConfigurator
    {
        readonly IServiceProvider _serviceProvider;
        readonly IList<ICacheTagTracker> _trackers = new List<ICacheTagTracker>();

        public CacheTagsInvalidationConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool UseTimer { get; private set; }
        public TimeSpan TimerInterval { get; private set; }
        public bool UseMiddleware { get; private set; }
        public string ExcludeRequestPathRegex { get; private set; }

        public void AddTracker<T>() where T : ICacheTagTracker
        {
            var filter = (T)_serviceProvider.GetRequiredService(typeof(T));
            if (filter == null)
                throw new Exception($"CacheTagsInvalidationConfigurator: Type {typeof(T).Name} not found in IoC! ");
            _trackers.Add(filter);
        }

        public void AddTracker(ICacheTagTracker tracker)
        {
            _trackers.Add(tracker);
        }

        public void ByTimer(TimeSpan interval)
        {
            UseTimer = true;
        }

        public void ByMiddleware(string excludeRequestPathRegex)
        {
            UseMiddleware = true;
            ExcludeRequestPathRegex = excludeRequestPathRegex;
        }

        public IEnumerable<ICacheTagTracker> GetTrackers()
        {
            return _trackers;
        }
    }
}
