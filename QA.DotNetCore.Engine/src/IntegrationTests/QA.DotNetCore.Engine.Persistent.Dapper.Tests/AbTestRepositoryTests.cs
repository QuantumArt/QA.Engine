using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Settings;

namespace Tests
{
    public class AbTestRepositoryTests
    {
        private AbTestRepository _repository;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = Global.CreateMockServiceProviderWithConnection();
            var settings = CreateDefaultCacheSettings();
            var cacheProvider = new VersionedCacheCoreProvider(new MemoryCache(Options.Create(new MemoryCacheOptions())));
            var metaRepository = new MetaInfoRepository(serviceProvider);
            var sqlAnalyzer = new NetNameQueryAnalyzer(metaRepository, cacheProvider, settings);
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

        private QpSiteStructureCacheSettings CreateDefaultCacheSettings()
        {
            return new QpSiteStructureCacheSettings
            {
                QpSchemeCachePeriod = System.TimeSpan.MaxValue,
                ItemDefinitionCachePeriod = System.TimeSpan.MaxValue,
                SiteStructureCachePeriod = System.TimeSpan.MaxValue
            };
        }
    }
}
