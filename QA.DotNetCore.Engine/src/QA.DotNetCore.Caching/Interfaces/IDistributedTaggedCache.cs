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
        /// Synchronous alternative of <see cref="GetAsync(IEnumerable{string}, CancellationToken)"/>.
        /// </summary>
        IEnumerable<byte[]> Get(IEnumerable<string> keys, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="GetOrAddAsync{TId}(CacheInfo{TId}[], AsyncDataValuesFactoryDelegate{TId, MemoryStream}, CancellationToken)"/>.
        /// </summary>
        IReadOnlyList<byte[]> GetOrAdd<TId>(
            CacheInfo<TId>[] dataInfos,
            DataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory,
            CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="InvalidateAsync(string, CancellationToken)"/>.
        /// </summary>
        void Invalidate(string key, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="InvalidateTagAsync(string, CancellationToken)"/>.
        /// </summary>
        void InvalidateTag(string tag, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IDistributedTaggedCache.ExistAsync(IEnumerable{string}, CancellationToken)"/>.
        /// </summary>
        IEnumerable<bool> Exist(IEnumerable<string> keys, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="SetAsync(string, IEnumerable{string}, TimeSpan, MemoryStream, CancellationToken)"/>.
        /// </summary>
        void Set(string key, IEnumerable<string> tags, TimeSpan expiry, MemoryStream data, CancellationToken token = default);

        /// <summary>
        /// Get cached data by the <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">Keys associated with data.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>List of cached data (or null if a key is missing).</returns>
        Task<IEnumerable<byte[]>> GetAsync(IEnumerable<string> keys, CancellationToken token = default);

        /// <summary>
        /// Atomically get existing cache or otherwise execute <paramref name="dataStreamFactory"/> to obtain data and cache it.
        /// </summary>
        /// <param name="cacheInfos">Information to get or set cache.</param>
        /// <param name="dataStreamFactory">Method to retrieve fresh data.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Cached data.</returns>
        Task<IReadOnlyList<byte[]>> GetOrAddAsync<TId>(
            CacheInfo<TId>[] cacheInfos,
            AsyncDataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory,
            CancellationToken token = default);

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
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Caching task.</returns>
        Task SetAsync(string key, IEnumerable<string> tags, TimeSpan expiry, MemoryStream dataStream, CancellationToken token = default);
    }
}
