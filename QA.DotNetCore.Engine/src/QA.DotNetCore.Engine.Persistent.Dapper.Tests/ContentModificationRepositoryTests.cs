using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class ContentModificationRepositoryTests
    {
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
            _postgresUnitOfWork =
                new UnitOfWork("Server=mscpgsql01;Port=5432;Database=qa_demosite;User Id=postgres;Password=1q2w-p=[;", "pg");
            _postgresRepository = new ContentModificationRepository(_postgresUnitOfWork);
        }

        [Test]
        public void GetAllTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetAll();
                var pgSql = _postgresRepository.GetAll();
                Assert.AreEqual(msSql.Count(), pgSql.Count());
            });
        }
    }
}
