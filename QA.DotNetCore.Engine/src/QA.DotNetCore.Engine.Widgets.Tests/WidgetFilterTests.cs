using NUnit.Framework;
using QA.DotNetCore.Engine.Widgets.Tests.FakePagesAndWidgets;

namespace QA.DotNetCore.Engine.Widgets.Tests
{
    public class WidgetFilterTests
    {
        [Test]
        public void Test_OnlyForWidgets()
        {
            var filter = new WidgetFilter("SomeZone", "/");
            var page = new StubPage();
            Assert.False(filter.Match(page));
        }

        [Test]
        public void Test_ZoneMustMatch()
        {
            var filter = new WidgetFilter("SomeZone", "/");

            var widget1 = new StubWidget("SomeZone", null, null);
            Assert.True(filter.Match(widget1));

            var widget2 = new StubWidget("AnotherZone", null, null);
            Assert.False(filter.Match(widget2));
        }

        [Test]
        public void Test_DeniedPatternsMustWork()
        {
            const string zone = "SomeZone";
            var widget = new StubWidget(zone, null,
                new string[] { "/", "page1/*", "page2*", "page3", "/page11/*", "/page12*", "/page13" });

            var filter = new WidgetFilter(zone, "/");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/foo/bar");
            Assert.True(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page1");
            Assert.True(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page1/page2");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page11");
            Assert.True(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page11/page2");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page2");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page2/page3");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page12");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page12/page3");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page3");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page3/page4");
            Assert.True(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page13");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page13/page4");
            Assert.True(filter.Match(widget));
        }

        [Test]
        public void Test_AllowedPatternsMustWork()
        {
            const string zone = "SomeZone";
            var widget = new StubWidget(zone, new string[] { "/page1*" },
                new string[] { "*/details/*" });

            var filter = new WidgetFilter(zone, "/foo/bar");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page1");
            Assert.True(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page1/a/b/c");
            Assert.True(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page1/details/c");
            Assert.False(filter.Match(widget));

            filter = new WidgetFilter(zone, "/page1/a/b/details/c");
            Assert.False(filter.Match(widget));
        }
    }
}
