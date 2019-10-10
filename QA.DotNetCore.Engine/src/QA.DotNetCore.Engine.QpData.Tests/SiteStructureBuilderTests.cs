using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace QA.DotNetCore.Engine.QpData.Tests
{
    /// <summary>
    /// Тесты на корректность построения структуры сайта
    /// </summary>
    public class SiteStructureBuilderTests
    {
        const int siteId = 1;
        const bool isStage = false;
        const string abstractItemNetName = "AI";
        const int abstractItemContentId = 666;
        const string uploadUrlPlaceholder = "<%upload_url%>";
        readonly QpSiteStructureBuildSettings buildSettings = new QpSiteStructureBuildSettings
        {
            SiteId = siteId,
            IsStage = isStage,
            RootPageDiscriminator = typeof(RootPage).Name,
            UploadUrlPlaceholder = uploadUrlPlaceholder
        };
        readonly QpSiteStructureCacheSettings cacheSettings = new QpSiteStructureCacheSettings
        {
            ItemDefinitionCachePeriod = TimeSpan.FromSeconds(30),
            QpSchemeCachePeriod = TimeSpan.FromSeconds(30),
            SiteStructureCachePeriod = TimeSpan.FromSeconds(30)
        };
        readonly QpSitePersistentData siteData = new QpSitePersistentData
        {
            UseAbsoluteUploadUrl = true,
            UploadUrlPrefix = "http://storage.quntumart.ru",
            UploadUrl = "/upload"
        };

        [Fact]
        public void GeneralBuildIsCorrect()
        {
            /*
             * тестовая простейшая структура сайта: с корнем, стартовой страницей, парой обычных страниц и виджетом
             * для одной из страниц специально задан дискриминатор о котором не знает IAbstractItemFactory, такие страницы не попадают в структуру сайта
            */
            var aiRepositoryMoq = new Mock<IAbstractItemRepository>();
            aiRepositoryMoq.Setup(x => x.AbstractItemNetName).Returns(abstractItemNetName);

            var aiArray = new[] {
                new AbstractItemPersistentData{ Id = 4, Title = "текстовая страница 2", Alias = "bar", Discriminator = typeof(StubPage).Name, IsPage = true, ParentId = 3, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 2, Title = "стартовая страница", Alias = "start", Discriminator = typeof(StubStartPage).Name, IsPage = true, ParentId = 1, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 5, Title = "некий виджет", Alias = "blah", Discriminator = typeof(StubWidget).Name, IsPage = false, ParentId = 3, ExtensionId = null, IndexOrder = 100, ZoneName = "zonename" },
                new AbstractItemPersistentData{ Id = 1, Title = "корневая страница", Alias = "root", Discriminator = typeof(RootPage).Name, IsPage = true, ParentId = null, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 3, Title = "текстовая страница 1", Alias = "foo", Discriminator = typeof(StubPage).Name, IsPage = true, ParentId = 2, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 6, Title = "несуществующая страница 1", Alias = "bar2", Discriminator = "notexisting", IsPage = true, ParentId = 3, ExtensionId = null }
            };
            aiRepositoryMoq.Setup(x => x.GetPlainAllAbstractItems(siteId, isStage, null)).Returns(aiArray);

            //фабрика элементов структуры сайта
            var aiFactoryMoq = new Mock<IAbstractItemFactory>();
            aiFactoryMoq.Setup(x => x.Create(It.IsAny<string>())).Returns((string d) => {
                if (d == typeof(RootPage).Name) return new RootPage();
                if (d == typeof(StubStartPage).Name) return new StubStartPage();
                if (d == typeof(StubPage).Name) return new StubPage();
                if (d == typeof(StubWidget).Name) return new StubWidget();
                return null;
            });

            var builder = new QpAbstractItemStorageBuilder(aiFactoryMoq.Object,
                Mock.Of<IQpUrlResolver>(),
                aiRepositoryMoq.Object,
                Mock.Of<IMetaInfoRepository>(),
                Mock.Of<IItemDefinitionRepository>(),
                buildSettings,
                Mock.Of<ILogger<QpAbstractItemStorageBuilder>>());

            var aiStorage = builder.Build();

            //проверим корень
            Assert.NotNull(aiStorage.Root);
            Assert.Equal(1, aiStorage.Root.Id);

            //проверим получение по id
            Assert.NotNull(aiStorage.Get(3));
            Assert.NotNull(aiStorage.Get(4));
            Assert.Null(aiStorage.Get(6));
            Assert.Null(aiStorage.Get(100));

            //проверим типы
            Assert.IsType<RootPage>(aiStorage.Get(1));
            Assert.IsType<StubStartPage>(aiStorage.Get(2));
            Assert.IsType<StubPage>(aiStorage.Get(3));
            Assert.IsType<StubPage>(aiStorage.Get(4));
            Assert.IsType<StubWidget>(aiStorage.Get(5));

            //проверим формирование trail для страниц
            Assert.Equal("/foo", aiStorage.Get(3).GetTrail());
            Assert.Equal("/foo/bar", aiStorage.Get(4).GetTrail());

            //проверим стартовую страницу
            var startPage = aiStorage.GetStartPage(StubStartPage.DnsRegistered, null);
            Assert.NotNull(startPage);
            Assert.Equal(2, startPage.Id);

            //проверим, что все базовые поля присваиваются корректно (на примере виджета)
            var widget = aiStorage.Get(5) as IAbstractWidget;
            var aiWidget = aiArray.First(a => a.Id == 5);
            Assert.Equal(aiWidget.Title, widget.Title);
            Assert.Equal(aiWidget.Alias, widget.Alias);
            Assert.NotNull(widget.Parent);
            Assert.Equal(aiWidget.ParentId, widget.Parent.Id);
            Assert.Equal(aiWidget.IndexOrder, widget.SortOrder);
            Assert.Equal(aiWidget.ZoneName, widget.ZoneName);
            Assert.Equal("", widget.GetTrail());

            //проверим получение дочерних элементов по алиасу
            Assert.NotNull(startPage.Get("foo"));
            Assert.Equal(3, startPage.Get("foo").Id);
            Assert.NotNull(startPage.Get("foo").Get("bar"));
            Assert.Equal(4, startPage.Get("foo").Get("bar").Id);
            Assert.Null(startPage.Get("xxx"));
            Assert.Null(startPage.Get("foo").Get("bar2"));
        }

        [Fact]
        public void ExtensionsIsCorrect()
        {
            var extensionId = 777;
            var aiRepositoryMoq = new Mock<IAbstractItemRepository>();
            aiRepositoryMoq.Setup(x => x.AbstractItemNetName).Returns(abstractItemNetName);

            //корневая и 2 стартовых страницы, тип стартовой страницы подразумевает extension поля
            var aiArray = new[] {
                new AbstractItemPersistentData{ Id = 1, Title = "корневая страница", Alias = "root", Discriminator = typeof(RootPage).Name, IsPage = true, ParentId = null, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 2, Title = "стартовая страница 1", Alias = "start1", Discriminator = typeof(StartPage).Name, IsPage = true, ParentId = 1, ExtensionId = extensionId },
                new AbstractItemPersistentData{ Id = 3, Title = "стартовая страница 2", Alias = "start2", Discriminator = typeof(StartPage).Name, IsPage = true, ParentId = 1, ExtensionId = extensionId }
            };
            aiRepositoryMoq.Setup(x => x.GetPlainAllAbstractItems(siteId, isStage, null)).Returns(aiArray);

            //extension-поля стартовых страниц
            var startPageBaseExt = new AbstractItemExtensionCollection();
            startPageBaseExt.Add("DnsBinding", "quantumart.ru");
            var startPageJobExt = new AbstractItemExtensionCollection();
            startPageJobExt.Add("DnsBinding", "job.quantumart.ru");
            var startPageExtDictionary = new Dictionary<int, AbstractItemExtensionCollection>
            {
                { 2, startPageBaseExt},
                { 3, startPageJobExt}
            };
            aiRepositoryMoq.Setup(x => x.GetAbstractItemExtensionData(extensionId,
                It.Is<IEnumerable<int>>(ids => ids.Count() == 2 && ids.Contains(2) && ids.Contains(3)),
                It.IsAny<ContentPersistentData>(),
                buildSettings.LoadAbstractItemFieldsToDetailsCollection,
                buildSettings.IsStage,
                null)).Returns(startPageExtDictionary);

            //фабрика элементов структуры сайта
            var aiFactoryMoq = new Mock<IAbstractItemFactory>();
            aiFactoryMoq.Setup(x => x.Create(It.IsAny<string>())).Returns((string d) => {
                if (d == typeof(RootPage).Name) return new RootPage();
                if (d == typeof(StartPage).Name) return new StartPage();
                return null;
            });

            var builder = new QpAbstractItemStorageBuilder(aiFactoryMoq.Object,
                Mock.Of<IQpUrlResolver>(),
                aiRepositoryMoq.Object,
                Mock.Of<IMetaInfoRepository>(),
                Mock.Of<IItemDefinitionRepository>(),
                buildSettings,
                Mock.Of<ILogger<QpAbstractItemStorageBuilder>>());

            var aiStorage = builder.Build();

            //проверим, что стартовые страницы создались и в extension-полях у них то, что ожидается
            Assert.IsType<StartPage>(aiStorage.Get(2));
            Assert.IsType<StartPage>(aiStorage.Get(3));
            Assert.Equal("quantumart.ru", (aiStorage.Get(2) as StartPage).DnsBinding);
            Assert.Equal("job.quantumart.ru", (aiStorage.Get(3) as StartPage).DnsBinding);
        }

        [Fact]
        public void LibraryUrlFieldsIsCorrect()
        {
            var extensionId = 777;

            //замокаем информацию о сайте, базовом контенте, и полях с картинками (одно в базовом контенте, одно в контенте-расширении),
            //т.к. эта информация понадобится для создания урлов, которые участвуют в тесте
            var metaInfoMoq = new Mock<IMetaInfoRepository>();
            metaInfoMoq.Setup(x => x.GetSite(siteId, null)).Returns(siteData);
            metaInfoMoq.Setup(x => x.GetContent(abstractItemNetName, siteId, null)).Returns(new ContentPersistentData
            {
                ContentId = abstractItemContentId
            });
            metaInfoMoq.Setup(x => x.GetContentAttribute(extensionId, It.Is<string>(s => s.ToUpper() == "Image".ToUpper()), null)).Returns(new ContentAttributePersistentData
            {
                ContentId = extensionId,
                ColumnName = "Image",
                UseSiteLibrary = false,
                SubFolder = "subfolder"
            });
            metaInfoMoq.Setup(x => x.GetContentAttribute(abstractItemContentId, It.Is<string>(s => s.ToUpper() == "Icon".ToUpper()), null)).Returns(new ContentAttributePersistentData
            {
                ContentId = abstractItemContentId,
                ColumnName = "Icon",
                UseSiteLibrary = true,
                SubFolder = "subfolder"
            });

            var aiRepositoryMoq = new Mock<IAbstractItemRepository>();
            aiRepositoryMoq.Setup(x => x.AbstractItemNetName).Returns(abstractItemNetName);

            //корневая, стартовая страница и виджет с картинкой
            var aiArray = new[] {
                new AbstractItemPersistentData{ Id = 1, Title = "корневая страница", Alias = "root", Discriminator = typeof(RootPage).Name, IsPage = true, ParentId = null, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 2, Title = "стартовая страница", Alias = "start", Discriminator = typeof(StubStartPage).Name, IsPage = true, ParentId = 1, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 3, Title = "виджет картинки", Alias = "pic", Discriminator = typeof(PictureWidget).Name, IsPage = false, ParentId = 2, ExtensionId = extensionId }
            };
            aiRepositoryMoq.Setup(x => x.GetPlainAllAbstractItems(siteId, isStage, null)).Returns(aiArray);

            //extension-поля, среди них:
            //поля с картинками (которые приходят к нам без базового урла)
            //поле с плейсхолдером, который должен быть подменен на некий базовый урл
            var widgetExt = new AbstractItemExtensionCollection();
            widgetExt.Add("Image", "1.img");
            widgetExt.Add("Icon", "123.png");
            widgetExt.Add("Description", $"blah blah {uploadUrlPlaceholder}/blah.");
            var widgetExtDictionary = new Dictionary<int, AbstractItemExtensionCollection>
            {
                { 3, widgetExt}
            };
            aiRepositoryMoq.Setup(x => x.GetAbstractItemExtensionData(extensionId,
                It.Is<IEnumerable<int>>(ids => ids.Count() == 1 && ids.Contains(3)),
                It.IsAny<ContentPersistentData>(),
                buildSettings.LoadAbstractItemFieldsToDetailsCollection,
                buildSettings.IsStage,
                null)).Returns(widgetExtDictionary);

            //фабрика элементов структуры сайта
            var aiFactoryMoq = new Mock<IAbstractItemFactory>();
            aiFactoryMoq.Setup(x => x.Create(It.IsAny<string>())).Returns((string d) => {
                if (d == typeof(RootPage).Name) return new RootPage();
                if (d == typeof(StubStartPage).Name) return new StubStartPage();
                if (d == typeof(PictureWidget).Name) return new PictureWidget();
                return null;
            });

            var cache = new MemoryCache(new MemoryCacheOptions());
            var cacheProvider = new VersionedCacheCoreProvider(cache);
            var urlResolver = new QpUrlResolver(cacheProvider, metaInfoMoq.Object, cacheSettings);
            var builder = new QpAbstractItemStorageBuilder(aiFactoryMoq.Object,
                urlResolver, // реальный urlresolver
                aiRepositoryMoq.Object,
                metaInfoMoq.Object,
                Mock.Of<IItemDefinitionRepository>(),
                buildSettings,
                Mock.Of<ILogger<QpAbstractItemStorageBuilder>>());

            var aiStorage = builder.Build();

            var widget = aiStorage.Get(3) as PictureWidget;
            Assert.NotNull(widget);

            //проверим, что поля, помеченные атрибутом LibraryUrl обогатились и стали полной ссылкой на картинку
            Assert.StartsWith(urlResolver.UrlForImage(siteId, extensionId, "Image"), widget.ImageUrl);
            Assert.EndsWith("1.img", widget.ImageUrl);
            Assert.StartsWith(urlResolver.UrlForImage(siteId, abstractItemContentId, "Icon"), widget.Icon);
            Assert.EndsWith("123.png", widget.Icon);

            //проверим, что в поле, содержащее плейсхолдер адреса библиотеки сайта подставился реальный адрес 
            Assert.Contains(urlResolver.UploadUrl(siteId), widget.Description);
            Assert.DoesNotContain(uploadUrlPlaceholder, widget.Description);
        }

        [Fact]
        public void ManyToManyFieldsIsCorrect()
        {
            buildSettings.LoadM2mForAbstractItem = true;
            var extensionId = 777;
            var widgetExtId = 987348;
            var relationId = 7645;
            var relationValues = new[] { 435, 46, 56 };
            var baseContentRelationId = 435;
            var baseContentRelationValues = new[] { 576, 7568 };
            var widgetId = 3;

            var aiRepositoryMoq = new Mock<IAbstractItemRepository>();
            aiRepositoryMoq.Setup(x => x.AbstractItemNetName).Returns(abstractItemNetName);

            //корневая, стартовая страница и виджет с m2m (виджет просто для примера, m2m может быть и у страницы)
            var aiArray = new[] {
                new AbstractItemPersistentData{ Id = 1, Title = "корневая страница", Alias = "root", Discriminator = typeof(RootPage).Name, IsPage = true, ParentId = null, ExtensionId = null },
                new AbstractItemPersistentData{ Id = 2, Title = "стартовая страница", Alias = "start", Discriminator = typeof(StubStartPage).Name, IsPage = true, ParentId = 1, ExtensionId = null },
                new AbstractItemPersistentData{ Id = widgetId, Title = "виджет m2m", Alias = "m2m", Discriminator = typeof(ManyToManyWidget).Name, IsPage = false, ParentId = 2, ExtensionId = extensionId }
            };
            aiRepositoryMoq.Setup(x => x.GetPlainAllAbstractItems(siteId, isStage, null)).Returns(aiArray);

            //extension-поля виджета:
            //поле SomeRelations - это поле m2m(значением будет является некий relationid),
            //CONTENT_ITEM_ID - это id самого виджета
            //extension-поле из базового контента: поле BaseContentRelations - тоже m2m
            var widgetExt = new AbstractItemExtensionCollection();
            widgetExt.Add("CONTENT_ITEM_ID", widgetExtId);
            widgetExt.Add("SomeRelations", relationId);
            widgetExt.Add("BaseContentRelations", baseContentRelationId);
            var widgetExtDictionary = new Dictionary<int, AbstractItemExtensionCollection>
            {
                { widgetId, widgetExt}
            };
            aiRepositoryMoq.Setup(x => x.GetAbstractItemExtensionData(extensionId,
                It.Is<IEnumerable<int>>(ids => ids.Count() == 1 && ids.Contains(3)),
                It.IsAny<ContentPersistentData>(),
                buildSettings.LoadAbstractItemFieldsToDetailsCollection,
                buildSettings.IsStage,
                null)).Returns(widgetExtDictionary);

            //по relationid и id виджета в qp можно получить полный список id, участвующих в связи m2m с этим виджетом
            //замокаем методы получения такой информации для 2х relation: один из контента с extension, один в базовом контенте. нужно проверить оба варианта
            var widgetRelation = new M2mRelations();
            foreach (var relValue in relationValues)
            {
                widgetRelation.AddRelation(relationId, relValue);
            }
            aiRepositoryMoq.Setup(x => x.GetManyToManyData(It.Is<IEnumerable<int>>(ids => ids.Count() == 1 && ids.Contains(widgetExtId)), isStage, null)).Returns(new Dictionary<int, M2mRelations>
            {
                { widgetExtId, widgetRelation}
            });

            var baseContentRelation = new M2mRelations();
            foreach (var relValue in baseContentRelationValues)
            {
                baseContentRelation.AddRelation(baseContentRelationId, relValue);
            }
            aiRepositoryMoq.Setup(x => x.GetManyToManyData(It.Is<IEnumerable<int>>(ids => ids.Contains(widgetId)), isStage, null)).Returns(new Dictionary<int, M2mRelations>
            {
                { widgetId, baseContentRelation}
            });

            //фабрика элементов структуры сайта
            var aiFactoryMoq = new Mock<IAbstractItemFactory>();
            aiFactoryMoq.Setup(x => x.Create(It.IsAny<string>())).Returns((string d) => {
                if (d == typeof(RootPage).Name) return new RootPage();
                if (d == typeof(StubStartPage).Name) return new StubStartPage();
                if (d == typeof(ManyToManyWidget).Name) return new ManyToManyWidget();
                return null;
            });

            var builder = new QpAbstractItemStorageBuilder(aiFactoryMoq.Object,
                Mock.Of<IQpUrlResolver>(),
                aiRepositoryMoq.Object,
                Mock.Of<IMetaInfoRepository>(),
                Mock.Of<IItemDefinitionRepository>(),
                buildSettings,
                Mock.Of<ILogger<QpAbstractItemStorageBuilder>>());

            var aiStorage = builder.Build();

            var widget = aiStorage.Get(widgetId) as ManyToManyWidget;
            Assert.NotNull(widget);

            //проверим, что в поле m2m подставились все id из relation, который был в контенте-расширения
            Assert.Equal(relationValues, widget.RelationIds);
            //проверим, что в поле m2m подставились все id из relation, который был в базовом контенте
            Assert.Equal(baseContentRelationValues, widget.BaseContentRelationIds);
        }
    }
}