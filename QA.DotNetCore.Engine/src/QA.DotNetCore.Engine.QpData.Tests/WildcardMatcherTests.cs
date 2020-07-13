using System;
using System.Diagnostics;
using System.Linq;
using QA.DotNetCore.Engine.Abstractions.Wildcard;
using Xunit;
using Xunit.Abstractions;

namespace QA.DotNetCore.Engine.QpData.Tests
{
    public class WildcardMatcherTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public WildcardMatcherTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private static IWildcardMatcher Create(WildcardMatchingOption option, params string[] items)
        {
            return new WildcardMatcher(option, items);
        }

        [Fact]
        public void Test_WildCardMatcher()
        {
            var matcher = Create(WildcardMatchingOption.StartsWith, "bee.ru",
                "*.bee.ru",
                "*.stage.bee.ru",
                "stage.bee.ru",
                "stage.*.ru",
                "*");

            Assert.Equal("*.bee.ru", matcher.MatchLongest("msc.bee.ru"));
            Assert.Equal("stage.bee.ru", matcher.MatchLongest("stage.bee.ru"));
            Assert.Equal("*.bee.ru", matcher.MatchLongest("msc.bee.ru"));
            Assert.Equal("stage.*.ru", matcher.MatchLongest("stage.123.ru"));
            Assert.Equal("stage.*.ru", matcher.MatchLongest("stage.1232344.ru"));
            Assert.Equal("*.bee.ru", matcher.MatchLongest("msc.bee.ru"));
            Assert.Equal("*", matcher.MatchLongest("ee.ru"));
            Assert.Equal("*.bee.ru", matcher.MatchLongest("moskovskaya-obl.bee.ru"));
        }

        [Fact]
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

        [Fact]
        public void Test_WildCardMatcher_Issue01_Incorrect()
        {
            var matcher = Create(WildcardMatchingOption.FullMatch,
                "bee.ru",
                "*.bee.ru",
                "stage.bee.ru"
            );

            Assert.Equal("*.bee.ru", matcher.MatchLongest("msc.bee.ru"));
            Assert.Equal("*.bee.ru", matcher.MatchLongest("www.bee.ru"));
            Assert.Null(matcher.MatchLongest("bee.ru.artq.com"));

        }


        [Fact]
        public void Test_WildCardMatcher_Issue02_Incorrect_one_letter()
        {
            var matcher = Create(WildcardMatchingOption.FullMatch,
                "bee.ru",
                "*.bee.ru",
                "f.bee.ru"
            );

            Assert.Equal("*.bee.ru", matcher.MatchLongest("msc.bee.ru"));
            Assert.Equal("f.bee.ru", matcher.MatchLongest("f.bee.ru"));
            Assert.Null(matcher.MatchLongest("bee.ru.artq.com"));
        }

        [Fact]
        public void Test_WildCardMatcher_UrlPatterns()
        {
            var matcher = Create(WildcardMatchingOption.FullMatch,
                "/page1*",
                "/page2/test*",
                "/page3*/details/*"
            );

            Assert.True(matcher.Match("/page1/page3").Any());
            Assert.True(matcher.Match("/page1").Any());

            Assert.False(matcher.Match("/page2").Any());
            Assert.True(matcher.Match("/page2/test").Any());
            Assert.True(matcher.Match("/page2/test/test2").Any());
            Assert.False(matcher.Match("/page2/tes").Any());

            Assert.True(matcher.Match("/page3/details/123").Any());
            Assert.True(matcher.Match("/page3/a/b/c/details/123").Any());
            Assert.False(matcher.Match("/page3/a/b/c/details").Any());
            Assert.False(matcher.Match("/page3/a/b/c/detail/123").Any());
        }
    }
}
