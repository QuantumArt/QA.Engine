using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class ItemDefinitionRepositoryTests
    {
        private ItemDefinitionRepository _sqlRepository;
        private ItemDefinitionRepository _postgresRepository;
        private UnitOfWork _sqlUnitOfWork;
        private UnitOfWork _postgresUnitOfWork;

        [SetUp]
        public void Setup()
        {
            _sqlUnitOfWork =
                new UnitOfWork(
                    "Initial Catalog=qa_demosite;Data Source=spbdevsql01\\dev;User ID=publishing;Password=QuantumartHost.SQL");
            var sqlMetaRepo = new MetaInfoRepository(_sqlUnitOfWork);
            var sqlAnalyzer = new NetNameQueryAnalyzer(sqlMetaRepo);
            _sqlRepository = new ItemDefinitionRepository(_sqlUnitOfWork, sqlAnalyzer);
            _postgresUnitOfWork =
                new UnitOfWork("Server=mscpgsql01;Port=5432;Database=qa_demosite;User Id=postgres;Password=1q2w-p=[;", "pg");
            var postgresMetaRepo = new MetaInfoRepository(_postgresUnitOfWork);
            var postgresAnalyzer = new NetNameQueryAnalyzer(postgresMetaRepo);
            _postgresRepository = new ItemDefinitionRepository(_postgresUnitOfWork, postgresAnalyzer);
        }

        [Test]
        public void GetAllItemDefinitionsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetAllItemDefinitions(52, false);
                var pgSql = _postgresRepository.GetAllItemDefinitions(52, false);
                Assert.AreEqual(msSql.Count(), pgSql.Count());
            });
        }
    }
}
