using Microsoft.VisualStudio.TestTools.UnitTesting;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.Tests.Fakes;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing.Tests
{
    [TestClass]
    public class UrlTokenMatcherUnitTest
    {
        [TestMethod]
        public void Test_Match_SimpleTemplate()
        {
            var urlTokenMatcher = new UrlTokenMatcher(CreateSimpleUrlTokenConfig());
            var ctx = CreateFakeTargetingContext();
            UrlMatchingResult m;

            m = urlTokenMatcher.Match("http://test.somesite.com/moskva/test1/test2/test3", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "ru-ru");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "http://test.somesite.com/test1/test2/test3");

            m = urlTokenMatcher.Match("http://test.somesite.com/en-us/moskva/a", ctx);
            Assert.AreEqual(m.SanitizedUrl, "http://test.somesite.com/a");

            m = urlTokenMatcher.Match("http://test.somesite.com/en-us/moskva/a/b", ctx);
            Assert.AreEqual(m.SanitizedUrl, "http://test.somesite.com/a/b");

            m = urlTokenMatcher.Match("http://test.somesite.com/en-us/moskva/a/b/c", ctx);
            Assert.AreEqual(m.SanitizedUrl, "http://test.somesite.com/a/b/c");

            m = urlTokenMatcher.Match("/en-us/moskva/test1/test2/test3", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "en-us");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "/test1/test2/test3");

            m = urlTokenMatcher.Match("/en-us/test1/test2/test3", ctx);
            Assert.IsTrue(m.IsMatch);
            Assert.IsFalse(m.AllTokenFound);
            Assert.AreEqual(m.TokenValues.Count, 1);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "en-us");
            Assert.AreEqual(m.SanitizedUrl, "/test1/test2/test3");
        }

        [TestMethod]
        public void Test_Match_AuthorityTemplate()
        {
            var urlTokenMatcher = new UrlTokenMatcher(CreateAuthorityUrlTokenConfig());
            var ctx = CreateFakeTargetingContext();
            UrlMatchingResult m;

            m = urlTokenMatcher.Match("/en-us/test1/test2/test3", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 1);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "en-us");
            Assert.AreEqual(m.SanitizedUrl, "/test1/test2/test3");

            m = urlTokenMatcher.Match("/test1/test2/test3", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 1);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "ru-ru");
            Assert.AreEqual(m.SanitizedUrl, "/test1/test2/test3");

            m = urlTokenMatcher.Match("http://moskva.localhost.ru/en-us/test1/test2/test3", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "en-us");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "http://moskva.localhost.ru/test1/test2/test3");

            m = urlTokenMatcher.Match("http://moskva.localhost.ru/test1/test2/test3", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "ru-ru");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "http://moskva.localhost.ru/test1/test2/test3");

            m = urlTokenMatcher.Match("http://stage.localhost.ru/test1/test2/test3", ctx);

            Assert.IsFalse(m.IsMatch);
        }

        [TestMethod]
        public void Test_Match_AllDefaultsTemplate()
        {
            var urlTokenMatcher = new UrlTokenMatcher(CreateAllDefaultsUrlTokenConfig());
            var ctx = CreateFakeTargetingContext();
            UrlMatchingResult m;

            m = urlTokenMatcher.Match("/", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "ru-ru");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "/");

            m = urlTokenMatcher.Match("/en-us", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "en-us");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "/");

            m = urlTokenMatcher.Match("/spb", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "ru-ru");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "spb");
            Assert.AreEqual(m.SanitizedUrl, "/");

            m = urlTokenMatcher.Match("/moskva", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "ru-ru");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "/");

            m = urlTokenMatcher.Match("/ru-ru", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "ru-ru");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "/");

            m = urlTokenMatcher.Match("/kk-kz/spb/test1/test2", ctx);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "kk-kz");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "spb");
            Assert.AreEqual(m.SanitizedUrl, "/test1/test2");
        }

        [TestMethod]
        public void Test_ReplaceTokens_SimpleTemplate()
        {
            var urlTokenMatcher = new UrlTokenMatcher(CreateSimpleUrlTokenConfig());
            var ctx = CreateFakeTargetingContext();

            var newUrl = urlTokenMatcher.ReplaceTokens("http://test.somesite.com/moskva/qwe", new Dictionary<string, string> { { "region", "spb" } }, ctx);
            Assert.AreEqual(newUrl, "http://test.somesite.com/spb/qwe");

            newUrl = urlTokenMatcher.ReplaceTokens(newUrl, new Dictionary<string, string> { { "region", "moskva" }, { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "http://test.somesite.com/en-us/moskva/qwe");

            newUrl = urlTokenMatcher.ReplaceTokens(newUrl, new Dictionary<string, string> { { "culture", "ru-ru" } }, ctx);
            Assert.AreEqual(newUrl, "http://test.somesite.com/moskva/qwe");

            newUrl = urlTokenMatcher.ReplaceTokens("http://test.somesite.com/spb/", new Dictionary<string, string> { { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "http://test.somesite.com/en-us/spb");

            newUrl = urlTokenMatcher.ReplaceTokens("http://test.somesite.com/moskva/qwe", new Dictionary<string, string> { { "region", "dvfrg" } }, ctx);
            Assert.AreEqual(newUrl, "http://test.somesite.com/moskva/qwe");

            newUrl = urlTokenMatcher.ReplaceTokens("http://test.somesite.com/qwe", new Dictionary<string, string> { { "region", "moskva" }, { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "http://test.somesite.com/en-us/moskva/qwe");

            newUrl = urlTokenMatcher.ReplaceTokens("/", new Dictionary<string, string> { { "region", "moskva" } }, ctx);
            Assert.AreEqual(newUrl, "/moskva");

            newUrl = urlTokenMatcher.ReplaceTokens("/moskva", new Dictionary<string, string> { { "culture", "kk-kz" } }, ctx);
            Assert.AreEqual(newUrl, "/kk-kz/moskva");

            newUrl = urlTokenMatcher.ReplaceTokens("/", new Dictionary<string, string> { { "culture", "kk-kz" } }, ctx);
            Assert.AreEqual(newUrl, "/kk-kz");

            newUrl = urlTokenMatcher.ReplaceTokens("/kk-kz", new Dictionary<string, string> { { "region", "spb" } }, ctx);
            Assert.AreEqual(newUrl, "/kk-kz/spb");

            newUrl = urlTokenMatcher.ReplaceTokens("/", new Dictionary<string, string> { { "region", "moskva" }, { "culture", "kk-kz" } }, ctx);
            Assert.AreEqual(newUrl, "/kk-kz/moskva");

            newUrl = urlTokenMatcher.ReplaceTokens("/test1", new Dictionary<string, string> { { "culture", "kk-kz" }, { "region", "spb" } }, ctx);
            Assert.AreEqual(newUrl, "/kk-kz/spb/test1");

            newUrl = urlTokenMatcher.ReplaceTokens("/en-us/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "kk-kz" } }, ctx);
            Assert.AreEqual(newUrl, "/kk-kz/spb/test1/test2");

            newUrl = urlTokenMatcher.ReplaceTokens("/en-us", new Dictionary<string, string> { { "region", "spb" } }, ctx);
            Assert.AreEqual(newUrl, "/en-us/spb");
        }

        [TestMethod]
        public void Test_ReplaceTokens_AuthorityTemplate()
        {
            var urlTokenMatcher = new UrlTokenMatcher(CreateAuthorityUrlTokenConfig());
            var ctx = CreateFakeTargetingContext();

            var newUrl = urlTokenMatcher.ReplaceTokens("/", new Dictionary<string, string> { { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "/en-us");

            newUrl = urlTokenMatcher.ReplaceTokens("/en-us/test1/test2", new Dictionary<string, string> { { "culture", "ru-ru" } }, ctx);
            Assert.AreEqual(newUrl, "/test1/test2");

            newUrl = urlTokenMatcher.ReplaceTokens("http://moskva.localhost.ru/test1/test2", new Dictionary<string, string> { { "region", "spb" } }, ctx);
            Assert.AreEqual(newUrl, "http://spb.localhost.ru/test1/test2");

            newUrl = urlTokenMatcher.ReplaceTokens("http://moskva.localhost.ru/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "http://spb.localhost.ru/en-us/test1/test2");

            newUrl = urlTokenMatcher.ReplaceTokens("http://stage.localhost.ru/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "http://stage.localhost.ru/test1/test2");
        }

        [TestMethod]
        public void Test_ReplaceTokens_AllDefaultsTemplate()
        {
            var urlTokenMatcher = new UrlTokenMatcher(CreateAllDefaultsUrlTokenConfig());
            var ctx = CreateFakeTargetingContext();

            var newUrl = urlTokenMatcher.ReplaceTokens("/", new Dictionary<string, string> { { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "/en-us");

            newUrl = urlTokenMatcher.ReplaceTokens("/", new Dictionary<string, string> { { "culture", "ru-ru" } }, ctx);
            Assert.AreEqual(newUrl, "/");

            newUrl = urlTokenMatcher.ReplaceTokens("/", new Dictionary<string, string> { { "region", "spb" } }, ctx);
            Assert.AreEqual(newUrl, "/spb");

            newUrl = urlTokenMatcher.ReplaceTokens("/", new Dictionary<string, string> { { "region", "moskva" } }, ctx);
            Assert.AreEqual(newUrl, "/");

            newUrl = urlTokenMatcher.ReplaceTokens("/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "/en-us/spb/test1/test2");

            newUrl = urlTokenMatcher.ReplaceTokens("/test1/test2", new Dictionary<string, string> { { "region", "moskva" }, { "culture", "en-us" } }, ctx);
            Assert.AreEqual(newUrl, "/en-us/test1/test2");

            newUrl = urlTokenMatcher.ReplaceTokens("/test1/test2", new Dictionary<string, string> { { "region", "spb" }, { "culture", "ru-ru" } }, ctx);
            Assert.AreEqual(newUrl, "/spb/test1/test2");

            newUrl = urlTokenMatcher.ReplaceTokens("/test1/test2", new Dictionary<string, string> { { "region", "moskva" }, { "culture", "ru-ru" } }, ctx);
            Assert.AreEqual(newUrl, "/test1/test2");
        }

        private ITargetingContext CreateFakeTargetingContext()
        {
            return new FakeTargetingContext(
                new Dictionary<string, string> { { "culture", "ru-ru" }, { "region", "moskva" } },
                new Dictionary<string, IEnumerable<string>> { { "culture", new string[] { "ru-ru", "en-us", "kk-kz" } }, { "region", new string[] { "moskva", "spb" } } }
            );
        }

        private UrlTokenConfig CreateSimpleUrlTokenConfig()
        {
            return new UrlTokenConfig
            {
                MatchingPatterns = new List<UrlMatchingPattern>
                {
                    new UrlMatchingPattern{ Value = "/{culture}/{region}"},
                    new UrlMatchingPattern{ Value = "/{region}", Defaults = new Dictionary<string, string> { { "culture", "ru-ru" } } }
                }
            };
        }

        private UrlTokenConfig CreateAllDefaultsUrlTokenConfig()
        {
            return new UrlTokenConfig
            {
                MatchingPatterns = new List<UrlMatchingPattern>
                {
                    new UrlMatchingPattern{ Value = "/{culture}/{region}"},
                    new UrlMatchingPattern{ Value = "/{region}", Defaults = new Dictionary<string, string> { { "culture", "ru-ru" } } },
                    new UrlMatchingPattern{ Value = "/{culture}", Defaults = new Dictionary<string, string> { { "region", "moskva" } } },
                    new UrlMatchingPattern{ Value = "/", Defaults = new Dictionary<string, string> { { "culture", "ru-ru" }, { "region", "moskva" } }}
                }
            };
        }

        private UrlTokenConfig CreateAuthorityUrlTokenConfig()
        {
            return new UrlTokenConfig
            {
                MatchingPatterns = new List<UrlMatchingPattern>
                {
                    new UrlMatchingPattern{ Value = "//{region}.test.ru/{culture}"},
                    new UrlMatchingPattern{ Value = "//{region}.test.ru", Defaults = new Dictionary<string, string> { { "culture", "ru-ru" } } }
                }
            };
        }
    }
}
