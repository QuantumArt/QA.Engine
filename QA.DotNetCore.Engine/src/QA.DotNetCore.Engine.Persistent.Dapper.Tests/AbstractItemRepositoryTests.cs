using NUnit.Framework;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class AbstractItemRepositoryTests
    {
        private MetaInfoRepository _sqlMetaRepo;
        private MetaInfoRepository _postgresMetaRepo;
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
            _sqlMetaRepo = new MetaInfoRepository(_sqlUnitOfWork);
            var sqlAnalyzer = new NetNameQueryAnalyzer(_sqlMetaRepo);
            _sqlRepository = new AbstractItemRepository(_sqlUnitOfWork, sqlAnalyzer, _postgresMetaRepo);
            _postgresUnitOfWork =
                new UnitOfWork("Server=mscpgsql01;Port=5432;Database=qa_demosite;User Id=postgres;Password=1q2w-p=[;", "pg");
            _postgresMetaRepo = new MetaInfoRepository(_postgresUnitOfWork);
            var postgresAnalyzer = new NetNameQueryAnalyzer(_postgresMetaRepo);
            _postgresRepository = new AbstractItemRepository(_postgresUnitOfWork, postgresAnalyzer, _postgresMetaRepo);
        }

        [Test]
        public void GetAbstractItemExtensionDataTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var startPageId = 741114;

                var sqlBaseContent = _sqlMetaRepo.GetContent("QPAbstractItem", 52);//получим инфу о контенте AbstractItem
                var sqlExtData = _sqlRepository.GetAbstractItemExtensionData(547, new[] { startPageId }, sqlBaseContent, true, false);//получим данные о extension c типом StartPageExtension с id=startPageId
                Assert.True(sqlExtData.ContainsKey(startPageId));
                Assert.IsNotNull(sqlExtData[startPageId].Get("Bindings", typeof(string)));
                
                var pgBaseContent = _postgresMetaRepo.GetContent("QPAbstractItem", 52);
                var pgExtData = _postgresRepository.GetAbstractItemExtensionData(547, new[] { startPageId }, pgBaseContent, true, false);
                Assert.True(pgExtData.ContainsKey(startPageId));
                Assert.IsNotNull(pgExtData[startPageId].Get("Bindings", typeof(string)));
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
