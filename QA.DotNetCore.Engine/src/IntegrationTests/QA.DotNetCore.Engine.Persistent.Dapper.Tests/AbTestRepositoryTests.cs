using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class AbTestRepositoryTests
    {
        private AbTestRepository _repository;
        private UnitOfWork _connection;

        [SetUp]
        public void Setup()
        {
            _connection = Global.CreateConnection;
            var metaRepository = new MetaInfoRepository(_connection);
            var sqlAnalyzer = new NetNameQueryAnalyzer(metaRepository);
            _repository = new AbTestRepository(_connection, sqlAnalyzer);
        }

        [Test]
        public void GetAllTestsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var tests = _repository.GetAllTests(Global.SiteId, false);
            });
        }

        [Test]
        public void GetActiveTestsTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var tests = _repository.GetActiveTests(Global.SiteId, false);
            });
        }

        [Test]
        public void GetAllTestsContainersTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var containers = _repository.GetAllTestsContainers(Global.SiteId, false);
            });
        }

        [Test]
        public void GetActiveTestsContainersTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var containers = _repository.GetActiveTestsContainers(Global.SiteId, false);
            });
        }

    }
}
