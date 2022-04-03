using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Redis
{
    public interface IDistributedTaggedCache : IAsyncDistributedTaggedCache
    {
        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.GetAsync(string, CancellationToken)"/>.
        /// </summary>
        byte[] Get(string key, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.GetOrAddAsync(string, string[], TimeSpan, Func{Task{byte[]}}, CancellationToken)"/>.
        /// </summary>
        byte[] GetOrAdd(string key, string[] tags, TimeSpan expiry, Func<byte[]> dataFactory, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.InvalidateAsync(string, CancellationToken)"/>.
        /// </summary>
        void Invalidate(string key, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.InvalidateTagAsync(string, CancellationToken)"/>.
        /// </summary>
        void InvalidateTag(string tag, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.IsExistsAsync(string, CancellationToken)"/>.
        /// </summary>
        bool IsExists(string key, CancellationToken token = default);

        /// <summary>
        /// Synchronous alternative of <see cref="IAsyncDistributedTaggedCache.SetAsync(string, IEnumerable{string}, TimeSpan, byte[], CancellationToken)"/>.
        /// </summary>
        void Set(string key, IEnumerable<string> tags, TimeSpan expiry, byte[] data, CancellationToken token = default);
    }
}
