using System;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    /// <summary>
    /// Combines two cache sources: global and in-memory.
    /// </summary>
    public interface ICompositeCacheProvider
    {
        T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getData, TimeSpan waitForCalculateTimeout = default);
        Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<Task<T>> getData, TimeSpan waitForCalculateTimeout = default);
    }
}
