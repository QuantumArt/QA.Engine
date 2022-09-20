using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using System;
using System.Linq;

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
                Mock.Of<ILogger>());
            var metaRepository = new MetaInfoRepository(serviceProvider, cacheProvider, settings);
            var sqlAnalyzer = new NetNameQueryAnalyzer(metaRepository);
            _repository = new ItemDefinitionRepository(serviceProvider, sqlAnalyzer, new StubNamingProvider(), cacheProvider, settings);
        }

        [Test]
        public void GetAllItemDefinitionsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var defs = _repository.GetAllItemDefinitions(Global.SiteId, false);

                var startPageDef = defs.FirstOrDefault(d => d.Discriminator.Equals("start_page", StringComparison.OrdinalIgnoreCase));

                Assert.IsNotNull(startPageDef);
                Assert.AreEqual(startPageDef.TypeName, "StartPage");
            });
        }
    }
}
