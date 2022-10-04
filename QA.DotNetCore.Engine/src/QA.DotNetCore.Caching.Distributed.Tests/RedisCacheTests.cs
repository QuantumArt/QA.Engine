using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
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
using Xunit;
using Xunit.Abstractions;

namespace QA.DotNetCore.Caching.Distributed.Tests
{
    public class RedisCacheTests
    {
        private static readonly DnsEndPoint _redisEndpoint = new("SPBREDIS01.ARTQ.COM", 6407);
        private static readonly string _connectionString = $"{_redisEndpoint.Host}:{_redisEndpoint.Port}";
        private static readonly string _instanceName = "wptests:" + Guid.NewGuid().ToString();
        private static readonly TimeSpan _deprecatedCacheTtl = new RedisCacheSettings().DeprecatedCacheTimeToLive;
        private static readonly TimeSpan _defaultExpiry = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan _existingCacheTtl = _deprecatedCacheTtl + _defaultExpiry;
        private static readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(1);

        private readonly ITestOutputHelper _output;

        public RedisCacheTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Set_SetData_CacheIsSet()
        {
            // Arrange
            string key = "key1";
            var tag = "tag1";
            var expiry = _defaultExpiry;

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
            var expiry = _defaultExpiry;

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
            var expiry = _defaultExpiry;

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
            var expiry = _defaultExpiry;

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
            var expiry = TimeSpan.FromSeconds(1);

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
        public async Task Set_SetExpired_InvalidExpiration()
        {
            // Arrange
            var key = "key1";
            var tag = "tag1";
            var expiry = TimeSpan.FromSeconds(-1);

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await Task.WhenAll(
                    cache.KeyDeleteAsync(GetKey(key)),
                    cache.KeyDeleteAsync(GetTag(tag)));
            }

            using var redisCache = CreateRedisCacheWithoutOffsets();

            // Act
            _ = Assert.Throws<ArgumentOutOfRangeException>(
                () => redisCache.Set(key, new[] { tag }, expiry, GetRandomDataStream()));
        }

        [Fact]
        public async Task Set_SetDeprecated_KeyDeprecated()
        {
            // Arrange
            var key = "key1";
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
                await Task.Delay(expiry);

                var cache = connection.GetDatabase();

                var ttlTask = cache.KeyTimeToLiveAsync(GetKey(key));

                Assert.All(
                    await Task.WhenAll(
                        cache.KeyExistsAsync(GetKey(key)),
                        cache.KeyExistsAsync(GetTag(tag))),
                    Assert.True);

                Assert.True(_deprecatedCacheTtl > await ttlTask);
            }
        }

        [Fact]
        public async Task Get_CachedKey_CachedValue()
        {
            // Arrange
            string key = "key1";
            byte[] cachedData = GetRandomData();
            var expiry = _existingCacheTtl;

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

        [Fact]
        public async Task Get_CachedKey_Deprecated()
        {
            // Arrange
            string key = "key1";
            byte[] cachedData = GetRandomData();
            var expiry = _deprecatedCacheTtl;

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().StringSetAsync(GetKey(key), cachedData, expiry);
            }

            using var redisCache = CreateRedisCache();

            // Act
            byte[]? result = redisCache.Get(key);

            // Assert
            Assert.Null(result);
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
                _ = await db.KeyDeleteAsync(GetKey(key));
                _ = await db.StringSetAsync(GetKey(key), string.Empty, TimeSpan.FromMilliseconds(1));
            }

            using var redisCache = CreateRedisCache();
            await Task.Delay(1);

            // Act
            byte[]? result = redisCache.Get(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrAdd_GetByKey_CachedValue()
        {
            // Arrange
            string key = "key1";
            string[] tags = new[] { "tag1", "tag2" };
            byte[] cachedData = GetRandomData();
            var expiry = _defaultExpiry;
            MemoryStream dataFactory() => new(cachedData.Append((byte)0).ToArray());

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().StringSetAsync(GetKey(key), cachedData, _existingCacheTtl);
            }

            using var redisCache = CreateRedisCache();

            // Act
            byte[]? result = redisCache.GetOrAdd(key, tags, expiry, dataFactory, _lockTimeout);

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
            var expiry = _defaultExpiry;

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using var redisCache = CreateRedisCache();

            // Act
            var result = redisCache.GetOrAdd(key, tags, expiry, dataFactory, _lockTimeout);

            // Assert
            Assert.Equal(realData, result);
        }

        [Fact]
        public async Task GetOrAdd_AddRealData_CacheIsSet()
        {
            // Arrange
            string key = "key1";
            static MemoryStream dataFactory() => GetRandomDataStream();
            var expiry = _defaultExpiry;

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using (var redisCache = CreateRedisCache())
            {
                // Act
                _ = redisCache.GetOrAdd(key, Array.Empty<string>(), expiry, dataFactory, _lockTimeout);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.True(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
            }
        }

        [Fact]
        public async Task GetOrAdd_SetWithExpiration_InvalidExpiration()
        {
            // Arrange
            var key = "key1";
            var expiry = TimeSpan.FromSeconds(-1);
            static MemoryStream dataFactory() => GetRandomDataStream();

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
            }

            using var redisCache = CreateRedisCacheWithoutOffsets();

            _ = Assert.Throws<ArgumentOutOfRangeException>(
                () => redisCache.GetOrAdd(key, Array.Empty<string>(), expiry, dataFactory, _lockTimeout));
        }

        [Fact]
        public async Task GetOrAdd_SetWithExpiration_KeyExpiredAfterTime()
        {
            // Arrange
            var key = "key1";
            var tag = "tag1";
            var expiry = TimeSpan.FromSeconds(1);
            static MemoryStream dataFactory() => GetRandomDataStream();

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
                _ = redisCache.GetOrAdd(key, new[] { tag }, expiry, dataFactory, _lockTimeout);
            }

            await Task.Delay(expiry);

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
            var expiry = _defaultExpiry;
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
                _ = redisCache.GetOrAdd(key, new[] { sharedTag }, expiry, dataFactory, _lockTimeout);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.False(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
            }
        }

        [Fact]
        public async Task GetOrAdd_ConcurrentAdd_ReturnDeprecatedCache()
        {
            // Arrange
            string sharedKey = Guid.NewGuid().ToString();
            var deprecatedData = GetRandomData();
            var tags = Array.Empty<string>();
            var expiry = _defaultExpiry;
            AutoResetEvent dataObtainingStartedEvent = new(false);
            var eventTimeout = _lockTimeout + TimeSpan.FromSeconds(2);

            MemoryStream waitingDataFactory()
            {
                Assert.True(dataObtainingStartedEvent.Set());
                _output.WriteLine($"{DateTime.UtcNow.TimeOfDay}: Lock released.");
                Assert.True(dataObtainingStartedEvent.WaitOne());
                return GetRandomDataStream();
            }

            using (var connection = CreateConnection())
            {
                var db = connection.GetDatabase();
                _ = await db.StringSetAsync(GetKey(sharedKey), deprecatedData, _deprecatedCacheTtl);
            }

            var lockCacheTask = Task.Factory.StartNew(
                () =>
                {
                    using var otherRedisCache = CreateRedisCache();
                    _ = otherRedisCache.GetOrAdd(sharedKey, tags, expiry, waitingDataFactory, _lockTimeout);
                });

            Assert.True(dataObtainingStartedEvent.WaitOne(eventTimeout));

            using var redisCache = CreateRedisCache();

            // Act
            var value = redisCache.GetOrAdd(
                sharedKey,
                tags,
                expiry,
                () =>
                {
                    Assert.True(false, "Duplicate call of the data factory when already locked.");
                    return GetRandomDataStream();
                },
                _lockTimeout);

            Assert.True(dataObtainingStartedEvent.Set());

            // Assert
            Assert.Equal(deprecatedData, value);

            await lockCacheTask;
        }

        [Fact]
        public async Task GetOrAdd_ConcurrentAdd_ThrowsException()
        {
            // Arrange
            string sharedKey = Guid.NewGuid().ToString();
            var tags = Array.Empty<string>();
            var expiry = _defaultExpiry;
            AutoResetEvent dataObtainingStartedEvent = new(false);
            var eventTimeout = _lockTimeout + TimeSpan.FromSeconds(2);

            MemoryStream waitingDataFactory()
            {
                Assert.True(dataObtainingStartedEvent.Set());
                _output.WriteLine($"{DateTime.UtcNow.TimeOfDay}: Event released when key is locked.");
                Assert.True(dataObtainingStartedEvent.WaitOne());
                return GetRandomDataStream();
            }

            using (var connection = CreateConnection())
            {
                _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(sharedKey));
            }

            var lockCacheKeyTask = Task.Factory.StartNew(
                () =>
                {
                    using var otherRedisCache = CreateRedisCacheWithoutOffsets();
                    _ = otherRedisCache.GetOrAdd(sharedKey, tags, expiry, waitingDataFactory, _lockTimeout);
                });

            using var redisCache = CreateRedisCacheWithoutOffsets();
            Assert.True(dataObtainingStartedEvent.WaitOne(eventTimeout));

            // Assert
            _ = Assert.Throws<DeprecateCacheIsExpiredOrMissingException>(() =>
            {
                // Act
                _ = redisCache.GetOrAdd(
                    sharedKey,
                    tags,
                    expiry,
                    () =>
                    {
                        Assert.True(false, "Duplicate call of the data factory when already locked.");
                        return GetRandomDataStream();
                    }, _lockTimeout);
            });

            Assert.True(dataObtainingStartedEvent.Set());
            _output.WriteLine($"{DateTime.UtcNow.TimeOfDay}: End.");
            await lockCacheKeyTask;
            _output.WriteLine($"{DateTime.UtcNow.TimeOfDay}: Release.");
        }

        [Fact]
        public async Task GetOrAdd_ConcurrentDifferentTagInvalidation_CacheSet()
        {
            // Arrange
            var key = "key1";
            var tag = "tag1";
            var invalidatedTag = "tag2";
            var expiry = _defaultExpiry;
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
                _ = redisCache.GetOrAdd(key, new[] { tag }, expiry, dataFactory, _lockTimeout);
            }

            // Assert
            using (var connection = CreateConnection())
            {
                Assert.True(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
            }
        }

        [Fact]
        public async Task InvalidateTag_WithKeys_TagRemovedAndKeysDeprecated()
        {
            // Arrange
            string tag = "tag1";
            var keys = new[] { "key1", "key2" }.Select(GetKey).ToArray();

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                _ = await cache.KeyDeleteAsync(GetTag(tag));
                _ = await cache.SetAddAsync(GetTag(tag), keys.Select(key => new RedisValue(key)).ToArray());
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

                Assert.Equal(2, await cache.KeyExistsAsync(redisKeys));
                var keyTtls = await Task.WhenAll(redisKeys.Select(key => cache.KeyTimeToLiveAsync(key)).ToArray());
                Assert.All(keyTtls, ttl => Assert.True(ttl < _deprecatedCacheTtl));

                Assert.False(await cache.KeyExistsAsync(GetTag(tag)));
            }
        }

        [Fact]
        public async Task InvalidateTag_WithMaxLockCount_CounterReset()
        {
            // Arrange
            string tag = "tag1";

            using (var connection = CreateConnection())
            {
                var cache = connection.GetDatabase();
                await Task.WhenAll(
                    cache.KeyDeleteAsync(GetTag(tag)),
                    cache.SetAddAsync(GetTag(tag), Array.Empty<RedisValue>()),
                    cache.StringSetAsync(GetLock(tag), long.MaxValue, _existingCacheTtl));
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
        public async Task Invalidate_InvalidateExisting_KeyDeprecated()
        {
            // Arrange
            string key = "key1";
            var expiry = _defaultExpiry;

            using (var redisCache = CreateRedisCache())
            {
                redisCache.Set(key, Enumerable.Empty<string>(), expiry, GetRandomDataStream());

                // Act
                redisCache.Invalidate(key);
            }

            // Assert
            using var connection = CreateConnection();

            Assert.True(_deprecatedCacheTtl >= await connection.GetDatabase().KeyTimeToLiveAsync(GetKey(key)));
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
        [MemberData(nameof(GetKeysArrays), 2, 1, 1)]
        public async Task Exist_MultipleKeys_SomeExist(string[] existingKeys, string[] deprecatedKeys, string[] notExistingKeys)
        {
            // Arrange
            using (var connection = CreateConnection())
            {
                var db = connection.GetDatabase();

                _ = await Task.WhenAll(
                    notExistingKeys.Select(key => db.KeyDeleteAsync(GetKey(key))).ToList());

                _ = await Task.WhenAll(
                    deprecatedKeys.Select(key => db.StringSetAsync(GetKey(key), string.Empty, _deprecatedCacheTtl)).ToList());

                _ = await Task.WhenAll(
                    existingKeys.Select(key => db.StringSetAsync(GetKey(key), string.Empty, _existingCacheTtl)).ToList());
            }

            var allKeys = existingKeys
                .Concat(deprecatedKeys)
                .Concat(notExistingKeys);

            using var redisCache = CreateRedisCache();

            // Act
            var existResults = redisCache.Exist(allKeys);

            // Assert
            Assert.Equal(existingKeys.Length + deprecatedKeys.Length + notExistingKeys.Length, existResults.Count());
            Assert.All(existResults.Take(existingKeys.Length), Assert.True);
            Assert.All(existResults.Skip(existingKeys.Length), Assert.False);
        }

        private static IEnumerable<object[]> GetKeysArrays(int existingCount, int deprecatedCount, int notExistingCount)
        {
            static string GetKeyName(int index) => $"key{index}";

            var existingKeys = Enumerable.Range(0, existingCount).Select(GetKeyName).ToArray();
            var deprecatedKeys = Enumerable.Range(existingCount, deprecatedCount).Select(GetKeyName).ToArray();
            var notExistingKeys = Enumerable.Range(existingCount + deprecatedCount, notExistingCount).Select(GetKeyName).ToArray();

            yield return new object[] { existingKeys, deprecatedKeys, notExistingKeys };
            yield return new object[] { Array.Empty<string>(), deprecatedKeys, notExistingKeys };
            yield return new object[] { existingKeys, deprecatedKeys, Array.Empty<string>() };
            yield return new object[] { existingKeys, Array.Empty<string>(), notExistingKeys };
        }

        private static string GetKey(string key) => $"{_instanceName}:key:{key}";

        private static string GetTag(string tag) => $"{_instanceName}:tag:{tag}";

        private static string GetPack(string tag) => $"{_instanceName}:pack:{tag}";

        private static string GetLock(string tag) => new CacheKey(CacheKeyType.Tag, tag, _instanceName).GetLock().ToString();

        private static RedisCacheSettings CreateDefaultRedisCacheOptions() =>
            new()
            {
                Configuration = _connectionString,
                InstanceName = _instanceName,
                TagExpirationOffset = TimeSpan.FromSeconds(1),
                CompactTagSizeThreshold = 100,
                CompactTagFrequency = 100
            };

        private static ILoggerFactory CreateLoggerFactory(ITestOutputHelper output, string name)
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();

            _ = mockLoggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns<string>(_ => GetLogger(output, name));

            return mockLoggerFactory.Object;
        }

        private static ILogger GetLogger(ITestOutputHelper output, string name)
        {
            return GetLogger<ILogger>(output, name);
        }

        private static ILogger<T> GetLogger<T>(ITestOutputHelper output)
        {
            return GetLogger<ILogger<T>>(output, typeof(T).Name);
        }

        private static TLogger GetLogger<TLogger>(ITestOutputHelper output, string name)
            where TLogger : class, ILogger
        {
            var mockLogger = new Mock<TLogger>();
            _ = mockLogger
                .Setup(logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
                .Callback(new InvocationAction((invocation) => output.WriteLine(
                    $"{Environment.CurrentManagedThreadId}> [{DateTime.UtcNow.TimeOfDay} | {invocation.Arguments[0]} | {name}] " +
                    $"{invocation.Arguments[2]} {invocation.Arguments[3]}")));

            _ = mockLogger
                .Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);

            return mockLogger.Object;
        }

        private RedLockFactory CreateDefaultRedLockFactory() =>
            RedLockFactory.Create(
                new[] { new RedLockEndPoint(_redisEndpoint) },
                new RedLockRetryConfiguration(retryCount: 1),
                CreateLoggerFactory(_output, nameof(RedLock)));

        private RedisCache CreateRedisCache()
        {
            var options = CreateDefaultRedisCacheOptions();

            return new RedisCache(
                CreateDefaultRedLockFactory(),
                Options.Create(options),
                GetLogger<RedisCache>(_output));
        }

        private RedisCache CreateRedisCacheThatAlwaysCompacts()
        {
            var options = CreateDefaultRedisCacheOptions();

            options.CompactTagSizeThreshold = 0;
            options.CompactTagFrequency = 0;

            return new RedisCache(
                CreateDefaultRedLockFactory(),
                Options.Create(options),
                GetLogger<RedisCache>(_output));
        }

        private RedisCache CreateRedisCacheWithRareCompacts()
        {
            var options = CreateDefaultRedisCacheOptions();

            options.CompactTagSizeThreshold = 0;

            return new RedisCache(
                CreateDefaultRedLockFactory(),
                Options.Create(options),
                GetLogger<RedisCache>(_output));
        }

        private RedisCache CreateRedisCacheWithoutOffsets()
        {
            IOptions<RedisCacheSettings> optionsAccessor = Options.Create(new RedisCacheSettings
            {
                Configuration = _connectionString,
                TagExpirationOffset = TimeSpan.Zero,
                InstanceName = _instanceName,
                DeprecatedCacheTimeToLive = TimeSpan.Zero
            });

            return new RedisCache(
                CreateDefaultRedLockFactory(),
                optionsAccessor,
                GetLogger<RedisCache>(_output));
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
