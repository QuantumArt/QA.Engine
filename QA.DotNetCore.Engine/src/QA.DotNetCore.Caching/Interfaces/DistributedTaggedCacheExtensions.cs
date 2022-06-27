using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    public static class DistributedTaggedCacheExtensions
    {
        public static byte[] Get(this IDistributedTaggedCache taggedCache, string key)
        {
            if (taggedCache is null)
            {
                throw new ArgumentNullException(nameof(taggedCache));
            }

            return taggedCache.Get(new[] { key }).Single();
        }

        public static bool IsExist(this IDistributedTaggedCache taggedCache, string key)
        {
            if (taggedCache is null)
            {
                throw new ArgumentNullException(nameof(taggedCache));
            }

            return taggedCache.Exist(new[] { key }).Single();
        }

        public static async Task<byte[]> GetAsync(this IDistributedTaggedCache taggedCache, string key)
        {
            if (taggedCache is null)
            {
                throw new ArgumentNullException(nameof(taggedCache));
            }

            var cachedValues = await taggedCache.GetAsync(new[] { key });

            return cachedValues.Single();
        }

        public static async Task<bool> ExistAsync(this IDistributedTaggedCache taggedCache, string key)
        {
            if (taggedCache is null)
            {
                throw new ArgumentNullException(nameof(taggedCache));
            }

            var existCollection = await taggedCache.ExistAsync(new[] { key });

            return existCollection.Single();
        }

        /// <summary>
        /// Synchronous alternative of <see cref="GetOrAddAsync(IDistributedTaggedCache, string, string[], TimeSpan, Func{Task{MemoryStream}}, CancellationToken)/>.
        /// </summary>
        public static byte[] GetOrAdd(
            this IDistributedTaggedCache cache,
            string key,
            string[] tags,
            TimeSpan expiry,
            Func<MemoryStream> dataStreamFactory,
            CancellationToken token = default)
        {
            if (cache is null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            var cacheInfos = new[] { new CacheInfo<bool>(default, key, expiry, tags) };

            return cache
                .GetOrAdd(cacheInfos, SingleOrDefaultDataFactory, token)
                .Single();

            IEnumerable<MemoryStream> SingleOrDefaultDataFactory(IEnumerable<CacheInfo<bool>> missingCacheInfos)
            {
                return missingCacheInfos.Any() ? new[] { dataStreamFactory() } : Array.Empty<MemoryStream>();
            }
        }

        /// <summary>
        /// Atomically get existing cache or otherwise execute <paramref name="dataStreamFactory"/> to obtain data and cache it.
        /// </summary>
        /// <param name="key">Key associated with data.</param>
        /// <param name="tags">Tags to link to key.</param>
        /// <param name="expiry">Expiration time for new cache.</param>
        /// <param name="dataStreamFactory">Method to retrieve fresh data.</param>
        /// <param name="token">Operation cancellation token</param>
        /// <returns>Cached data.</returns>
        public static async Task<byte[]> GetOrAddAsync(
            this IDistributedTaggedCache cache,
            string key,
            string[] tags,
            TimeSpan expiry,
            Func<Task<MemoryStream>> dataStreamFactory,
            CancellationToken token = default)
        {
            if (cache is null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            var cacheInfos = new[] { new CacheInfo<bool>(default, key, expiry, tags) };

            var results = await cache
                .GetOrAddAsync(cacheInfos, SingleOrDefaultDataFactoryAsync, token);

            return results.Single();

            async Task<IEnumerable<MemoryStream>> SingleOrDefaultDataFactoryAsync(IEnumerable<CacheInfo<bool>> missingCacheInfos)
            {
                return missingCacheInfos.Any() ? new[] { await dataStreamFactory() } : Array.Empty<MemoryStream>();
            }
        }
    }
}
