using System;

namespace QA.DotNetCore.Caching
{
    public interface ICacheProvider
    {
        void Set(string key, object data, int cacheTime);
        void Set(string key, object data, TimeSpan expiration);
        void Add(object data, string key, string[] tags, TimeSpan expiration);
        bool IsSet(string key);
        bool TryGetValue(string key, out object result);
        object Get(string key);
        void Invalidate(string key);
        void InvalidateByTag(string tag);
        void InvalidateByTags(params string[] tags);
    }
}
