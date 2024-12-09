using NUnit.Framework;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests;

public class ContentModificationRepositoryTests
{
    private ContentModificationRepository _repository;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = Global.CreateMockServiceProviderWithConnection();
        _repository = new ContentModificationRepository(serviceProvider, NullLogger<ContentModificationRepository>.Instance);
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
