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
            Assert.That(filter.Match(page), Is.False);
        }

        [Test]
        public void Test_ZoneMustMatch()
        {
            var filter = new WidgetFilter("SomeZone", "/");

            var widget1 = new StubWidget("SomeZone", null, null);
            Assert.That(filter.Match(widget1), Is.True);

            var widget2 = new StubWidget("AnotherZone", null, null);
            Assert.That(filter.Match(widget2), Is.False);
        }

        [Test]
        public void Test_DeniedPatternsMustWork()
        {
            const string zone = "SomeZone";
            var widget = new StubWidget(zone, null,
                new[] { "/", "page1/*", "page2*", "page3", "/page11/*", "/page12*", "/page13" });

            var filter = new WidgetFilter(zone, "/");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/foo/bar");
            Assert.That(filter.Match(widget), Is.True);

            filter = new WidgetFilter(zone, "/page1");
            Assert.That(filter.Match(widget), Is.True);

            filter = new WidgetFilter(zone, "/page1/page2");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page11");
            Assert.That(filter.Match(widget), Is.True);

            filter = new WidgetFilter(zone, "/page11/page2");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page2");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page2/page3");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page12");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page12/page3");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page3");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page3/page4");
            Assert.That(filter.Match(widget), Is.True);

            filter = new WidgetFilter(zone, "/page13");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page13/page4");
            Assert.That(filter.Match(widget), Is.True);
        }

        [Test]
        public void Test_AllowedPatternsMustWork()
        {
            const string zone = "SomeZone";
            var widget = new StubWidget(zone, new[] { "/page1*" },
                new[] { "*/details/*" });

            var filter = new WidgetFilter(zone, "/foo/bar");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page1");
            Assert.That(filter.Match(widget), Is.True);

            filter = new WidgetFilter(zone, "/page1/a/b/c");
            Assert.That(filter.Match(widget), Is.True);

            filter = new WidgetFilter(zone, "/page1/details/c");
            Assert.That(filter.Match(widget), Is.False);

            filter = new WidgetFilter(zone, "/page1/a/b/details/c");
            Assert.That(filter.Match(widget), Is.False);
        }
    }
}
