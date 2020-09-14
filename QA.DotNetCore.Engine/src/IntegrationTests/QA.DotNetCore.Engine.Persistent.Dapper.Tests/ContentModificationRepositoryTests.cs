using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace Tests
{
    public class ContentModificationRepositoryTests
    {
        private ContentModificationRepository _repository;
        private UnitOfWork _connection;

        [SetUp]
        public void Setup()
        {
            _connection = Global.CreateConnection;
            _repository = new ContentModificationRepository(_connection);
        }

        [Test]
        public void GetAllTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var allContentMods = _repository.GetAll();
                Assert.Positive(allContentMods.Count());
                Assert.True(allContentMods.All(cm => cm.StageModified >= cm.LiveModified));

                var abstractItemChanged = allContentMods.FirstOrDefault(m => m.ContentName == "AbstractItem");
                Assert.NotNull(abstractItemChanged);
            });
        }
    }
}
