using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing.Tests
{
    public class HeadUrlResolverTests
    {
        [TestMethod]
        public void Test_Match_SimpleTemplate()
        {
            var possibleValuesAccessorMoq = new Mock<IHeadTokenPossibleValuesAccessor>();
            possibleValuesAccessorMoq.Setup(x => x.GetPossibleValues("culture")).Returns(new[] { "ru-ru", "en-us" });
            possibleValuesAccessorMoq.Setup(x => x.GetPossibleValues("region")).Returns(new[] { "moskva", "spb" });

            var headUrlResolver = new HeadUrlResolver(CreateSimpleUrlTokenConfig(), possibleValuesAccessorMoq.Object);

            var tokens = headUrlResolver.ResolveTokenValues("http://test.somesite.com/moskva/test1/test2/test3");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "ru-ru");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "moskva");

            var sanitized = headUrlResolver.SanitizeUrl("http://test.somesite.com/moskva/test1/test2/test3");
            Assert.AreEqual(sanitized, "http://test.somesite.com/test1/test2/test3");

            sanitized = headUrlResolver.SanitizeUrl("http://test.somesite.com/en-us/moskva/a");
            Assert.AreEqual(sanitized, "http://test.somesite.com/a");

            sanitized = headUrlResolver.SanitizeUrl("http://test.somesite.com/en-us/moskva/a/b");
            Assert.AreEqual(sanitized, "http://test.somesite.com/a/b");

            sanitized = headUrlResolver.SanitizeUrl("http://test.somesite.com/en-us/moskva/a/b/c");
            Assert.AreEqual(sanitized, "http://test.somesite.com/a/b/c");

            tokens = headUrlResolver.ResolveTokenValues("/en-us/moskva/test1/test2/test3");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "en-us");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "moskva");

            sanitized = headUrlResolver.SanitizeUrl("/en-us/moskva/test1/test2/test3");
            Assert.AreEqual(sanitized, "/test1/test2/test3");

            tokens = headUrlResolver.ResolveTokenValues("/en-us/test1/test2/test3");
            Assert.AreEqual(tokens.Count, 1);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "en-us");

            sanitized = headUrlResolver.SanitizeUrl("/en-us/test1/test2/test3");
            Assert.AreEqual(sanitized, "/test1/test2/test3");
        }

        [TestMethod]
        public void Test_Match_AuthorityTemplate()
        {
            var possibleValuesAccessorMoq = new Mock<IHeadTokenPossibleValuesAccessor>();
            possibleValuesAccessorMoq.Setup(x => x.GetPossibleValues("culture")).Returns(new[] { "ru-ru", "en-us" });
            possibleValuesAccessorMoq.Setup(x => x.GetPossibleValues("region")).Returns(new[] { "moskva", "spb" });

            var headUrlResolver = new HeadUrlResolver(CreateAuthorityUrlTokenConfig(), possibleValuesAccessorMoq.Object);

            var tokens = headUrlResolver.ResolveTokenValues("/en-us/test1/test2/test3");
            Assert.AreEqual(tokens.Count, 1);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "en-us");

            var sanitized = headUrlResolver.SanitizeUrl("/en-us/test1/test2/test3");
            Assert.AreEqual(sanitized, "/test1/test2/test3");

            tokens = headUrlResolver.ResolveTokenValues("/test1/test2/test3");
            Assert.AreEqual(tokens.Count, 1);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "ru-ru");

            sanitized = headUrlResolver.SanitizeUrl("/test1/test2/test3");
            Assert.AreEqual(sanitized, "/test1/test2/test3");

            tokens = headUrlResolver.ResolveTokenValues("http://moskva.localhost.ru/en-us/test1/test2/test3");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "en-us");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "moskva");

            sanitized = headUrlResolver.SanitizeUrl("http://moskva.localhost.ru/en-us/test1/test2/test3");
            Assert.AreEqual(sanitized, "http://moskva.localhost.ru/test1/test2/test3");

            tokens = headUrlResolver.ResolveTokenValues("http://moskva.localhost.ru/test1/test2/test3");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "ru-ru");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "moskva");

            sanitized = headUrlResolver.SanitizeUrl("http://moskva.localhost.ru/test1/test2/test3");
            Assert.AreEqual(sanitized, "http://moskva.localhost.ru/test1/test2/test3");

            tokens = headUrlResolver.ResolveTokenValues("http://stage.localhost.ru/en-us/test1/test2/test3");
            Assert.AreEqual(tokens.Count, 0);
        }

        private UrlTokenConfig CreateSimpleUrlTokenConfig()
        {
            return new UrlTokenConfig
            {
                HeadPatterns = new List<HeadUrlMatchingPattern>
                {
                    new HeadUrlMatchingPattern{ Pattern = "/{culture}/{region}"},
                    new HeadUrlMatchingPattern{ Pattern = "/{region}", Defaults = new Dictionary<string, string> { { "culture", "ru-ru" } } }
                }
            };
        }

        private UrlTokenConfig CreateAuthorityUrlTokenConfig()
        {
            return new UrlTokenConfig
            {
                HeadPatterns = new List<HeadUrlMatchingPattern>
                        {
                            new HeadUrlMatchingPattern{ Pattern = "//{region}.test.ru/{culture}"},
                            new HeadUrlMatchingPattern{ Pattern = "//{region}.test.ru", Defaults = new Dictionary<string, string> { { "culture", "ru-ru" } } }
                        }
            };
        }
    }
}
