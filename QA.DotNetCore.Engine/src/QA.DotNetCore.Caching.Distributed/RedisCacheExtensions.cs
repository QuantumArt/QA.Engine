using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Caching.Distributed
{
    public static class RedisCacheExtensions
    {
        public static RedisKey GetRedisKey(this CacheKey cacheKey) => new RedisKey(cacheKey.ToString());

        public static RedisValue GetRedisValue(this CacheKey cacheKey) => new RedisValue(cacheKey.ToString());


        internal static IEnumerable<CacheKey> CreateTags(this ExternalCacheKeyFactory keyFactory, IEnumerable<string> tags)
        {
            if (keyFactory is null)
            {
                throw new ArgumentNullException(nameof(keyFactory));
            }

            if (tags is null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            return tags.Select(keyFactory.CreateTag);
        }
    }
}
