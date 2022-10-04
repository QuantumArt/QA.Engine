using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System;
using System.Net;
using System.Threading;
using Tests.CommonUtils.Helpers;
using Tests.CommonUtils.Xunit.Traits;
using Xunit;
using Xunit.Abstractions;

namespace QA.DotNetCore.Caching.Distributed.Tests;

public class DistributedMemoryCacheProviderTests
{
    private static readonly DnsEndPoint s_redisEndpoint = new("SPBREDIS01.ARTQ.COM", 6407);
    private static readonly string ConnectionString = $"{s_redisEndpoint.Host}:{s_redisEndpoint.Port}";
    private static readonly string s_instanceName = "wptests:" + Guid.NewGuid().ToString();

    private readonly ITestOutputHelper _output;
    private readonly MockRepository _mockRepository;

    public DistributedMemoryCacheProviderTests(ITestOutputHelper output)
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);
        _output = output;
    }

    [Fact]
    [Category(CategoryType.Integration)]
    public void Add_NewCacheValue_ValueExist()
    {
        // Arrange
        var cacheProvider = CreateRedisCacheProvider();
        var provider = CreateProvider(cacheProvider);

        string key = "key";
        int value = 1;
        string[] tags = Array.Empty<string>();
        TimeSpan expiration = TimeSpan.FromSeconds(5);

        // Act
        provider.Add(value, key, tags, expiration);

        // Assert
        var result = provider.Get<int>(key);
        Assert.Equal(value, result);
        _mockRepository.VerifyAll();
    }

    [Fact]
    [Category(CategoryType.Integration)]
    public void Add_ChangeExistingValueOnSiblingInstance_ValueChagnedForEveryone()
    {
        // Arrange
        var cacheProvider = CreateRedisCacheProvider();

        var firstNodeProvider = CreateProvider(cacheProvider);
        var secondNodeProvider = CreateProvider(cacheProvider);

        string key = "key";
        int originalValue = 1;
        int changedValue = 2;
        string[] tags = Array.Empty<string>();
        TimeSpan expiration = TimeSpan.FromSeconds(5);

        firstNodeProvider.Add(originalValue, key, tags, expiration);

        // Act
        secondNodeProvider.Add(changedValue, key, tags, expiration);

        // Assert
        Assert.False(firstNodeProvider.IsSet(key));

        var secondNodeValue = secondNodeProvider.Get<int>(key);
        Assert.Equal(changedValue, secondNodeValue);

        _mockRepository.VerifyAll();
    }

    private IMemoryCache CreateMemoryCache() =>
        new MemoryCache(
            Options.Create(new MemoryCacheOptions()),
            LoggerUtils.CreateLoggerFactory(_output, $"MemoryCache.{Guid.NewGuid()}"));

    private static RedisCacheSettings CreateDefaultRedisCacheOptions() =>
        new()
        {
            Configuration = ConnectionString,
            InstanceName = s_instanceName,
            TagExpirationOffset = TimeSpan.FromSeconds(1),
            CompactTagSizeThreshold = 100,
            CompactTagFrequency = 100
        };

    private RedLockFactory CreateDefaultRedLockFactory() =>
        RedLockFactory.Create(
            new[] { new RedLockEndPoint(s_redisEndpoint) },
            new RedLockRetryConfiguration(retryCount: 1),
            LoggerUtils.CreateLoggerFactory(_output, nameof(RedLock)));

    private RedisCache CreateRedisCache()
    {
        var options = CreateDefaultRedisCacheOptions();

        return new RedisCache(
            CreateDefaultRedLockFactory(),
            Options.Create(options),
            LoggerUtils.GetLogger<RedisCache>(_output));
    }

    private RedisCacheProvider CreateRedisCacheProvider() =>
        new(
            CreateRedisCache(),
            LoggerUtils.GetLogger<RedisCacheProvider>(_output));

    private INodeIdentifier CreateNodeIndentifier()
    {
        var mockIdentifier = _mockRepository.Create<INodeIdentifier>();

        _ = mockIdentifier
            .Setup(id => id.GetUniqueId(It.IsAny<CancellationToken>()))
            .Returns(Guid.NewGuid().ToString());

        return mockIdentifier.Object;
    }

    private DistributedMemoryCacheProvider CreateProvider<TCacheProvider>(
        TCacheProvider cacheProvider)
        where TCacheProvider : class, IDistributedCacheProvider, ICacheInvalidator
    {
        var memoryCache = CreateMemoryCache();
        var nodeIdentifier = CreateNodeIndentifier();

        return new DistributedMemoryCacheProvider(
            memoryCache,
            cacheProvider,
            cacheProvider,
            nodeIdentifier,
            LoggerUtils.GetLogger<DistributedMemoryCacheProvider>(_output));
    }
}
