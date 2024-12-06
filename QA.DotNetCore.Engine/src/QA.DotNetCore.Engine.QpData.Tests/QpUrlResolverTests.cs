using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using Tests.CommonUtils.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace QA.DotNetCore.Engine.QpData.Tests
{
    /// <summary>
    /// Тесты на правила построения урлов до библиотеки сайта QP
    /// </summary>
    public class QpUrlResolverTests
    {
        private readonly ITestOutputHelper _output;

        public QpUrlResolverTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(true, "http://static.quantumart.ru", "storage.demo.artq.com", "/upload/dpcupload/", false,
            "http://static.quantumart.ru/upload/dpcupload/images")]
        [InlineData(true, "http://static.quantumart.ru/", "storage.demo.artq.com", "/upload/dpcupload/", false,
            "http://static.quantumart.ru/upload/dpcupload/images")]
        [InlineData(true, "http://static.quantumart.ru", "storage.demo.artq.com", "/upload/dpcupload", false,
            "http://static.quantumart.ru/upload/dpcupload/images")]
        [InlineData(true, "http://static.quantumart.ru", "storage.demo.artq.com", "/upload/dpcupload/", true,
            "//static.quantumart.ru/upload/dpcupload/images")]
        [InlineData(true, "http://static.quantumart.ru/", "storage.demo.artq.com", "/upload/dpcupload/", true,
            "//static.quantumart.ru/upload/dpcupload/images")]
        [InlineData(true, "blah://static.quantumart.ru", "storage.demo.artq.com", "/upload/dpcupload/", true,
            "blah://static.quantumart.ru/upload/dpcupload/images")]
        [InlineData(false, "http://static.quantumart.ru", "storage.demo.artq.com", "/upload/dpcupload/", false,
            "http://storage.demo.artq.com/upload/dpcupload/images")]
        [InlineData(false, "http://static.quantumart.ru", "storage.demo.artq.com", "/upload/dpcupload", false,
            "http://storage.demo.artq.com/upload/dpcupload/images")]
        [InlineData(false, "http://static.quantumart.ru", "storage.demo.artq.com", "/upload/dpcupload/", true,
            "//storage.demo.artq.com/upload/dpcupload/images")]
        public void UploadUrlIsCorrect(bool useAbsoluteUploadUrl,
            string uploadUrlPrefix,
            string dns,
            string uploadUrl,
            bool removeScheme,
            string expected)
        {
            var siteId = 1;
            var metaInfoMoq = new Mock<IMetaInfoRepository>();
            metaInfoMoq.Setup(x => x.GetSite(siteId, null)).Returns(new QpSitePersistentData
            {
                UseAbsoluteUploadUrl = useAbsoluteUploadUrl,
                UploadUrlPrefix = uploadUrlPrefix,
                Dns = dns,
                UploadUrl = uploadUrl
            });

            var urlResolver = CreateMockedUrlResolver(metaInfoMoq.Object);

            Assert.Equal(expected, urlResolver.UploadUrl(siteId, removeScheme));
        }

        [Theory]
        [InlineData("http://static.quantumart.ru", "/upload/dpcupload/", 777, false, null,
            "http://static.quantumart.ru/upload/dpcupload/contents/777")]
        [InlineData("http://static.quantumart.ru", "/upload/dpcupload/", 777, false, "subfolder",
            "http://static.quantumart.ru/upload/dpcupload/contents/777/subfolder")]
        [InlineData("http://static.quantumart.ru", "/upload/dpcupload/", 777, false, @"\subfolder",
            "http://static.quantumart.ru/upload/dpcupload/contents/777/subfolder")]
        [InlineData("http://static.quantumart.ru", "/upload/dpcupload/", 777, false, "/subfolder",
            "http://static.quantumart.ru/upload/dpcupload/contents/777/subfolder")]
        [InlineData("http://static.quantumart.ru", "/upload/dpcupload/", 777, true, null,
            "http://static.quantumart.ru/upload/dpcupload/images")]
        [InlineData("http://static.quantumart.ru", "/upload/dpcupload/", 777, true, "subfolder",
            "http://static.quantumart.ru/upload/dpcupload/images/subfolder")]
        public void UrlForImageIsCorrect(string uploadUrlPrefix,
            string uploadUrl,
            int contentId,
            bool useSiteLibrary,
            string subfolder,
            string expected)
        {
            //при проверке UrlForImage не будем проверять воздействие на него этих параметров, считаем что проверили это уже при проверке UploadUrl
            var useAbsoluteUploadUrl = true;
            var dns = "blah";
            var removeScheme = false;

            var siteId = 1;
            var fieldName = "testfield";
            var metaInfoMoq = new Mock<IMetaInfoRepository>();
            metaInfoMoq.Setup(x => x.GetSite(siteId, null)).Returns(new QpSitePersistentData
            {
                UseAbsoluteUploadUrl = useAbsoluteUploadUrl,
                UploadUrlPrefix = uploadUrlPrefix,
                Dns = dns,
                UploadUrl = uploadUrl
            });
            metaInfoMoq.Setup(x => x.GetContentAttribute(contentId, fieldName, null)).Returns(
                new ContentAttributePersistentData
                {
                    ContentId = contentId,
                    ColumnName = fieldName,
                    UseSiteLibrary = useSiteLibrary,
                    SubFolder = subfolder
                });

            var urlResolver = CreateMockedUrlResolver(metaInfoMoq.Object);

            Assert.Equal(expected, urlResolver.UrlForImage(siteId, contentId, fieldName, removeScheme));
        }

        private QpUrlResolver CreateMockedUrlResolver(IMetaInfoRepository metaInfoRepository)
        {
            var cacheSettings = new QpSiteStructureCacheSettings
            {
                ItemDefinitionCachePeriod = TimeSpan.FromSeconds(30),
                QpSchemeCachePeriod = TimeSpan.FromSeconds(30),
                SiteStructureCachePeriod = TimeSpan.FromSeconds(30)
            };
            var cache = new MemoryCache(new MemoryCacheOptions());
            var cacheProvider = new VersionedCacheCoreProvider(
                cache,
                new CacheKeyFactoryBase(),
                new MemoryLockFactory(new LoggerFactory()),
                new LoggerFactory()
            );

            var urlResolver = new QpUrlResolver(cacheProvider, metaInfoRepository, cacheSettings);
            return urlResolver;
        }
    }
}
