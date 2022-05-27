using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    public interface IDistributedTaggedCache : IDisposable
    {
        string GetClientId(CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.GetAsync(string, CancellationToken)"/>.
        /// </summary>
        byte[] Get(string key, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.GetOrAddAsync(string, string[], TimeSpan, Func{Task{MemoryStream}}, CancellationToken)"/>.
        /// </summary>
        byte[] GetOrAdd(string key, string[] tags, TimeSpan expiry, Func<MemoryStream> dataFactory, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.InvalidateAsync(string, CancellationToken)"/>.
        /// </summary>
        void Invalidate(string key, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.InvalidateTagAsync(string, CancellationToken)"/>.
        /// </summary>
        void InvalidateTag(string tag, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.IsExistAsync(string, CancellationToken)"/>.
        /// </summary>
        bool IsExist(string key, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.SetAsync(string, IEnumerable{string}, TimeSpan, MemoryStream, CancellationToken)"/>.
        /// </summary>
        void Set(string key, IEnumerable<string> tags, TimeSpan expiry, MemoryStream data, CancellationToken token = default);

        /// <summary>
        /// Get cached data by the paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key associated with data.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Cached data (or null if key is missing).</returns>
        Task<byte[]> GetAsync(string key, CancellationToken token = default);

        /// <summary>
        /// Atomically get existing cache or otherwise execute <paramref name="dataFactory"/> to obtain data and cache it.
        /// </summary>
        /// <param name="key">Key associated with data.</param>
        /// <param name="tags">Tags to link to key.</param>
        /// <param name="expiry">Expiration time for new cache.</param>
        /// <param name="dataFactory">Method to retrieve fresh data.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Cached data.</returns>
        Task<byte[]> GetOrAddAsync(string key, string[] tags, TimeSpan expiry, Func<Task<MemoryStream>> dataFactory, CancellationToken token = default);

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
        /// <param name="key">Key to find.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Search key operation task.</returns>
        Task<bool> IsExistAsync(string key, CancellationToken token = default);

        /// <summary>
        /// Set data in cache.
        /// </summary>
        /// <param name="key">Key associated with data.</param>
        /// <param name="tags">Tags to link to key.</param>
        /// <param name="expiry">Data expiration time in cache.</param>
        /// <param name="dataStream">Data to cache.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Caching task.</returns>
        Task SetAsync(string key, IEnumerable<string> tags, TimeSpan expiry, MemoryStream dataStream, CancellationToken token = default);
    }
}
