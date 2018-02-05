using System;

namespace QA.DotNetCore.Caching.Interfaces
{
    public interface ICacheProvider
    {
        void Set(string key, object data, int cacheTime);
        void Set(string key, object data, TimeSpan expiration);
        void Add(object data, string key, string[] tags, TimeSpan expiration);
        bool IsSet(string key);
        bool TryGetValue(string key, out object result);
        object Get(string key);
        T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData, TimeSpan monitorTimeout = default(TimeSpan));
        T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getData, TimeSpan monitorTimeout = default(TimeSpan));
        void Invalidate(string key);
        void InvalidateByTag(string tag);
        void InvalidateByTags(params string[] tags);
    }
}
