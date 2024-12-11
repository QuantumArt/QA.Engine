using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests
{
    public class ItemDefinitionRepositoryTests
    {
        private ItemDefinitionRepository _repository;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = Global.CreateMockServiceProviderWithConnection();
            var settings = TestUtils.CreateDefaultCacheSettings();
            var cacheProvider = new VersionedCacheCoreProvider(
                new MemoryCache(Options.Create(new MemoryCacheOptions())),
                new CacheKeyFactoryBase(),
                new MemoryLockFactory(new LoggerFactory()),
                new LoggerFactory());
            var metaRepository = new MetaInfoRepository(serviceProvider, cacheProvider, settings, NullLogger<MetaInfoRepository>.Instance);
            var sqlAnalyzer = new NetNameQueryAnalyzer(metaRepository);
            _repository = new ItemDefinitionRepository(serviceProvider, sqlAnalyzer, new StubNamingProvider(),
                cacheProvider, settings, NullLogger<ItemDefinitionRepository>.Instance);
        }

        [Test]
        public void GetAllItemDefinitionsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var defs = _repository.GetAllItemDefinitions(Global.SiteId, false);

                var startPageDef = defs.FirstOrDefault(d =>
                    d.Discriminator.Equals("start_page", StringComparison.OrdinalIgnoreCase));

                Assert.That(startPageDef, Is.Not.Null);
                Assert.That(startPageDef.TypeName, Is.EqualTo("StartPage"));
            });
        }
    }
}
