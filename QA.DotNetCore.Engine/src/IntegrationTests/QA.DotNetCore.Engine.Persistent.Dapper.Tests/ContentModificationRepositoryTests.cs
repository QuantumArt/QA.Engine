using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using System.Linq;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests;

public class ContentModificationRepositoryTests
{
    private ContentModificationRepository _repository;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = Global.CreateMockServiceProviderWithConnection();
        _repository = new ContentModificationRepository(serviceProvider);
    }

    [Test]
    public void GetAllTest()
    {
        Assert.DoesNotThrow(() =>
        {
            var allContentMods = _repository.GetAll();
            Assert.That(allContentMods.Count(), Is.Positive);
            Assert.That(allContentMods.All(cm => cm.StageModified >= cm.LiveModified), Is.True);

            var abstractItemChanged = allContentMods.FirstOrDefault(m => m.ContentName == "AbstractItem");
            Assert.That(abstractItemChanged, Is.Not.Null);
        });
    }
}
