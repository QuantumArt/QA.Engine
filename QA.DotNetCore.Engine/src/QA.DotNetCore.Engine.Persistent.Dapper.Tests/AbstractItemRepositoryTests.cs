using NUnit.Framework;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class Tests
    {
        private AbstractItemRepository _sqlRepository;
        private AbstractItemRepository _postgresRepository;
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
            _sqlRepository = new AbstractItemRepository(_sqlUnitOfWork
                , sqlAnalyzer);
            _postgresUnitOfWork =
                new UnitOfWork("Server=mscpgsql01;Port=5432;Database=qa_demosite;User Id=postgres;Password=1q2w-p=[;", "pg");
            var postgresMetaRepo = new MetaInfoRepository(_postgresUnitOfWork);
            var postgresAnalyzer = new NetNameQueryAnalyzer(postgresMetaRepo);
            _postgresRepository = new AbstractItemRepository(_postgresUnitOfWork, postgresAnalyzer);
        }

        [Test]
        public void GetAbstractItemExtensionDataTest()
        {
            Assert.DoesNotThrow(() =>
            {
                _sqlRepository.GetAbstractItemExtensionData(30751, new[] {741180, 741278}, true, false);
                _postgresRepository.GetAbstractItemExtensionData(30751, new[] {741180, 741278}, true, false);
            });
        }

        [Test]
        public void GetPlainAllAbstractItemsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                _sqlRepository.GetPlainAllAbstractItems(52, false);
                _postgresRepository.GetPlainAllAbstractItems(52, false);
            });
        }

        [Test]
        public void LoadAbstractItemExtensionTest()
        {
            Assert.DoesNotThrow(() =>
            {
                _sqlRepository.GetManyToManyData(new[] {741035}, false);
                _postgresRepository.GetManyToManyData(new[] {741035}, false);
            });
        }


    }
}
