using QA.DotNetCore.Caching.Exceptions;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing.Tests.StubClasses
{
    public class StubCacheProvider : IMemoryCacheProvider
    {
        public void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData, TimeSpan waitForCalculateTimeout = default)
        {
            T deprecatedResult = Convert<T>(null);
            if (deprecatedResult == null)
                throw new DeprecateCacheIsExpiredOrMissingException();

            return getData();
        }

        public T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getData, TimeSpan waitForCalculateTimeout = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetOrAddAsync<T>(string cacheKey, TimeSpan expiration, Func<Task<T>> getData, TimeSpan waitForCalculateTimeout = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<Task<T>> getData, TimeSpan waitForCalculateTimeout = default)
        {
            throw new NotImplementedException();
        }

        public void Invalidate(string key)
        {
            throw new NotImplementedException();
        }

        public void InvalidateByTag(string tag)
        {
            throw new NotImplementedException();
        }

        public void InvalidateByTags(params string[] tags)
        {
            throw new NotImplementedException();
        }

        public bool IsSet(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object data, int cacheTimeInSeconds)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object data, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object result)
        {
            throw new NotImplementedException();
        }

        private static T Convert<T>(object result)
        {
            return result == null ? default : (T)result;
        }
    }
}
