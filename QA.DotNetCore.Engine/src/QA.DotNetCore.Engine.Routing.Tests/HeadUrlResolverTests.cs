using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing.Tests
{
    [TestClass]
    public class HeadUrlResolverTests
    {
        [TestMethod]
        public void Test_Match_SimpleTemplate()
        {
            var headUrlResolver = new HeadUrlResolver(CreateSimpleUrlTokenConfig(), PossibleValuesMock());

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
            var headUrlResolver = new HeadUrlResolver(CreateAuthorityUrlTokenConfig(), PossibleValuesMock());

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

        [TestMethod]
        public void Test_Match_AllDefaultsTemplate()
        {
            var headUrlResolver = new HeadUrlResolver(CreateAllDefaultsUrlTokenConfig(), PossibleValuesMock());

            var tokens = headUrlResolver.ResolveTokenValues("/");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "ru-ru");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "moskva");

            var sanitized = headUrlResolver.SanitizeUrl("/");
            Assert.AreEqual(sanitized, "/");

            tokens = headUrlResolver.ResolveTokenValues("/en-us");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "en-us");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "moskva");

            sanitized = headUrlResolver.SanitizeUrl("/en-us");
            Assert.AreEqual(sanitized, "/");

            tokens = headUrlResolver.ResolveTokenValues("/spb");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "ru-ru");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "spb");

            sanitized = headUrlResolver.SanitizeUrl("/spb");
            Assert.AreEqual(sanitized, "/");

            tokens = headUrlResolver.ResolveTokenValues("/moskva");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "ru-ru");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "moskva");

            sanitized = headUrlResolver.SanitizeUrl("/moskva");
            Assert.AreEqual(sanitized, "/");

            tokens = headUrlResolver.ResolveTokenValues("/ru-ru");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "ru-ru");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "moskva");

            sanitized = headUrlResolver.SanitizeUrl("/ru-ru");
            Assert.AreEqual(sanitized, "/");

            tokens = headUrlResolver.ResolveTokenValues("/kk-kz/spb/test1/test2");
            Assert.AreEqual(tokens.Count, 2);
            Assert.IsTrue(tokens.ContainsKey("culture"));
            Assert.AreEqual(tokens["culture"], "kk-kz");
            Assert.IsTrue(tokens.ContainsKey("region"));
            Assert.AreEqual(tokens["region"], "spb");

            sanitized = headUrlResolver.SanitizeUrl("/kk-kz/spb/test1/test2");
            Assert.AreEqual(sanitized, "/test1/test2");
        }

        [TestMethod]
        public void Test_ReplaceTokens_SimpleTemplate()
        {
            var headUrlResolver = new HeadUrlResolver(CreateSimpleUrlTokenConfig(), PossibleValuesMock());

            var newUrl = headUrlResolver.AddTokensToUrl("http://test.somesite.com/moskva/qwe", new Dictionary<string, string> { { "region", "spb" } });
            Assert.AreEqual(newUrl, "http://test.somesite.com/spb/qwe");

            newUrl = headUrlResolver.AddTokensToUrl(newUrl, new Dictionary<string, string> { { "region", "moskva" }, { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "http://test.somesite.com/en-us/moskva/qwe");

            newUrl = headUrlResolver.AddTokensToUrl(newUrl, new Dictionary<string, string> { { "culture", "ru-ru" } });
            Assert.AreEqual(newUrl, "http://test.somesite.com/moskva/qwe");

            newUrl = headUrlResolver.AddTokensToUrl("http://test.somesite.com/spb/", new Dictionary<string, string> { { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "http://test.somesite.com/en-us/spb");

            newUrl = headUrlResolver.AddTokensToUrl("http://test.somesite.com/moskva/qwe", new Dictionary<string, string> { { "region", "dvfrg" } });
            Assert.AreEqual(newUrl, "http://test.somesite.com/moskva/qwe");

            newUrl = headUrlResolver.AddTokensToUrl("http://test.somesite.com/qwe", new Dictionary<string, string> { { "region", "moskva" }, { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "http://test.somesite.com/en-us/moskva/qwe");

            newUrl = headUrlResolver.AddTokensToUrl("/", new Dictionary<string, string> { { "region", "moskva" } });
            Assert.AreEqual(newUrl, "/moskva");

            newUrl = headUrlResolver.AddTokensToUrl("/moskva", new Dictionary<string, string> { { "culture", "kk-kz" } });
            Assert.AreEqual(newUrl, "/kk-kz/moskva");

            newUrl = headUrlResolver.AddTokensToUrl("/", new Dictionary<string, string> { { "culture", "kk-kz" } });
            Assert.AreEqual(newUrl, "/kk-kz");

            newUrl = headUrlResolver.AddTokensToUrl("/kk-kz", new Dictionary<string, string> { { "region", "spb" } });
            Assert.AreEqual(newUrl, "/kk-kz/spb");

            newUrl = headUrlResolver.AddTokensToUrl("/", new Dictionary<string, string> { { "region", "moskva" }, { "culture", "kk-kz" } });
            Assert.AreEqual(newUrl, "/kk-kz/moskva");

            newUrl = headUrlResolver.AddTokensToUrl("/test1", new Dictionary<string, string> { { "culture", "kk-kz" }, { "region", "spb" } });
            Assert.AreEqual(newUrl, "/kk-kz/spb/test1");

            newUrl = headUrlResolver.AddTokensToUrl("/en-us/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "kk-kz" } });
            Assert.AreEqual(newUrl, "/kk-kz/spb/test1/test2");

            newUrl = headUrlResolver.AddTokensToUrl("/en-us", new Dictionary<string, string> { { "region", "spb" } });
            Assert.AreEqual(newUrl, "/en-us/spb");
        }

        [TestMethod]
        public void Test_ReplaceTokens_AuthorityTemplate()
        {
            var headUrlResolver = new HeadUrlResolver(CreateAuthorityUrlTokenConfig(), PossibleValuesMock());

            var newUrl = headUrlResolver.AddTokensToUrl("/", new Dictionary<string, string> { { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "/en-us");

            newUrl = headUrlResolver.AddTokensToUrl("/en-us/test1/test2", new Dictionary<string, string> { { "culture", "ru-ru" } });
            Assert.AreEqual(newUrl, "/test1/test2");

            newUrl = headUrlResolver.AddTokensToUrl("http://moskva.localhost.ru/test1/test2", new Dictionary<string, string> { { "region", "spb" } });
            Assert.AreEqual(newUrl, "http://spb.localhost.ru/test1/test2");

            newUrl = headUrlResolver.AddTokensToUrl("http://moskva.localhost.ru/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "http://spb.localhost.ru/en-us/test1/test2");

            newUrl = headUrlResolver.AddTokensToUrl("http://stage.localhost.ru/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "http://stage.localhost.ru/test1/test2");
        }

        [TestMethod]
        public void Test_ReplaceTokens_AllDefaultsTemplate()
        {
            var headUrlResolver = new HeadUrlResolver(CreateAllDefaultsUrlTokenConfig(), PossibleValuesMock());

            var newUrl = headUrlResolver.AddTokensToUrl("/", new Dictionary<string, string> { { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "/en-us");

            newUrl = headUrlResolver.AddTokensToUrl("/", new Dictionary<string, string> { { "culture", "ru-ru" } });
            Assert.AreEqual(newUrl, "/");

            newUrl = headUrlResolver.AddTokensToUrl("/", new Dictionary<string, string> { { "region", "spb" } });
            Assert.AreEqual(newUrl, "/spb");

            newUrl = headUrlResolver.AddTokensToUrl("/", new Dictionary<string, string> { { "region", "moskva" } });
            Assert.AreEqual(newUrl, "/");

            newUrl = headUrlResolver.AddTokensToUrl("/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "/en-us/spb/test1/test2");

            newUrl = headUrlResolver.AddTokensToUrl("/test1/test2", new Dictionary<string, string> { { "region", "moskva" }, { "culture", "en-us" } });
            Assert.AreEqual(newUrl, "/en-us/test1/test2");

            newUrl = headUrlResolver.AddTokensToUrl("/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "ru-ru" } });
            Assert.AreEqual(newUrl, "/spb/test1/test2");

            newUrl = headUrlResolver.AddTokensToUrl("/test1/test2", new Dictionary<string, string> { { "region", "moskva" }, { "culture", "ru-ru" } });
            Assert.AreEqual(newUrl, "/test1/test2");
        }

        private static IHeadTokenPossibleValuesAccessor PossibleValuesMock()
        {
            var possibleValuesAccessorMoq = new Mock<IHeadTokenPossibleValuesAccessor>();
            possibleValuesAccessorMoq.Setup(x => x.GetPossibleValues("culture")).Returns(new[] { "ru-ru", "en-us", "kk-kz" });
            possibleValuesAccessorMoq.Setup(x => x.GetPossibleValues("region")).Returns(new[] { "moskva", "spb" });
            return possibleValuesAccessorMoq.Object;
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

        private UrlTokenConfig CreateAllDefaultsUrlTokenConfig()
        {
            return new UrlTokenConfig
            {
                HeadPatterns = new List<HeadUrlMatchingPattern>
                        {
                            new HeadUrlMatchingPattern{ Pattern = "/{culture}/{region}"},
                            new HeadUrlMatchingPattern{ Pattern = "/{region}", Defaults = new Dictionary<string, string> { { "culture", "ru-ru" } } },
                            new HeadUrlMatchingPattern{ Pattern = "/{culture}", Defaults = new Dictionary<string, string> { { "region", "moskva" } } },
                            new HeadUrlMatchingPattern{ Pattern = "/", Defaults = new Dictionary<string, string> { { "culture", "ru-ru" }, { "region", "moskva" } }}
                        }
            };
        }
    }
}
