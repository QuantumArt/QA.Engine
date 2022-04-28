using System;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    public interface IMemoryCacheProvider
    {
        T GetOrAdd<T>(
            string cacheKey,
            TimeSpan expiration,
            Func<T> getData,
            TimeSpan waitForCalculateTimeout = default);

        Task<T> GetOrAddAsync<T>(
            string cacheKey,
            TimeSpan expiration,
            Func<Task<T>> getData,
            TimeSpan waitForCalculateTimeout = default);
        void Set(string key, object data, TimeSpan expiration);
    }
}
