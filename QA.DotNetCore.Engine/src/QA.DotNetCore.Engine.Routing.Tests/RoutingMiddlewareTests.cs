using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Exceptions;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets;
using QA.DotNetCore.Engine.Routing.Exceptions;
using QA.DotNetCore.Engine.Routing.Tests.StubClasses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tests.CommonUtils.Helpers;

namespace QA.DotNetCore.Engine.Routing.Tests
{
    [TestClass]
    public class RoutingMiddlewareTests
    {
        private readonly RequestDelegate _next = (HttpContext hc) => { return Task.CompletedTask; };

        private const int
            siteID = 1,
            abstractItemContentID = 666;

        private const bool isStage = false;

        private readonly static string
            stubHost = StubStartPage.DnsRegistered,
            abstractItemNetName = KnownNetNames.AbstractItem,
            uploadUrlPlaceholder = "<%upload_url%>";

        private readonly QpSiteStructureBuildSettings buildSettings = new QpSiteStructureBuildSettings
        {
            SiteId = siteID,
            IsStage = isStage,
            RootPageDiscriminator = typeof(RootPage).Name,
            UploadUrlPlaceholder = uploadUrlPlaceholder,
            LoadAbstractItemFieldsToDetailsCollection = true
        };

        [TestMethod]
        public async Task FunctionalTest_RoutingMiddleware_Success()
        {
            HttpContext ctx = new DefaultHttpContext();
            ctx.Request.Host = new HostString(stubHost);

            RoutingMiddleware routingMiddleware = new RoutingMiddleware(_next, new NullTargetingFilterAccessor());

            Mock<IAbstractItemStorageProvider> mockAbstractItemStorageProvider =
                new Mock<IAbstractItemStorageProvider>();

            AbstractItemStorage abstractItemStorage = BuildAbstractItemStorage();

            mockAbstractItemStorageProvider
                .Setup(x => x.Get())
                .Returns(abstractItemStorage);

            await routingMiddleware.Invoke(ctx, mockAbstractItemStorageProvider.Object);

            Assert.IsNotNull(ctx.Items[RoutingKeys.StartPage]);
            Assert.IsInstanceOfType(ctx.Items[RoutingKeys.StartPage], typeof(StubStartPage));
            Assert.AreEqual(2, ((StubStartPage) ctx.Items[RoutingKeys.StartPage]).Id);

            Assert.IsNotNull(ctx.Items[RoutingKeys.AbstractItemStorage]);
            Assert.IsInstanceOfType(ctx.Items[RoutingKeys.AbstractItemStorage], typeof(AbstractItemStorage));
            Assert.AreEqual(abstractItemStorage, (AbstractItemStorage) ctx.Items[RoutingKeys.AbstractItemStorage]);
        }

        [TestMethod]
        public async Task FunctionalTest_RoutingMiddleware_ThrowsStartPageNotFound_IncorrectDNSBinding()
        {
            HttpContext ctx = new DefaultHttpContext();
            ctx.Request.Host = new HostString("test.qp.lan");

            RoutingMiddleware routingMiddleware = new RoutingMiddleware(_next, new NullTargetingFilterAccessor());

            Mock<IAbstractItemStorageProvider> mockAbstractItemStorageProvider =
                new Mock<IAbstractItemStorageProvider>();

            AbstractItemStorage abstractItemStorage = BuildAbstractItemStorage();

            mockAbstractItemStorageProvider
                .Setup(x => x.Get())
                .Returns(abstractItemStorage);

            await Assert.ThrowsExceptionAsync<StartPageNotFoundException>(() =>
                routingMiddleware.Invoke(ctx, mockAbstractItemStorageProvider.Object));
        }

        [TestMethod]
        public async Task FunctionalTest_RoutingMiddleware_ThrowsStartPageNotFound_NoPage()
        {
            HttpContext ctx = new DefaultHttpContext();
            ctx.Request.Host = new HostString("test.qp.lan");

            RoutingMiddleware routingMiddleware = new RoutingMiddleware(_next, new NullTargetingFilterAccessor());

            Mock<IAbstractItemStorageProvider> mockAbstractItemStorageProvider =
                new Mock<IAbstractItemStorageProvider>();

            AbstractItemStorage abstractItemStorage = BuildAbstractItemStorage(new AbstractItemPersistentData[]
            {
                new AbstractItemPersistentData
                {
                    Id = 1, Title = "корневая страница", Alias = "root", Discriminator = typeof(RootPage).Name,
                    IsPage = true, ParentId = null, ExtensionId = null
                },
            });

            mockAbstractItemStorageProvider
                .Setup(x => x.Get())
                .Returns(abstractItemStorage);

            await Assert.ThrowsExceptionAsync<StartPageNotFoundException>(() =>
                routingMiddleware.Invoke(ctx, mockAbstractItemStorageProvider.Object));
        }

        [TestMethod]
        public async Task FunctionalTest_RoutingMiddleware_ThrowsDeprecateCacheIsExpiredOrMissingException_NoCache()
        {
            HttpContext ctx = new DefaultHttpContext();
            ctx.Request.Host = new HostString("test.qp.lan");

            RoutingMiddleware routingMiddleware = new RoutingMiddleware(_next, new NullTargetingFilterAccessor());

            StubCacheProvider stubCacheProvider = new StubCacheProvider();
            CacheStubAbstractItemStorageProvider cacheStubAbstractItemStorageProvider =
                new CacheStubAbstractItemStorageProvider(stubCacheProvider);

            await Assert.ThrowsExceptionAsync<DeprecateCacheIsExpiredOrMissingException>(() =>
                routingMiddleware.Invoke(ctx, cacheStubAbstractItemStorageProvider));
        }

        private AbstractItemStorage BuildAbstractItemStorage(
            AbstractItemPersistentData[] abstractItemPersistentDatas = null)
        {
            if (abstractItemPersistentDatas == null)
                abstractItemPersistentDatas = new[]
                {
                    new AbstractItemPersistentData
                    {
                        Id = 2, Title = "стартовая страница", Alias = "start",
                        Discriminator = typeof(StubStartPage).Name, IsPage = true, ParentId = 1, ExtensionId = null
                    },
                    new AbstractItemPersistentData
                    {
                        Id = 1, Title = "корневая страница", Alias = "root", Discriminator = typeof(RootPage).Name,
                        IsPage = true, ParentId = null, ExtensionId = null
                    },
                    new AbstractItemPersistentData
                    {
                        Id = 3, Title = "страница sitemap", Alias = "sitemap", Discriminator = typeof(StubPage).Name,
                        IsPage = true, ParentId = 2, ExtensionId = null
                    },
                    new AbstractItemPersistentData
                    {
                        Id = 101, Title = "страница 101", Alias = "page", Discriminator = typeof(StubPage).Name,
                        IsPage = true, ParentId = 2, ExtensionId = null, IndexOrder = 1
                    },
                    new AbstractItemPersistentData
                    {
                        Id = 102, Title = "страница 102", Alias = "page", Discriminator = typeof(StubPage).Name,
                        IsPage = true, ParentId = 2, ExtensionId = null, IndexOrder = 2
                    },
                    new AbstractItemPersistentData
                    {
                        Id = 1001, Title = "страница 1001", Alias = "page", Discriminator = typeof(StubPage).Name,
                        IsPage = true, ParentId = 102, ExtensionId = null
                    },
                };

            Mock<IAbstractItemRepository> aiRepositoryMoq = new Mock<IAbstractItemRepository>();

            aiRepositoryMoq.Setup(x => x.GetPlainAllAbstractItems(siteID, isStage, null))
                .Returns(abstractItemPersistentDatas);

            aiRepositoryMoq
                .Setup(x => x.GetAbstractItemExtensionIds(It.IsAny<int[]>(), null))
                .Returns(new int[0]);

            Mock<IMetaInfoRepository> metaInfoMoq = new Mock<IMetaInfoRepository>();
            metaInfoMoq.Setup(x => x.GetContent(abstractItemNetName, siteID, null)).Returns(new ContentPersistentData
            {
                ContentId = abstractItemContentID,
                ContentAttributes = new List<ContentAttributePersistentData>()
            });

            Mock<IAbstractItemFactory> aiFactoryMoq = new Mock<IAbstractItemFactory>();

            aiFactoryMoq.Setup(x => x.Create(It.IsAny<string>())).Returns((string d) =>
            {
                if (d == typeof(RootPage).Name) return new RootPage();
                if (d == typeof(StubStartPage).Name) return new StubStartPage();
                if (d == typeof(StubPage).Name) return new StubPage();
                if (d == typeof(StubWidget).Name) return new StubWidget();
                return null;
            });

            ILogger<QpAbstractItemStorageBuilder> logger =
                NullLoggerFactory.Instance.CreateLogger<QpAbstractItemStorageBuilder>();

            // Arrange serviceScopeFactory
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IQpUrlResolver)))
                .Returns(Mock.Of<IQpUrlResolver>());
            serviceProvider
                .Setup(x => x.GetService(typeof(QpSiteStructureBuildSettings)))
                .Returns(Mock.Of<QpSiteStructureBuildSettings>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IAbstractItemRepository)))
                .Returns(aiRepositoryMoq.Object);
            serviceProvider
                .Setup(x => x.GetService(typeof(ILogger<QpAbstractItemStorageBuilder>)))
                .Returns(Mock.Of<ILogger<QpAbstractItemStorageBuilder>>());

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory.Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            var cacheProvider = new VersionedCacheCoreProvider(
                new MemoryCache(new MemoryCacheOptions()),
                new CacheKeyFactoryBase(),
                new MemoryLockFactory(NullLoggerFactory.Instance.CreateLogger<MemoryLockFactory>()),
                Mock.Of<ILogger>());
            var cacheSettings = new QpSiteStructureCacheSettings
            {
                ItemDefinitionCachePeriod = TimeSpan.FromSeconds(30),
                QpSchemeCachePeriod = TimeSpan.FromSeconds(30),
                SiteStructureCachePeriod = TimeSpan.FromSeconds(30)
            };

            QpAbstractItemStorageBuilder builder = new QpAbstractItemStorageBuilder(
                aiFactoryMoq.Object,
                aiRepositoryMoq.Object,
                metaInfoMoq.Object,
                buildSettings,
                logger,
                serviceScopeFactory.Object,
                serviceProvider.Object,
                Mock.Of<IQpContentCacheTagNamingProvider>(),
                cacheProvider,
                cacheSettings);

            return builder.Build();
        }
    }
}
