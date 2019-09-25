using Microsoft.Extensions.Logging;
using Moq;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets;
using System;
using System.Collections.Generic;
using Xunit;

namespace QA.DotNetCore.Engine.QpData.Tests
{
    public class SiteStructureBuilderTests
    {
        [Fact]
        public void BuildIsCorrect()
        {
            var siteId = 1;
            var isStage = false;
            var abstractItemNetName = "AI";
            var abstractItemContentId = 666;

            var metaInfoMoq = new Mock<IMetaInfoRepository>();
            metaInfoMoq.Setup(x => x.GetContent(abstractItemNetName, siteId, null)).Returns(new ContentPersistentData {
                ContentId = abstractItemContentId,
                ContentNetName = abstractItemNetName
            });

            var buildSettings = new QpSiteStructureBuildSettings
            {
                SiteId = siteId,
                IsStage = isStage,
                RootPageDiscriminator = RootPage.Discriminator,
                UploadUrlPlaceholder = "<%upload_url%>"
            };

            /*
             * простейшая структура сайта: с корнем, стартовой страницей, парой обычных страниц и виджетом
             * в реальной QP у всех элементов структуры сайта поля Discriminator/ExtensionId/IsPage являются согласованными:
             * если у 2х записей совпадает Discriminator, то совпадают и 2 других поля,
             * аналогично, у 2х записей с одним ExtensionId, совпадают 2 других поля
            */
            var aiRepositoryMoq = new Mock<IAbstractItemRepository>();
            aiRepositoryMoq.Setup(x => x.AbstractItemNetName).Returns(abstractItemNetName);
            aiRepositoryMoq.Setup(x => x.GetPlainAllAbstractItems(siteId, isStage, null)).Returns(new[] {
                new AbstractItemPersistentData{ Id = 1, Title = "корневая страница", Alias = "root", Discriminator = RootPage.Discriminator, IsPage = true, ParentId = null, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 2, Title = "стартовая страница", Alias = "start", Discriminator = StartPage.Discriminator, IsPage = true, ParentId = 1, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 3, Title = "текстовая страница 1", Alias = "foo", Discriminator = TextPage.Discriminator, IsPage = true, ParentId = 2, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 4, Title = "текстовая страница 2", Alias = "bar", Discriminator = TextPage.Discriminator, IsPage = true, ParentId = 3, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 5, Title = "некий виджет", Alias = "blah", Discriminator = TestWidget.Discriminator, IsPage = false, ParentId = 3, ExtensionId = null },
            });

            var aiFactoryMoq = new Mock<IAbstractItemFactory>();
            aiFactoryMoq.Setup(x => x.Create(It.IsAny<string>())).Returns((string d) => {
                if (d == RootPage.Discriminator) return new RootPage();
                if (d == StartPage.Discriminator) return new StartPage();
                if (d == TextPage.Discriminator) return new TextPage();
                if (d == TestWidget.Discriminator) return new TestWidget();
                throw new Exception($"type {d} is not mocked in IAbstractItemFactory");
            });

            var builder = new QpAbstractItemStorageBuilder(aiFactoryMoq.Object,
                Mock.Of<IQpUrlResolver>(),
                aiRepositoryMoq.Object,
                metaInfoMoq.Object,
                Mock.Of<IItemDefinitionRepository>(),
                buildSettings,
                Mock.Of<ILogger<QpAbstractItemStorageBuilder>>());

            var aiStorage = builder.Build();
            Assert.Equal("/foo", aiStorage.Get(3).GetTrail());
            Assert.Equal("/foo/bar", aiStorage.Get(4).GetTrail());
            Assert.Equal(2, aiStorage.GetStartPage(StartPage.DnsRegistered, null).Id);
        }
    }
}
