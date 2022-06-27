using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QA.DotNetCore.Caching.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QA.DotNetCore.Caching.Distributed.Tests
{
    public class RedisCacheTests
    {
        private const string ConnectionString = "SPBREDIS01.ARTQ.COM:6407";
        private static readonly string s_instanceName = "wptests:" + Guid.NewGuid().ToString();

        private static RedisCacheSettings CreateDefaultRedisCacheOptions() =>
            new()
            {
                Configuration = ConnectionString,
                InstanceName = s_instanceName,
                TagExpirationOffset = TimeSpan.FromSeconds(1),
                CompactTagSizeThreshold = 100,
                CompactTagFrequency = 100,
            };

        private static ILogger<T> GetLogger<T>()
        {
            return Mock.Of<ILogger<T>>();
        }

        private static RedisCache CreateRedisCache()
        {
            var options = CreateDefaultRedisCacheOptions();

            return new RedisCache(Options.Create(options), GetLogger<RedisCache>());
        }

        private static RedisCache CreateRedisCacheThatAlwaysCompacts()
        {
            var options = CreateDefaultRedisCacheOptions();

            options.CompactTagSizeThreshold = 0;
            options.CompactTagFrequency = 0;

            return new RedisCache(Options.Create(options), GetLogger<RedisCache>());
        }

        private static RedisCache CreateRedisCacheWithRareCompacts()
        {
            var options = CreateDefaultRedisCacheOptions();

            options.CompactTagSizeThreshold = 0;

            return new RedisCache(Options.Create(options), GetLogger<RedisCache>());
        }

        private static RedisCache CreateRedisCacheWithoutTagOffset()
        {
            IOptions<RedisCacheSettings> optionsAccessor = Options.Create(new RedisCacheSettings
            {
                Configuration = ConnectionString,
                TagExpirationOffset = TimeSpan.Zero,
                InstanceName = s_instanceName
            });

            return new RedisCache(optionsAccessor, GetLogger<RedisCache>());
        }

        private static ConnectionMultiplexer CreateConnection() =>
            ConnectionMultiplexer.Connect(ConnectionString);

        [Fact]
        public async Task Set_SetData_CacheIsSet()
        {
            // Arrange
            string key = "key1";
            var tag = "tag1";
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(GetKey(key)),
                    cache.KeyDeleteAsync(GetTag(tag)));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                redisCache.Set(key, new[] { tag }, expiry, GetRandomDataStream());
            }

            // Assert
            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                Assert.True(await cache.KeyExistsAsync(GetKey(key)));
                Assert.True(await cache.KeyExistsAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task Set_WithCompacting_TagCompacted()
        {
            // Arrange
            var key = "key1";
            var expiredKey = "key2";
            var tag = "tag1";
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(GetKey(key)),
                    cache.KeyDeleteAsync(GetKey(expiredKey)),
                    cache.KeyDeleteAsync(GetTag(tag)));
                _ = await cache.SetAddAsync(GetTag(tag), GetKey(expiredKey));
            }

            using (var redisCache = CreateRedisCacheThatAlwaysCompacts())
            {
                // Act
                redisCache.Set(key, new[] { tag }, expiry, GetRandomDataStream());
            }

            // Assert
            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                Assert.True(await cache.KeyExistsAsync(GetKey(key)));
                Assert.Equal(0, await cache.StringGetAsync(GetPack(tag)));
                Assert.Equal(1, await cache.SetLengthAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task Set_SmallTagSize_SkipCompacting()
        {
            // Arrange
            var key = "key1";
            var expiredKey = "key2";
            var tag = "tag1";
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(GetKey(key)),
                    cache.KeyDeleteAsync(GetKey(expiredKey)),
                    cache.KeyDeleteAsync(GetPack(tag)),
                    cache.KeyDeleteAsync(GetTag(tag)));
                _ = await cache.SetAddAsync(GetTag(tag), GetKey(expiredKey));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                redisCache.Set(key, new[] { tag }, expiry, GetRandomDataStream());
            }

            // Assert
            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                Assert.Equal(2, await cache.SetLengthAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task Set_TagCompactedRecently_SkipCompacting()
        {
            // Arrange
            string key = "key1";
            string expiredKey = "expired_key";
            string tag = "tag1";
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(GetKey(key)),
                    cache.KeyDeleteAsync(GetPack(tag)),
                    cache.KeyDeleteAsync(GetTag(tag)));

                using var redisCache = CreateRedisCacheThatAlwaysCompacts();
                redisCache.Set(expiredKey, new[] { tag }, expiry, GetRandomDataStream());
                _ = await cache.KeyDeleteAsync(GetKey(expiredKey));
            }

            using (var redisCache = CreateRedisCacheWithRareCompacts())
            {
                // Act
                redisCache.Set(key, new[] { tag }, expiry, GetRandomDataStream());
            }

            // Assert
            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                Assert.Equal(2, await cache.SetLengthAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task Set_SetExpired_KeyExpired()
        {
            // Arrange
            var key = "key1";
            var tag = "tag1";
            var expiry = TimeSpan.Zero;

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(GetKey(key)),
                    cache.KeyDeleteAsync(GetTag(tag)));
            }

            using (var redisCache = CreateRedisCacheWithoutTagOffset())
            {
                // Act
                redisCache.Set(key, new[] { tag }, expiry, GetRandomDataStream());
            }

            // Assert
            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                Assert.False(await cache.KeyExistsAsync(GetKey(key)));
                Assert.False(await cache.KeyExistsAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task Get_CachedKey_CachedValue()
        {
            // Arrange
            string key = "key1";
            byte[] cachedData = GetRandomData();
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().StringSetAsync(GetKey(key), cachedData, expiry);
            }

            using var redisCache = CreateRedisCache();

            // Act
            byte[]? result = redisCache.Get(key);

            // Assert
            Assert.Equal(cachedData, result);
        }

        private static MemoryStream GetRandomDataStream() => new(GetRandomData());

        private static byte[] GetRandomData()
        {
            var random = new Random();

            var data = new byte[random.Next(0, 1000)];
            random.NextBytes(data);

            return data;
        }

        [Fact]
        public async Task Get_MissingKey_NullValue()
        {
            // Arrange
            string key = "key1";

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using var redisCache = CreateRedisCache();

            // Act
            byte[]? result = redisCache.Get(key);

            // Assert
            Assert.Equal(default, result);
        }

        [Fact]
        public async Task GetOrAdd_GetByKey_CachedValue()
        {
            // Arrange
            string key = "key1";
            string[] tags = new[] { "tag1", "tag2" };
            byte[] cachedData = GetRandomData();
            MemoryStream dataFactory() => new(cachedData.Append((byte)0).ToArray());
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().StringSetAsync(GetKey(key), cachedData, expiry);
            }

            using var redisCache = CreateRedisCache();

            // Act
            byte[]? result = redisCache.GetOrAdd(key, tags, expiry, dataFactory);

            // Assert
            Assert.Equal(cachedData, result);
        }

        [Fact]
        public async Task GetOrAdd_GetByKey_RealData()
        {
            // Arrange
            string key = "key1";
            string[] tags = new[] { "tag1", "tag2" };
            byte[] realData = GetRandomData();
            MemoryStream dataFactory() => new(realData);
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using var redisCache = CreateRedisCache();

            // Act
            var result = redisCache.GetOrAdd(key, tags, expiry, dataFactory);

            // Assert
            Assert.Equal(realData, result);
        }

        [Fact]
        public async Task GetOrAdd_AddRealData_CacheIsSet()
        {
            // Arrange
            string key = "key1";
            static MemoryStream dataFactory() => GetRandomDataStream();
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                _ = redisCache.GetOrAdd(key, Array.Empty<string>(), expiry, dataFactory);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.True(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
            }
        }

        [Fact]
        public async Task GetOrAdd_SetWithExpiration_KeyExpired()
        {
            // Arrange
            var key = "key1";
            var tag = "tag1";
            var expiry = TimeSpan.Zero;
            static MemoryStream dataFactory() => GetRandomDataStream();

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(GetKey(key)),
                    cache.KeyDeleteAsync(GetTag(tag)));
            }

            using (var redisCache = CreateRedisCacheWithoutTagOffset())
            {
                // Act
                _ = redisCache.GetOrAdd(key, new[] { tag }, expiry, dataFactory);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                Assert.False(await cache.KeyExistsAsync(GetKey(key)));
                Assert.False(await cache.KeyExistsAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task GetOrAdd_ConcurrentInvalidation_CacheNotSet()
        {
            // Arrange
            string key = "key1";
            string sharedTag = "tag1";
            var expiry = TimeSpan.FromSeconds(1);
            MemoryStream dataFactory()
            {
                using var redisCache = CreateRedisCache();
                redisCache.InvalidateTag(sharedTag);

                return GetRandomDataStream();
            }

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                _ = redisCache.GetOrAdd(key, new[] { sharedTag }, expiry, dataFactory);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.False(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
            }
        }

        [Fact]
        public async Task GetOrAdd_ConcurrentDifferentTagInvalidation_CacheSet()
        {
            // Arrange
            var key = "key1";
            var tag = "tag1";
            var invalidatedTag = "tag2";
            var expiry = TimeSpan.FromSeconds(1);
            MemoryStream dataFactory()
            {
                using var redisCache = CreateRedisCache();
                redisCache.InvalidateTag(invalidatedTag);

                return GetRandomDataStream();
            }

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                _ = redisCache.GetOrAdd(key, new[] { tag }, expiry, dataFactory);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.True(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
            }
        }

        [Fact]
        public async Task InvalidateTag_WithKeys_NoTagAndKeys()
        {
            // Arrange
            string tag = "tag1";
            var keys = new[] { "key1", "key2" }.Select(GetKey).ToArray();
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await cache.KeyDeleteAsync(GetTag(tag));
                _ = await cache.SetAddAsync(GetTag(tag), keys.Select(key => new RedisValue(key)).ToArray());
                foreach (var key in keys)
                {
                    _ = await cache.StringSetAsync(key, "value", expiry);
                }
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                redisCache.InvalidateTag(tag);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                var redisKeys = keys.Select(key => new RedisKey(key)).ToArray();
                var cache = connection.GetDatabase();
                Assert.Equal(0, await cache.KeyExistsAsync(redisKeys));
                Assert.False(await cache.KeyExistsAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task InvalidateTag_WithMaxLockCount_CounterReset()
        {
            // Arrange
            string tag = "tag1";
            TimeSpan expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                await Task.WhenAll(
                    cache.KeyDeleteAsync(GetTag(tag)),
                    cache.SetAddAsync(GetTag(tag), Array.Empty<RedisValue>()),
                    cache.StringSetAsync(GetLock(tag), long.MaxValue, expiry));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                redisCache.InvalidateTag(tag);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                RedisValue incrementedLock = await connection.GetDatabase().StringGetAsync(GetLock(tag));
                Assert.Equal(1, incrementedLock);
            }
        }

        [Fact]
        public async Task Invalidate_InvalidateExisting_KeyRemoved()
        {
            // Arrange
            string key = "key1";
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().StringSetAsync(GetKey(key), "value", expiry);
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                redisCache.Invalidate(key);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.False(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
            }
        }

        [Fact]
        public async Task Invalidate_InvalidateMissingKey_Noop()
        {
            // Arrange
            string key = "key1";

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                redisCache.Invalidate(key);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.False(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
            }
        }

        [Theory]
        [MemberData(nameof(GetKeysArrays), 2, 1)]
        public async Task Exist_MultipleKeys_SomeExist(string[] existingKeys, string[] notExistingKeys)
        {
            // Arrange
            var expiry = TimeSpan.FromSeconds(1);

            using (var connection = CreateConnection())
            {
                var db = connection.GetDatabase();

                _ = await Task.WhenAll(
                    notExistingKeys.Select(key => db.KeyDeleteAsync(GetKey(key))).ToList());

                _ = await Task.WhenAll(
                    existingKeys.Select(key => db.StringSetAsync(GetKey(key), string.Empty, expiry)).ToList());
            }

            var allKeys = existingKeys.Concat(notExistingKeys);
            using var redisCache = CreateRedisCache();

            // Act
            var existResults = redisCache.Exist(allKeys);

            // Assert
            Assert.Equal(existingKeys.Length + notExistingKeys.Length, existResults.Count());
            Assert.All(existResults.Take(existingKeys.Length), isExist => Assert.True(isExist));
            Assert.All(existResults.Skip(existingKeys.Length), isExist => Assert.False(isExist));
        }

        private static IEnumerable<object[]> GetKeysArrays(int existingCount, int notExistingCount)
        {
            static string GetKeyName(int index) => $"key{index}";

            var existingKeys = Enumerable.Range(0, existingCount).Select(GetKeyName).ToArray();
            var notExistingKeys = Enumerable.Range(existingCount, existingCount + notExistingCount).Select(GetKeyName).ToArray();

            yield return new object[] { existingKeys, notExistingKeys };
            yield return new object[] { Array.Empty<string>(), notExistingKeys };
            yield return new object[] { existingKeys, Array.Empty<string>() };
        }

        private static string GetKey(string key) => $"{s_instanceName}:key:{key}";

        private static string GetTag(string tag) => $"{s_instanceName}:tag:{tag}";

        private static string GetPack(string tag) => $"{s_instanceName}:pack:{tag}";

        private static string GetLock(string tag) => new CacheKeyFactory(s_instanceName).CreateTag(tag).GetLock().ToString();
    }
}
