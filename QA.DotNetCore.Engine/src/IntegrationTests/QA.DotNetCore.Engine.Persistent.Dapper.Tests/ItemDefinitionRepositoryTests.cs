using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Settings;
using System.Linq;

namespace Tests
{
    public class ItemDefinitionRepositoryTests
    {
        private ItemDefinitionRepository _repository;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = Global.CreateMockServiceProviderWithConnection();
            var settings = CreateDefaultCacheSettings();
            var cacheProvider = new VersionedCacheCoreProvider(new MemoryCache(Options.Create(new MemoryCacheOptions())));
            var metaRepository = new MetaInfoRepository(serviceProvider);
            var sqlAnalyzer = new NetNameQueryAnalyzer(metaRepository, cacheProvider, settings);
            _repository = new ItemDefinitionRepository(serviceProvider, sqlAnalyzer, new StubNamingProvider(), cacheProvider, settings);
        }

        [Test]
        public void GetAllItemDefinitionsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var defs = _repository.GetAllItemDefinitions(Global.SiteId, false);

                var startPageDef = defs.FirstOrDefault(d => d.Discriminator.Equals("start_page", System.StringComparison.InvariantCultureIgnoreCase));

                Assert.IsNotNull(startPageDef);
                Assert.AreEqual(startPageDef.TypeName, "StartPage");
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
