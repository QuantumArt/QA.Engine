using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class ItemDefinitionRepositoryTests
    {
        private ItemDefinitionRepository _repository;
        private UnitOfWork _connection;

        [SetUp]
        public void Setup()
        {
            _connection = Global.CreateConnection;
            var metaRepository = new MetaInfoRepository(_connection);
            var sqlAnalyzer = new NetNameQueryAnalyzer(metaRepository);
            _repository = new ItemDefinitionRepository(_connection, sqlAnalyzer);
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
    }
}
