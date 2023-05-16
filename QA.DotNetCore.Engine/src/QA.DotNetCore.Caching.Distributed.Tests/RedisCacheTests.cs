using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching.Exceptions;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tests.CommonUtils.Helpers;
using Tests.CommonUtils.Xunit.Traits;
using Xunit;
using Xunit.Abstractions;

namespace QA.DotNetCore.Caching.Distributed.Tests
{
    [Category(CategoryType.Integration)]
    public class RedisCacheTests
    {
        private const int DefaultExpiryMs = 3000;

        private static readonly DnsEndPoint _redisEndpoint = new("SPBREDIS01.ARTQ.COM", 6407);
        private static readonly string _connectionString = $"{_redisEndpoint.Host}:{_redisEndpoint.Port}";
        private static readonly string _instanceName = Guid.NewGuid().ToString();
        private static readonly string _appName = "wptests";
        private static readonly TimeSpan _defaultExpiry = TimeSpan.FromSeconds(DefaultExpiryMs);
        private static readonly TimeSpan _existingCacheTtl = _defaultExpiry;

        private readonly ITestOutputHelper _output;

        public RedisCacheTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Timeout = DefaultExpiryMs)]
        public async Task Set_SetData_CacheIsSet()
        {
            // Arrange
            string key = "key111";
            var tag = "tag111";
            var expiry = _defaultExpiry;

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(key),
                    cache.KeyDeleteAsync(tag));
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
                Assert.True(await cache.KeyExistsAsync(new RedisKey(key)));
                Assert.True(await cache.KeyExistsAsync(new RedisKey(tag)));
            }
        }


        [Fact]
        public async Task Set_SetExpired_KeyExpired()
        {
            // Arrange
            var key = "key111";
            var tag = "tag111";
            var expiry = TimeSpan.FromSeconds(3);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(GetKey(key)),
                    cache.KeyDeleteAsync(GetTag(tag)));
            }

            using (var redisCache = CreateRedisCacheWithoutOffsets())
            {
                // Act
                redisCache.Set(key, new[] { tag }, expiry, GetRandomDataStream());
            }

            // Assert
            using (var connection = CreateConnection())
            {
                await Task.Delay(expiry);

                var cache = connection.GetDatabase();
                Assert.False(await cache.KeyExistsAsync(GetKey(key)));
                Assert.False(await cache.KeyExistsAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task Set_SetDeprecated_KeyDeprecated()
        {
            // Arrange
            var key = "key1";
            var tag = "tag1";
            var expiry = TimeSpan.FromSeconds(0.5);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(key),
                    cache.KeyDeleteAsync(tag));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                redisCache.Set(key, new[] { tag }, expiry, GetRandomDataStream());
            }

            // Assert
            using (var connection = CreateConnection())
            {
                await Task.Delay(expiry);

                var cache = connection.GetDatabase();

                Assert.True(await cache.KeyExistsAsync(new RedisKey(tag)));
                Assert.False(await cache.KeyExistsAsync(new RedisKey(key)));
            }
        }

        [Fact(Timeout = DefaultExpiryMs)]
        public async Task Get_CachedKey_CachedValue()
        {
            // Arrange
            string key = "ckey1";
            byte[] cachedData = GetRandomData();
            var expiry = _existingCacheTtl;

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().StringSetAsync(key, cachedData, expiry);
            }

            using var redisCache = CreateRedisCache();

            // Act
            var result = redisCache.Get<byte[]>(new[] { key });

            // Assert
            Assert.Equal(cachedData, result.Single());
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
            var result = redisCache.Get(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Get_ExpiredKey_NullValue()
        {
            // Arrange
            string key = "key1";

            using (var connection = CreateConnection())
            {
                var db = connection.GetDatabase();
                _ = await db.KeyDeleteAsync(key);
                _ = await db.StringSetAsync(key, string.Empty, TimeSpan.FromMilliseconds(1));
            }

            using var redisCache = CreateRedisCache();
            await Task.Delay(1);

            // Act
            var result = redisCache.Get(key);

            // Assert
            Assert.Null(result);
        }

        
        [Fact(Timeout = DefaultExpiryMs)]
        public async Task InvalidateTag_WithKeys_TagAndKeysRemoved()
        {
            // Arrange
            string tag = "tag91";
            var keys = new[] { "key91", "key92" };

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await cache.KeyDeleteAsync(tag);
                _ = await cache.SetAddAsync(tag, keys.Select(key => new RedisValue(key)).ToArray());
                _ = await Task.WhenAll(keys.Select(key => cache.StringSetAsync(key, "value", _existingCacheTtl)).ToArray());
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
                Assert.False(await cache.KeyExistsAsync(tag));
            }
        }

        [Fact]
        public async Task Invalidate_InvalidateMissingKey_Noop()
        {
            // Arrange
            string key = "key1";

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(key);
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                redisCache.Invalidate(key);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.False(await connection.GetDatabase().KeyExistsAsync(key));
            }
        }

        [Theory(Timeout = DefaultExpiryMs)]
        [MemberData(nameof(GetKeysArrays), 2, 1)]
        public async Task Exist_MultipleKeys_SomeExist(string[] existingKeys, string[] notExistingKeys)
        {
            // Arrange
            var allKeys = existingKeys
                .Concat(notExistingKeys)
                .ToArray();

            using var redisCache = CreateRedisCache();

            using (var connection = CreateConnection())
            {
                var db = connection.GetDatabase();

                _ = await Task.WhenAll(
                    notExistingKeys.Select(key => db.KeyDeleteAsync(key)).ToList());

 
                _ = await Task.WhenAll(
                    existingKeys.Select(key => db.StringSetAsync(key, string.Empty, _existingCacheTtl)).ToList());
            }

            // Act
            var existResults = redisCache.Exist(allKeys);

            // Assert
            Assert.Equal(existingKeys.Length + notExistingKeys.Length, existResults.Count());
            Assert.All(existResults.Take(existingKeys.Length), Assert.True);
            Assert.All(existResults.Skip(existingKeys.Length), Assert.False);
        }

        public static IEnumerable<object[]> GetKeysArrays(int existingCount, int notExistingCount)
        {
            static string GetKeyName(int index) => $"key{index}";

            var existingKeys = Enumerable.Range(0, existingCount).Select(GetKeyName).ToArray();
            var notExistingKeys = Enumerable.Range(existingCount, notExistingCount).Select(GetKeyName).ToArray();

            yield return new object[] { existingKeys,  notExistingKeys };
            yield return new object[] { Array.Empty<string>(), notExistingKeys };
            yield return new object[] { existingKeys, Array.Empty<string>() };
            yield return new object[] { existingKeys, notExistingKeys };
        }

        private static string GetKey(string key) => new CacheKey(CacheKeyType.Key, key, _appName, _instanceName).ToString();

        private static string GetTag(string tag) => new CacheKey(CacheKeyType.Tag, tag, _appName, _instanceName).ToString();

        private static string GetLock(string key) => new CacheKey(CacheKeyType.Lock, key, _appName, _instanceName).ToString();

        private static RedisCacheSettings CreateDefaultRedisCacheOptions() =>
            new()
            {
                Configuration = _connectionString,
                AppName = _instanceName,
                TagExpirationOffset = TimeSpan.FromSeconds(1),
            };

        private RedLockFactory CreateDefaultRedLockFactory() =>
            RedLockFactory.Create(
                new[] { new RedLockEndPoint(_redisEndpoint) },
                new RedLockRetryConfiguration(retryCount: 1),
                LoggerUtils.CreateLoggerFactory(_output, nameof(RedLock)));

        private RedisCache CreateRedisCache()
        {
            var options = CreateDefaultRedisCacheOptions();

            return new RedisCache(
                Options.Create(options),
                LoggerUtils.GetLogger<RedisCache>(_output));
        }

        private RedisCache CreateRedisCacheWithoutOffsets()
        {
            IOptions<RedisCacheSettings> optionsAccessor = Options.Create(new RedisCacheSettings
            {
                Configuration = _connectionString,
                TagExpirationOffset = TimeSpan.Zero,
                AppName = _instanceName,
            });

            return new RedisCache(
                optionsAccessor,
                LoggerUtils.GetLogger<RedisCache>(_output));
        }

        private static ConnectionMultiplexer CreateConnection() =>
            ConnectionMultiplexer.Connect(_connectionString);

        private static MemoryStream GetRandomDataStream() => new(GetRandomData());

        private static byte[] GetRandomData()
        {
            var random = new Random();

            var data = new byte[random.Next(0, 1000)];
            random.NextBytes(data);

            return data;
        }
    }
}
