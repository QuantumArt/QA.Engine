using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class AbTestRepositoryTests
    {
        private AbTestRepository _sqlRepository;
        private AbTestRepository _postgresRepository;
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
            _sqlRepository = new AbTestRepository(_sqlUnitOfWork,sqlAnalyzer);
            _postgresUnitOfWork =
                new UnitOfWork("Server=mscpgsql01;Port=5432;Database=qa_demosite;User Id=postgres;Password=1q2w-p=[;", "pg");
            var postgresMetaRepo = new MetaInfoRepository(_postgresUnitOfWork);
            var postgresAnalyzer = new NetNameQueryAnalyzer(postgresMetaRepo);
            _postgresRepository = new AbTestRepository(_postgresUnitOfWork, postgresAnalyzer);
        }

        [Test]
        public void GetAllTestsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetAllTests(52, false);
                var pgSql = _postgresRepository.GetAllTests(52, false);
            });
        }

        [Test]
        public void GetActiveTestsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetActiveTests(52, false);
                var pgSql = _postgresRepository.GetActiveTests(52, false);
            });
        }

        [Test]
        public void GetAllTestsContainersTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetAllTestsContainers(52, false);
                var pgSql = _postgresRepository.GetAllTestsContainers(52, false);
            });
        }

        [Test]
        public void GetActiveTestsContainersTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetActiveTestsContainers(52, false);
                var pgSql = _postgresRepository.GetActiveTestsContainers(52, false);
            });
        }

    }
}
