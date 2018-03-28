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
        public void MatchTest()
        {
            var urlTokenMatcher = new UrlTokenMatcher(CreateUrlTokenConfig());
            var ctx = CreateFakeTargetingContext();
            UrlMatchingResult m;

            m = urlTokenMatcher.Match("http://test.somesite.com/moskva/test1/test2/test3/", ctx);

            Assert.AreEqual(m.TokenValues.Count, 2);
            Assert.IsTrue(m.TokenValues.ContainsKey("culture"));
            Assert.AreEqual(m.TokenValues["culture"], "ru-ru");
            Assert.IsTrue(m.TokenValues.ContainsKey("region"));
            Assert.AreEqual(m.TokenValues["region"], "moskva");
            Assert.AreEqual(m.SanitizedUrl, "http://test.somesite.com/test1/test2/test3/");
            
        }

        [TestMethod]
        public void ReplaceTokensTest()
        {
            var urlTokenMatcher = new UrlTokenMatcher(CreateUrlTokenConfig());
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
        }

        private ITargetingContext CreateFakeTargetingContext()
        {
            return new FakeTargetingContext(
                new Dictionary<string, string> { { "culture", "ru-ru" }, { "region", "moskva" } },
                new Dictionary<string, IEnumerable<string>> { { "culture", new string[] { "ru-ru", "en-us", "kk-kz" } }, { "region", new string[] { "moskva", "spb" } } }
            );
        }

        private UrlTokenConfig CreateUrlTokenConfig()
        {
            return new UrlTokenConfig
            {
                MatchingPatterns = new List<UrlMatchingPattern>
                {
                    new UrlMatchingPattern{ Value = "//test.somesite.com/{culture}/{region}"},
                    new UrlMatchingPattern{ Value = "//test.somesite.com/{region}", Defaults = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("culture", "ru-ru") } }
                }
            };
        }
    }
}
