using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Distributed
{
    public interface IExternalCache : IDisposable
    {
 
        IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys);

        bool TryAdd(object value, string key, string deprecatedKey, string[] tags, TimeSpan expiration, TimeSpan deprecatedExpiration);
        
        /// <summary>
        /// Synchronous alternative of <see cref="InvalidateAsync(string, CancellationToken)"/>.
        /// </summary>
        void Invalidate(string key, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="InvalidateTagAsync(string, CancellationToken)"/>.
        /// </summary>
        void InvalidateTag(string tag, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IExternalCache.ExistAsync(IEnumerable{string}, CancellationToken)"/>.
        /// </summary>
        IEnumerable<bool> Exist(IEnumerable<string> keys, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="SetAsync"/>.
        /// </summary>
        void Set(string key, IEnumerable<string> tags, TimeSpan expiry, MemoryStream data, string deprecatedKey, TimeSpan deprecatedExpiry, CancellationToken token = default);

        /// <summary>
        /// Get cached data by the <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">Keys associated with data.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>List of cached data (or null if a key is missing).</returns>
        Task<IEnumerable<byte[]>> GetAsync(IEnumerable<string> keys, CancellationToken token = default);

        /// <summary>
        /// Remove data from cache by the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key associated with data.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Invalidation task.</returns>
        Task InvalidateAsync(string key, CancellationToken token = default);

        /// <summary>
        /// Remove all data linked to the <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">Tag to invalidate.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Invalidation task.</returns>
        Task InvalidateTagAsync(string tag, CancellationToken token = default);

        /// <summary>
        /// Whether there is the <paramref name="key"/> in cache.
        /// </summary>
        /// <param name="keys">Keys to find.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Search keys operation task.</returns>
        Task<IEnumerable<bool>> ExistAsync(IEnumerable<string> keys, CancellationToken token = default);

        /// <summary>
        /// Set data in cache.
        /// </summary>
        /// <param name="key">Key associated with data.</param>
        /// <param name="tags">Tags to link to key.</param>
        /// <param name="expiry">Data expiration time in cache.</param>
        /// <param name="dataStream">Data to cache.</param>
        /// <param name="deprecatedKey"></param>
        /// <param name="deprecatedExpiry"></param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Caching task.</returns>
        Task SetAsync(string key, IEnumerable<string> tags, TimeSpan expiry, MemoryStream dataStream, string deprecatedKey, TimeSpan deprecatedExpiry, CancellationToken token = default);
    }
}
