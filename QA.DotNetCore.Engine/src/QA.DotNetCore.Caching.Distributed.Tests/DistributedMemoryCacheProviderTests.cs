using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using QA.DotNetCore.Caching.Distributed.Internals;
using QA.DotNetCore.Caching.Distributed.Keys;
using QA.DotNetCore.Caching.Exceptions;
using QA.DotNetCore.Engine.QpData;
using StackExchange.Redis;
using Tests.CommonUtils.Helpers;
using Tests.CommonUtils.Xunit.Traits;
using Xunit;
using Xunit.Abstractions;

namespace QA.DotNetCore.Caching.Distributed.Tests;

public class DistributedMemoryCacheProviderTests
{
    private const int DefaultExpiryMs = 3000;
    private static readonly TimeSpan _defaultExpiry = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan _existingCacheTtl = _defaultExpiry;
    private static readonly DnsEndPoint s_redisEndpoint = new("mscredis01.artq.com", 6382);
    private static readonly string ConnectionString = $"{s_redisEndpoint.Host}:{s_redisEndpoint.Port}";
    private static readonly string _instanceName = Guid.NewGuid().ToString();
    private static readonly string _appName = "wptests";
    private static readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(1);

    private readonly ITestOutputHelper _output;
    private readonly MockRepository _mockRepository;
    private readonly Mock<IExternalCache> _mockDistributedTaggedCache;

    public DistributedMemoryCacheProviderTests(ITestOutputHelper output)
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);
        _output = output;
        _mockDistributedTaggedCache = _mockRepository.Create<IExternalCache>();
    }

    [Fact]
    [Category(CategoryType.Integration)]
    public void Add_NewCacheValue_ValueExist()
    {
        // Arrange
        var provider = CreateProvider(CreateRedisCache());

        string key = "key";
        int value = 1;
        string[] tags = Array.Empty<string>();
        TimeSpan expiration = TimeSpan.FromSeconds(5);

        // Act
        provider.Add(value, key, tags, expiration, false);

        // Assert
        var result = provider.Get<int>(key);
        Assert.Equal(value, result);
        _mockRepository.VerifyAll();
    }

    [Fact]
    [Category(CategoryType.Integration)]
    public void Add_ChangeExistingValueOnSiblingInstance_ValueChangedForEveryone()
    {
        // Arrange

        var redisCache = CreateRedisCache();
        var firstNodeProvider = CreateProvider(redisCache);
        var secondNodeProvider = CreateProvider(redisCache);

        string key = "key";
        int originalValue = 1;
        int changedValue = 2;
        string[] tags = Array.Empty<string>();
        TimeSpan expiration = TimeSpan.FromSeconds(5);

        firstNodeProvider.Add(originalValue, key, tags, expiration, false);

        // Act
        secondNodeProvider.Add(changedValue, key, tags, expiration, false);

        // Assert
        Assert.True(firstNodeProvider.IsSet(key));

        var firstNodeValue = firstNodeProvider.Get<int>(key);
        var secondNodeValue = secondNodeProvider.Get<int>(key);
        Assert.Equal(changedValue, secondNodeValue);
        //TODO: Add versions while rewriting keys
        //Assert.Equal(changedValue, firstNodeValue);

        _mockRepository.VerifyAll();
    }

    [Fact(Timeout = DefaultExpiryMs)]
    public async Task GetOrAdd_GetByKey_CachedValue()
    {
        // Arrange
        string key = "key1";
        string[] tags = new[] {"tag1", "tag2"};
        byte[] cachedData = GetRandomData();
        var expiry = _defaultExpiry;
        var dataFactory = () => cachedData;

        using (var connection = CreateConnection())
        {
            _ = await connection.GetDatabase().StringSetAsync(GetKey(key), cachedData, _existingCacheTtl);
        }

        var provider = CreateProvider(CreateRedisCache());

        // Act
        byte[]? result = provider.GetOrAdd(key, tags, expiry, dataFactory, _lockTimeout);

        // Assert
        Assert.Equal(cachedData, result);
    }

    [Fact]
    public async Task GetOrAdd_GetByKey_RealData()
    {
        // Arrange
        string key = "key31";
        string[] tags = new[] {"tag311", "tag312"};
        byte[] realData = GetRandomData();
        var dataFactory = () => realData;
        var expiry = _defaultExpiry;

        using (var connection = CreateConnection())
        {
            _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
        }

        var provider = CreateProvider(CreateRedisCache());

        // Act
        var result = provider.GetOrAdd(key, tags, expiry, dataFactory, _lockTimeout);

        // Assert
        Assert.Equal(realData, result);
    }

    [Fact]
    public async Task GetOrAdd_AddRealData_CacheIsSet()
    {
        // Arrange
        string key = "key1";
        var expiry = _defaultExpiry;

        using (var connection = CreateConnection())
        {
            _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
        }

        var provider = CreateProvider(CreateRedisCache());
        {
            // Act
            _ = provider.GetOrAdd(key, Array.Empty<string>(), expiry, GetRandomData, _lockTimeout);
        }

        // Assert
        using (var connection = CreateConnection())
        {
            Assert.True(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
        }
    }

    [Fact]
    public async Task GetOrAdd_SetWithExpiration_KeyExpiredAfterTime()
    {
        // Arrange
        var key = "key1";
        var tag = "tag1";
        var expiry = TimeSpan.FromSeconds(0.5);
        var extraDelay = TimeSpan.FromSeconds(1);

        using (var connection = CreateConnection())
        {
            var cache = connection.GetDatabase();
            _ = await Task.WhenAll(
                cache.KeyDeleteAsync(GetKey(key)),
                cache.KeyDeleteAsync(GetTag(tag)));
        }

        var provider = CreateProvider(CreateRedisCache());
        {
            // Act
            _ = provider.GetOrAdd(key, new[] {tag}, expiry, GetRandomData, _lockTimeout);
        }

        await Task.Delay(expiry + extraDelay);

        // Assert
        using (var connection = CreateConnection())
        {
            var cache = connection.GetDatabase();
            Assert.False(await cache.KeyExistsAsync(GetKey(key)));
            Assert.False(await cache.KeyExistsAsync(GetTag(tag)));
        }
    }

    [Fact]
    public async Task GetOrAdd_ConcurrentAdd_ReturnDeprecatedCache()
    {
        // Arrange
        string sharedKey = "shared";
        var deprecatedData = GetRandomData();
        var tags = Array.Empty<string>();
        var expiry = _defaultExpiry;
        AutoResetEvent dataObtainingStartedEvent = new(false);
        var eventTimeout = _lockTimeout + TimeSpan.FromSeconds(2);

        byte[] waitingDataFactory()
        {
            Assert.True(dataObtainingStartedEvent.Set());
            _output.WriteLine($"{DateTime.UtcNow.TimeOfDay}: Lock released.");
            Assert.True(dataObtainingStartedEvent.WaitOne());
            return GetRandomData();
        }

        using (var connection = CreateConnection())
        {
            var key2 = GetKey(sharedKey + "__Deprecated");
            var db = connection.GetDatabase();
            _ = await db.StringSetAsync(key2, deprecatedData, _existingCacheTtl);
        }

        var lockFactory = new MemoryLockFactory(new LoggerFactory());
        var lockCacheTask = Task.Factory.StartNew(
            () =>
            {
                var provider = CreateProvider(CreateRedisCache(), lockFactory);
                _ = provider.GetOrAdd(sharedKey, tags, expiry, waitingDataFactory, _lockTimeout);
            });

        Assert.True(dataObtainingStartedEvent.WaitOne(eventTimeout));

        var provider = CreateProvider(CreateRedisCache(), lockFactory);

        // Act
        var value = provider.GetOrAdd(
            sharedKey,
            tags,
            expiry,
            () =>
            {
                Assert.True(false, "Duplicate call of the data factory when already locked.");
                return GetRandomData();
            },
            _lockTimeout);

        Assert.True(dataObtainingStartedEvent.Set());

        // Assert
        Assert.Equal(deprecatedData, value);

        await lockCacheTask;
    }

    [Fact(Timeout = DefaultExpiryMs)]
    public async Task GetOrAdd_ConcurrentAdd_ThrowsException()
    {
        // Arrange
        string sharedKey = Guid.NewGuid().ToString();
        var tags = Array.Empty<string>();
        var expiry = _defaultExpiry;
        AutoResetEvent dataObtainingStartedEvent = new(false);
        var eventTimeout = _lockTimeout + TimeSpan.FromSeconds(2);

        byte[] waitingDataFactory()
        {
            Assert.True(dataObtainingStartedEvent.Set());
            _output.WriteLine($"{DateTime.UtcNow.TimeOfDay}: Event released when key is locked.");
            Assert.True(dataObtainingStartedEvent.WaitOne());
            return GetRandomData();
        }

        using (var connection = CreateConnection())
        {
            _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(sharedKey));
        }

        var lockFactory = new MemoryLockFactory(new LoggerFactory());

        var lockCacheKeyTask = Task.Factory.StartNew(
            () =>
            {
                var provider = CreateProvider(CreateRedisCacheWithoutOffsets(), lockFactory);
                _ = provider.GetOrAdd(sharedKey, tags, expiry, waitingDataFactory, _lockTimeout);
            });

        var provider = CreateProvider(CreateRedisCacheWithoutOffsets(), lockFactory);
        Assert.True(dataObtainingStartedEvent.WaitOne(eventTimeout));

        // Assert
        _ = Assert.Throws<DeprecateCacheIsExpiredOrMissingException>(() =>
        {
            // Act
            _ = provider.GetOrAdd(
                sharedKey,
                tags,
                expiry,
                () =>
                {
                    Assert.True(false, "Duplicate call of the data factory when already locked.");
                    return GetRandomData();
                },
                _lockTimeout);
        });

        Assert.True(dataObtainingStartedEvent.Set());
        _output.WriteLine($"{DateTime.UtcNow.TimeOfDay}: End.");
        await lockCacheKeyTask;
        _output.WriteLine($"{DateTime.UtcNow.TimeOfDay}: Release.");
    }

    [Fact(Timeout = DefaultExpiryMs)]
    public async Task GetOrAdd_ConcurrentDifferentTagInvalidation_CacheSet()
    {
        // Arrange
        var key = "key1";
        var tag = "tag1";
        var invalidatedTag = "tag2";
        var expiry = _defaultExpiry;

        byte[] dataFactory()
        {
            using var redisCache = CreateRedisCache();
            redisCache.InvalidateTag(invalidatedTag);

            return GetRandomData();
        }

        using (var connection = CreateConnection())
        {
            _ = await connection.GetDatabase().KeyDeleteAsync(GetKey(key));
        }

        var provider = CreateProvider(CreateRedisCache());
        {
            // Act
            _ = provider.GetOrAdd(key, new[] {tag}, expiry, dataFactory, _lockTimeout);
        }

        // Assert
        using (var connection = CreateConnection())
        {
            Assert.True(await connection.GetDatabase().KeyExistsAsync(GetKey(key)));
        }
    }

    [Theory]
    [InlineData(new object[] {new[] {"key"}, new[] {"1"}})]
    [InlineData(new object[] {new[] {"key1", "key2"}, new[] {"1", "2"}})]
    [InlineData(new object[] {new[] {"key1", "key1"}, new[] {"1", "1"}})]
    public void Get_ExistingKeys_FoundValues(string[] keys, string[] values)
    {
        // Arrange
        _mockDistributedTaggedCache
            .Setup(cache => cache.Get<string>(It.IsAny<IEnumerable<string>>()))
            .Returns(GetValues);

        _mockDistributedTaggedCache
            .Setup(cache => cache.Exist(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(GetFlags);

        var provider = CreateProvider(_mockDistributedTaggedCache.Object);

        // Act
        var results = provider.Get<string>(keys).ToArray();

        // Assert
        Assert.Equal(values, results);

        _mockRepository.VerifyAll();

        IEnumerable<string> GetValues(IEnumerable<string> requestedKeys)
        {
            return requestedKeys.Select((key) =>
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (GetKey(keys[i]).Equals(key, StringComparison.Ordinal))
                    {
                        return values[i];
                    }
                }

                return null;
            });
        }

        static IEnumerable<bool> GetFlags(IEnumerable<string> requestedKeys, CancellationToken _) =>
            requestedKeys.Select(_ => true);
    }

    [Theory]
    [InlineData(new object[] {new[] {"key"}})]
    [InlineData(new object[] {new[] {"key1", "key2"}})]
    [InlineData(new object[] {new[] {"key1", "key1"}})]
    public void Get_MissingKeys_NullValues(string[] keys)
    {
        _mockDistributedTaggedCache
            .Setup(cache => cache.Get<string>(It.IsAny<IEnumerable<string>>())).Returns(GetValues);
        _mockDistributedTaggedCache
            .Setup(cache => cache.Exist(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(GetFlags);
        // Arrange
        var provider = CreateProvider(_mockDistributedTaggedCache.Object);

        // Act
        var results = provider.Get<string>(keys).ToArray();

        // Assert
        Assert.Equal(keys.Length, results.Length);
        Assert.All(results, Assert.Null);

        _mockRepository.VerifyAll();

        static IEnumerable<string> GetValues(IEnumerable<string> requestedKeys) =>
            requestedKeys.Select(_ => (string) null);

        static IEnumerable<bool> GetFlags(IEnumerable<string> requestedKeys, CancellationToken _) =>
            requestedKeys.Select(_ => false);
    }


    [Theory]
    [InlineData(new object[] {new[] {"key"}})]
    [InlineData(new object[] {new[] {"key1", "key2"}})]
    public void IsSet_ExistingKey_IsExist(string[] keys)
    {
        _mockDistributedTaggedCache
            .Setup(cache => cache.Exist(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(Enumerable.Repeat(true, keys.Length));
        // Arrange
        var provider = CreateProvider(_mockDistributedTaggedCache.Object);


        // Act
        var results = provider.IsSet(keys);

        // Assert
        Assert.All(results, Assert.True);
        _mockRepository.VerifyAll();
    }

    [Theory]
    [InlineData(new object[] {new[] {"key"}})]
    [InlineData(new object[] {new[] {"key1", "key2"}})]
    public void IsSet_MissingKey_NotExist(string[] keys)
    {
        // Arrange
        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Exist(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(Enumerable.Repeat(false, keys.Length));

        var provider = CreateProvider(_mockDistributedTaggedCache.Object);

        // Act
        var results = provider.IsSet(keys);

        // Assert
        Assert.All(results, Assert.False);
        _mockRepository.VerifyAll();
    }

    [Fact]
    public void TryGetValue_MissingKey_NotFoundValue()
    {
        // Arrange

        string key = "key";

        _mockDistributedTaggedCache
            .Setup(cache => cache.Exist(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>())
            ).Returns(new[] {false});

        var provider = CreateProvider(_mockDistributedTaggedCache.Object);

        // Act
        var isFound = provider.TryGetValue<string>(key, out var result);

        // Assert
        Assert.False(isFound);
        _mockRepository.VerifyAll();
    }

    [Fact]
    public void TryGetValue_ExistingKey_FoundValue()
    {
        // Arrange

        string key = "key";
        DateTime value = DateTime.UtcNow;
        _mockDistributedTaggedCache
            .Setup(cache => cache.Exist(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>())
            ).Returns(new[] {true});

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Get<DateTime>(It.IsAny<IEnumerable<string>>()))
            .Returns(new[] {value});

        var provider = CreateProvider(_mockDistributedTaggedCache.Object);

        // Act
        var isFound = provider.TryGetValue<DateTime>(key, out var result);

        // Assert
        Assert.True(isFound);
        Assert.Equal(value, result);
        _mockRepository.VerifyAll();
    }

    private static byte[] Serialize(object value)
    {
        byte[] rawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));

        using var memoryStream = new MemoryStream();
        using var gZipStream = new GZipOutputStream(memoryStream);

        gZipStream.Write(rawData);
        gZipStream.Finish();

        return memoryStream.GetBuffer();
    }

    private IMemoryCache CreateMemoryCache() =>
        new MemoryCache(
            Options.Create(new MemoryCacheOptions()),
            LoggerUtils.CreateLoggerFactory(_output, $"MemoryCache.{Guid.NewGuid()}"));

    private static RedisCacheSettings CreateDefaultRedisCacheOptions() =>
        new()
        {
            Configuration = ConnectionString,
            AppName = _appName,
            InstanceName = _instanceName,
            TagExpirationOffset = TimeSpan.FromSeconds(1),
        };

    private RedLockFactory CreateDefaultRedLockFactory() =>
        RedLockFactory.Create(
            new[] {new RedLockEndPoint(s_redisEndpoint)},
            new RedLockRetryConfiguration(retryCount: 1),
            LoggerUtils.CreateLoggerFactory(_output, nameof(RedLock)));

    private RedisCache CreateRedisCache()
    {
        var options = CreateDefaultRedisCacheOptions();

        return new RedisCache(Options.Create(options), NullLogger<RedisCache>.Instance);
    }

    private RedisCache CreateRedisCacheWithoutOffsets()
    {
        IOptions<RedisCacheSettings> optionsAccessor = Options.Create(new RedisCacheSettings
        {
            Configuration = ConnectionString,
            TagExpirationOffset = TimeSpan.Zero,
            AppName = _instanceName,
        });

        return new RedisCache(optionsAccessor, NullLogger<RedisCache>.Instance);
    }

    private DistributedMemoryCacheProvider CreateProvider(
        IExternalCache cache, ILockFactory? lockFactory = null)

    {
        var memoryCache = CreateMemoryCache();

        return new DistributedMemoryCacheProvider(
            memoryCache,
            cache,
            new ExternalCacheKeyFactory(new ExternalCacheSettings() {AppName = _appName, InstanceName = _instanceName}),
            lockFactory ?? new MemoryLockFactory(new LoggerFactory()),
            new LoggerFactory());
    }

    private static ConnectionMultiplexer CreateConnection() =>
        ConnectionMultiplexer.Connect(ConnectionString);

    private static byte[] GetRandomData()
    {
        var random = new Random();

        var data = new byte[random.Next(0, 1000)];
        random.NextBytes(data);

        return data;
    }

    private static string GetKey(string key) => new CacheKey(CacheKeyType.Key, key, _appName, _instanceName).ToString();

    private static string GetTag(string tag) => new CacheKey(CacheKeyType.Tag, tag, _appName, _instanceName).ToString();

    private static string GetLock(string key) =>
        new CacheKey(CacheKeyType.Lock, key, _appName, _instanceName).ToString();
}
