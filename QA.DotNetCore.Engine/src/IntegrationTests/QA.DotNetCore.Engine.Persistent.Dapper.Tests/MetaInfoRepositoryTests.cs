using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests;

public class MetaInfoRepositoryTests
{
    private MetaInfoRepository _repository;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = Global.CreateMockServiceProviderWithConnection();
        var settings = TestUtils.CreateDefaultCacheSettings();
        var cacheProvider = new VersionedCacheCoreProvider(
            new MemoryCache(Options.Create(new MemoryCacheOptions())),
            new CacheKeyFactoryBase(),
            Mock.Of<ILogger>());

        _repository = new MetaInfoRepository(serviceProvider, cacheProvider, settings);
    }

    [Test]
    public void GetSiteTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var siteInfo = _repository.GetSite(Global.SiteId);
            Assert.IsNotEmpty(siteInfo.UploadUrlPrefix);
        });
    }

    [Test]
    public void GetContentTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var contentInfo = _repository.GetContent("QPDiscriminator", Global.SiteId);
            Assert.AreEqual(contentInfo.ContentNetName, "QPDiscriminator");
        });
    }

    [Test]
    public void GetContentAttributeTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var contentInfo = _repository.GetContent("QPDiscriminator", Global.SiteId);
            var fieldInfo = _repository.GetContentAttribute(contentInfo.ContentId, "Description");
            Assert.AreEqual(fieldInfo.ColumnName, "Description");
        });
    }

    [Test]
    public void GetContentAttributeByNetNameTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var contentInfo = _repository.GetContent("QPDiscriminator", Global.SiteId);
            var fieldInfo = _repository.GetContentAttributeByNetName(contentInfo.ContentId, "Description");
            Assert.AreEqual(fieldInfo.NetName, "Description");
        });
    }

}
