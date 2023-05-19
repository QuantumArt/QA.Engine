using System;
using System.Linq;
using System.Threading.Tasks;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching.Distributed
{
    public static class DistributedTaggedCacheExtensions
    {
        public static object Get(this IExternalCache taggedCache, string key)
        {
            if (taggedCache is null)
            {
                throw new ArgumentNullException(nameof(taggedCache));
            }

            return taggedCache.Get<object>(new[] { key }).Single();
        }

        public static bool IsExist(this IExternalCache taggedCache, string key)
        {
            if (taggedCache is null)
            {
                throw new ArgumentNullException(nameof(taggedCache));
            }

            return taggedCache.Exist(new[] { key }).Single();
        }

        public static async Task<byte[]> GetAsync(this IExternalCache taggedCache, string key)
        {
            if (taggedCache is null)
            {
                throw new ArgumentNullException(nameof(taggedCache));
            }

            var cachedValues = await taggedCache.GetAsync(new[] { key });

            return cachedValues.Single();
        }

        public static async Task<bool> ExistAsync(this IExternalCache taggedCache, string key)
        {
            if (taggedCache is null)
            {
                throw new ArgumentNullException(nameof(taggedCache));
            }

            var existCollection = await taggedCache.ExistAsync(new[] { key });

            return existCollection.Single();
        }

  }
}
