using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class ContentModificationRepositoryTests
    {
        private MetaInfoRepository _sqlMetaRepo;
        private MetaInfoRepository _postgresMetaRepo;
        private ContentModificationRepository _sqlRepository;
        private ContentModificationRepository _postgresRepository;
        private UnitOfWork _sqlUnitOfWork;
        private UnitOfWork _postgresUnitOfWork;

        [SetUp]
        public void Setup()
        {
            _sqlUnitOfWork =
                new UnitOfWork(
                    "Initial Catalog=qa_demosite;Data Source=spbdevsql01\\dev;User ID=publishing;Password=QuantumartHost.SQL");
            _sqlRepository = new ContentModificationRepository(_sqlUnitOfWork);
            _sqlMetaRepo = new MetaInfoRepository(_sqlUnitOfWork);

            _postgresUnitOfWork =
                new UnitOfWork("Server=mscpgsql01;Port=5432;Database=qa_demosite;User Id=postgres;Password=1q2w-p=[;", "pg");
            _postgresRepository = new ContentModificationRepository(_postgresUnitOfWork);
            _postgresMetaRepo = new MetaInfoRepository(_postgresUnitOfWork);
        }

        [Test]
        public void GetAllTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var sqlContentMods = _sqlRepository.GetAll();
                var pgContentMods = _postgresRepository.GetAll();

                Assert.Positive(sqlContentMods.Count());
                Assert.Positive(pgContentMods.Count());
                Assert.True(sqlContentMods.All(cm => cm.StageModified >= cm.LiveModified));
                Assert.True(pgContentMods.All(cm => cm.StageModified >= cm.LiveModified));
            });
        }
    }
}
