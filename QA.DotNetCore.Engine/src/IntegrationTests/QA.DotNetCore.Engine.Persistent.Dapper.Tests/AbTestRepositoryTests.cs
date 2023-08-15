using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests;

public class AbTestRepositoryTests
{
    private AbTestRepository _repository;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = Global.CreateMockServiceProviderWithConnection();
        var settings = TestUtils.CreateDefaultCacheSettings();
        var cacheProvider = new VersionedCacheCoreProvider(
            new MemoryCache(Options.Create(new MemoryCacheOptions())),
            new CacheKeyFactoryBase(),
            new MemoryLockFactory(NullLoggerFactory.Instance.CreateLogger<MemoryLockFactory>()),
            Mock.Of<ILogger>());
        var metaRepository = new MetaInfoRepository(serviceProvider, cacheProvider, settings);
        var sqlAnalyzer = new NetNameQueryAnalyzer(metaRepository);
        _repository = new AbTestRepository(serviceProvider, sqlAnalyzer);
    }

    [Test]
    public void GetAllTestsTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var tests = _repository.GetAllTests(Global.SiteId, false);
        });
    }

    [Test]
    public void GetActiveTestsTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var tests = _repository.GetActiveTests(Global.SiteId, false);
        });
    }

    [Test]
    public void GetAllTestsContainersTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var containers = _repository.GetAllTestsContainers(Global.SiteId, false);
        });
    }

    [Test]
    public void GetActiveTestsContainersTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var containers = _repository.GetActiveTestsContainers(Global.SiteId, false);
        });
    }
}
