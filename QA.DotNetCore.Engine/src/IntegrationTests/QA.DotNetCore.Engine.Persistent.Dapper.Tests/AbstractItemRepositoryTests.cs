using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Settings;

namespace Tests
{
    public class AbstractItemRepositoryTests
    {
        private AbstractItemRepository _repository;
        private MetaInfoRepository _metaRepo;

        private QpSiteStructureCacheSettings CreateDefaultCacheSettings()
        {
            return new QpSiteStructureCacheSettings
            {
                QpSchemeCachePeriod = System.TimeSpan.MaxValue,
                ItemDefinitionCachePeriod = System.TimeSpan.MaxValue,
                SiteStructureCachePeriod = System.TimeSpan.MaxValue
            };
        }

        [SetUp]
        public void Setup()
        {
            var serviceProvider = Global.CreateMockServiceProviderWithConnection();
            var settings = CreateDefaultCacheSettings();
            var cacheProvider = new VersionedCacheCoreProvider(new MemoryCache(Options.Create(new MemoryCacheOptions())));
            _metaRepo = new MetaInfoRepository(serviceProvider);
            var sqlAnalyzer = new NetNameQueryAnalyzer(_metaRepo, cacheProvider, settings);
            _repository = new AbstractItemRepository(serviceProvider, sqlAnalyzer, new StubNamingProvider(), cacheProvider, settings);
        }

        [Test]
        public void GetAbstractItemExtensionDataTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var startPageId = 741114;

                var baseContent = _metaRepo.GetContent("QPAbstractItem", Global.SiteId);
                var sqlExtData = _repository.GetAbstractItemExtensionData(547, new[] { startPageId }, baseContent, true, false);//получим данные о extension c типом StartPageExtension с id=startPageId
                Assert.True(sqlExtData.ContainsKey(startPageId));
                Assert.IsNotNull(sqlExtData[startPageId].Get("Bindings", typeof(string)));
            });
        }

        [Test]
        public void GetPlainAllAbstractItemsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                _repository.GetPlainAllAbstractItems(52, false);
            });
        }

        [Test]
        public void LoadAbstractItemExtensionTest()
        {
            Assert.DoesNotThrow(() =>
            {
                _repository.GetManyToManyData(new[] { 741035 }, false);
            });
        }
    }
}
