using System.Linq;
using NUnit.Framework;
using QA.DotNetCore.Engine.Abstractions.Wildcard;

namespace QA.DotNetCore.Engine.Utils.Tests
{
    public class WildcardMatcherTests
    {
        private static IWildcardMatcher Create(WildcardMatchingOption option, params string[] items) => new WildcardMatcher(option, items);

        [Test]
        public void Test_WildCardMatcher()
        {
            var matcher = Create(WildcardMatchingOption.StartsWith, "bee.ru",
                "*.bee.ru",
                "*.stage.bee.ru",
                "stage.bee.ru",
                "stage.*.ru",
                "*");

            Assert.That("*.bee.ru", Is.EqualTo(matcher.MatchLongest("msc.bee.ru")));
            Assert.That("stage.bee.ru", Is.EqualTo(matcher.MatchLongest("stage.bee.ru")));
            Assert.That("*.bee.ru", Is.EqualTo(matcher.MatchLongest("msc.bee.ru")));
            Assert.That("stage.*.ru", Is.EqualTo(matcher.MatchLongest("stage.123.ru")));
            Assert.That("stage.*.ru", Is.EqualTo(matcher.MatchLongest("stage.1232344.ru")));
            Assert.That("*.bee.ru", Is.EqualTo(matcher.MatchLongest("msc.bee.ru")));
            Assert.That("*", Is.EqualTo(matcher.MatchLongest("ee.ru")));
            Assert.That("*.bee.ru", Is.EqualTo(matcher.MatchLongest("moskovskaya-obl.bee.ru")));
        }

        [Test]
        public void Test_WildCardMatcher_BatchBench()
        {
            var matcher = Create(WildcardMatchingOption.FullMatch, "bee.ru",
                "*.bee.ru",
                "*.stage.bee.ru",
                "stage.bee.ru",
                "stage.bee.ru",
                "stage.*.ru.*",
                "*");

            for (int i = 0; i < 10000; i++)
            {
                matcher.MatchLongest("stage.123.ru");
            }
        }

        [Test]
        public void Test_WildCardMatcher_Issue01_Incorrect()
        {
            var matcher = Create(WildcardMatchingOption.FullMatch,
                "bee.ru",
                "*.bee.ru",
                "stage.bee.ru"
            );

            Assert.That("*.bee.ru", Is.EqualTo(matcher.MatchLongest("msc.bee.ru")));
            Assert.That("*.bee.ru", Is.EqualTo(matcher.MatchLongest("www.bee.ru")));
            Assert.That(matcher.MatchLongest("bee.ru.artq.com"), Is.Null);

        }


        [Test]
        public void Test_WildCardMatcher_Issue02_Incorrect_one_letter()
        {
            var matcher = Create(WildcardMatchingOption.FullMatch,
                "bee.ru",
                "*.bee.ru",
                "f.bee.ru"
            );

            Assert.That("*.bee.ru", Is.EqualTo(matcher.MatchLongest("msc.bee.ru")));
            Assert.That("f.bee.ru", Is.EqualTo(matcher.MatchLongest("f.bee.ru")));
            Assert.That(matcher.MatchLongest("bee.ru.artq.com"), Is.Null);
        }

        [Test]
        public void Test_WildCardMatcher_UrlPatterns()
        {
            var matcher = Create(WildcardMatchingOption.FullMatch,
                "/page1*",
                "/page2/test*",
                "/page3*/details/*"
            );

            Assert.That(matcher.Match("/page1/page3").Any(), Is.True);
            Assert.That(matcher.Match("/page1").Any(), Is.True);

            Assert.That(matcher.Match("/page2").Any(), Is.False);
            Assert.That(matcher.Match("/page2/test").Any(), Is.True);
            Assert.That(matcher.Match("/page2/test/test2").Any(), Is.True);
            Assert.That(matcher.Match("/page2/tes").Any(), Is.False);

            Assert.That(matcher.Match("/page3/details/123").Any(), Is.True);
            Assert.That(matcher.Match("/page3/a/b/c/details/123").Any(), Is.True);
            Assert.That(matcher.Match("/page3/a/b/c/details").Any(), Is.False);
            Assert.That(matcher.Match("/page3/a/b/c/detail/123").Any(), Is.False);
        }
    }
}
