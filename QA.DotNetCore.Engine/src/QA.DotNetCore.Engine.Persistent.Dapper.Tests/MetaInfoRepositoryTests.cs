using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class MetaInfoRepositoryTests
    {
        private MetaInfoRepository _sqlRepository;
        private MetaInfoRepository _postgresRepository;
        private UnitOfWork _sqlUnitOfWork;
        private UnitOfWork _postgresUnitOfWork;

        [SetUp]
        public void Setup()
        {
            _sqlUnitOfWork =
                new UnitOfWork(
                    "Initial Catalog=qa_demosite;Data Source=spbdevsql01\\dev;User ID=publishing;Password=QuantumartHost.SQL");
            _sqlRepository = new MetaInfoRepository(_sqlUnitOfWork);
            _postgresUnitOfWork =
                new UnitOfWork("Server=mscpgsql01;Port=5432;Database=qa_demosite;User Id=postgres;Password=1q2w-p=[;", "pg");
            _postgresRepository = new MetaInfoRepository(_postgresUnitOfWork);
        }

        [Test]
        public void GetSiteTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetSite(52);
                var pgSql = _postgresRepository.GetSite(52);
                Assert.IsNotEmpty(msSql.UploadUrlPrefix);
                Assert.IsNotEmpty(pgSql.UploadUrlPrefix);
            });
        }

        [Test]
        public void GetContentTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetContent("QPDiscriminator",52);
                var pgSql = _postgresRepository.GetContent("QPDiscriminator",52);
                Assert.AreEqual(msSql.ContentId, pgSql.ContentId);
            });
        }

        [Test]
        public void GetContentAttributeTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetContentAttribute(537,"Description");
                var pgSql = _postgresRepository.GetContentAttribute(537,"Description");
                Assert.AreEqual(msSql.Id, pgSql.Id);
            });
        }

        [Test]
        public void GetContentAttributeByNetNameTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var msSql = _sqlRepository.GetContentAttributeByNetName(537,"Description");
                var pgSql = _postgresRepository.GetContentAttribute(537,"Description");
                Assert.AreEqual(msSql.Id, pgSql.Id);
            });
        }

    }
}
