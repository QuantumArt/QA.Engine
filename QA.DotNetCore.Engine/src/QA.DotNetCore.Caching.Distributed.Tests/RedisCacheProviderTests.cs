using ICSharpCode.SharpZipLib.GZip;
using Moq;
using Newtonsoft.Json;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Tests.CommonUtils.Helpers;
using Tests.CommonUtils.Xunit.Traits;
using Xunit;
using Xunit.Abstractions;

namespace QA.DotNetCore.Caching.Distributed.Tests;

[Category(CategoryType.Unit)]
public class RedisCacheProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly MockRepository _mockRepository;
    private readonly Mock<IDistributedTaggedCache> _mockDistributedTaggedCache;

    public RedisCacheProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _mockRepository = new MockRepository(MockBehavior.Strict);
        _mockDistributedTaggedCache = _mockRepository.Create<IDistributedTaggedCache>();
    }

    [Theory]
    [InlineData(new object[] { new[] { "key" }, new[] { "1" } })]
    [InlineData(new object[] { new[] { "key1", "key2" }, new[] { "1", "2" } })]
    [InlineData(new object[] { new[] { "key1", "key1" }, new[] { "1", "1" } })]
    [InlineData(new object[] { new string[0], new string[0] })]
    public void Get_ExistingKeys_FoundValues(string[] keys, string[] values)
    {
        // Arrange
        var provider = CreateProvider();

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Get(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(GetValues);

        // Act
        var results = provider.Get<string>(keys).ToArray();

        // Assert
        Assert.Equal(values, results);

        _mockRepository.VerifyAll();

        IEnumerable<byte[]> GetValues(IEnumerable<string> requestedKeys, CancellationToken _)
        {
            return requestedKeys.Select((key) =>
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].Equals(key, StringComparison.Ordinal))
                    {
                        return Serialize(values[i]);
                    }
                }

                return Array.Empty<byte>();
            });
        }
    }

    [Theory]
    [InlineData(new object[] { new[] { "key" } })]
    [InlineData(new object[] { new[] { "key1", "key2" } })]
    [InlineData(new object[] { new[] { "key1", "key1" } })]
    [InlineData(new object[] { new string[0] })]
    public void Get_MissingKeys_NullValues(string[] keys)
    {
        // Arrange
        var provider = CreateProvider();

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Get(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(GetValues);

        // Act
        var results = provider.Get<string>(keys).ToArray();

        // Assert
        Assert.Equal(keys.Length, results.Length);
        Assert.All(results, Assert.Null);

        _mockRepository.VerifyAll();

        static IEnumerable<byte[]> GetValues(IEnumerable<string> requestedKeys, CancellationToken _) =>
            requestedKeys.Select((key) => Array.Empty<byte>());
    }

    [Fact]
    public void Get_ExistingValueByWrongType_NullValue()
    {
        // Arrange
        const string value = "value";
        var provider = CreateProvider();

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Get(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { Serialize(value) });

        // Act
        var results = provider.Get<int>(new[] { "key" });

        // Assert
        var result = Assert.Single(results);
        Assert.Equal(default, result);

        _mockRepository.VerifyAll();
    }

    [Theory]
    [InlineData(new object[] { new[] { "key" } })]
    [InlineData(new object[] { new[] { "key1", "key2" } })]
    [InlineData(new object[] { new string[0] })]
    public void IsSet_ExistingKey_IsExist(string[] keys)
    {
        // Arrange
        var provider = CreateProvider();

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Exist(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(Enumerable.Repeat(true, keys.Length));

        // Act
        var results = provider.IsSet(keys);

        // Assert
        Assert.All(results, Assert.True);
        _mockRepository.VerifyAll();
    }

    [Theory]
    [InlineData(new object[] { new[] { "key" } })]
    [InlineData(new object[] { new[] { "key1", "key2" } })]
    [InlineData(new object[] { new string[0] })]
    public void IsSet_MissingKey_NotExist(string[] keys)
    {
        // Arrange
        var provider = CreateProvider();

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Exist(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(Enumerable.Repeat(false, keys.Length));

        // Act
        var results = provider.IsSet(keys);

        // Assert
        Assert.All(results, Assert.False);
        _mockRepository.VerifyAll();
    }

    [Fact]
    public void TryGetValue_ExceptionOnGet_ValueNotFound()
    {
        // Arrange
        var provider = CreateProvider();
        string key = "key";

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Get(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Throws<Exception>();

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
        var provider = CreateProvider();
        string key = "key";
        DateTime value = DateTime.UtcNow;

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Get(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { Serialize(value) });

        // Act
        var isFound = provider.TryGetValue<DateTime>(key, out var result);

        // Assert
        Assert.True(isFound);
        Assert.Equal(value, result);
        _mockRepository.VerifyAll();
    }

    [Fact]
    public void TryGetValue_MissingKey_NotFoundValue()
    {
        // Arrange
        var provider = CreateProvider();
        string key = "key";
        DateTime value = DateTime.UtcNow;

        _ = _mockDistributedTaggedCache
            .Setup(cache => cache.Get(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { Array.Empty<byte>() });

        // Act
        var isFound = provider.TryGetValue<DateTime>(key, out var result);

        // Assert
        Assert.False(isFound);
        _mockRepository.VerifyAll();
    }

    private RedisCacheProvider CreateProvider()
    {
        return new RedisCacheProvider(
            _mockDistributedTaggedCache.Object,
            LoggerUtils.GetLogger<RedisCacheProvider>(_output));
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
}
