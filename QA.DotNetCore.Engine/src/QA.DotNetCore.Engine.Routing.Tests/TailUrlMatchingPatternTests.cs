using Microsoft.VisualStudio.TestTools.UnitTesting;
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching.TailUrlMatchingPattern;

namespace QA.DotNetCore.Engine.Routing.Tests
{
    [TestClass]
    public class TailUrlMatchingPatternTests
    {
        [TestMethod]
        public void Test_Match_SimpleTemplate()
        {
            var pattern = new TailUrlMatchingPattern { Pattern = "{action=Index}/{id?}" };

            var mr = pattern.Match("");
            Assert.IsTrue(mr.IsMatch);
            Assert.AreEqual(mr.Values.Keys.Count, 1);
            Assert.IsTrue(mr.Values.ContainsKey("action"));
            Assert.AreEqual(mr.Values["action"], "Index");

            mr = pattern.Match("list");
            Assert.IsTrue(mr.IsMatch);
            Assert.AreEqual(mr.Values.Keys.Count, 1);
            Assert.IsTrue(mr.Values.ContainsKey("action"));
            Assert.AreEqual(mr.Values["action"], "list");

            mr = pattern.Match("details/123");
            Assert.IsTrue(mr.IsMatch);
            Assert.AreEqual(mr.Values.Keys.Count, 2);
            Assert.IsTrue(mr.Values.ContainsKey("action"));
            Assert.AreEqual(mr.Values["action"], "details");
            Assert.IsTrue(mr.Values.ContainsKey("id"));
            Assert.AreEqual(mr.Values["id"], "123");

            mr = pattern.Match("details/123/something");
            Assert.IsFalse(mr.IsMatch);
        }

        [TestMethod]
        public void Test_Match_CheckDefaults()
        {
            var pattern = new TailUrlMatchingPattern { Pattern = "{id}", Defaults = new Dictionary<string, string> { { "action", "Details" } } };

            var mr = pattern.Match("123");
            Assert.IsTrue(mr.IsMatch);
            Assert.AreEqual(mr.Values.Keys.Count, 2);
            Assert.IsTrue(mr.Values.ContainsKey("action"));
            Assert.AreEqual(mr.Values["action"], "Details");
            Assert.IsTrue(mr.Values.ContainsKey("id"));
            Assert.AreEqual(mr.Values["id"], "123");
        }

        [TestMethod]
        public void Test_Match_InvalidTemplate()
        {
            var pattern = new TailUrlMatchingPattern { Pattern = "{action=Index}/{id}" };

            Assert.ThrowsException<TailUrlMatchingPatternException>(() => pattern.Match(""));
        }

        [TestMethod]
        public void Test_Match_CheckConstraints_Success()
        {
            var pattern = new TailUrlMatchingPattern
            {
                Pattern = "tariff/{geoCode}/{tariffId}",
                Defaults = new Dictionary<string, string> { { "action", "Details" } },
                Constraints = new Dictionary<string, string> { { "tariffId", "^$|\\d{5,9}" }, { "geoCode", "[a-zA-z_\\-]*" } }
            };

            TailUrlMatchResult tailUrlMatchResult = pattern.Match("tariff/ru-RU/12345");

            Assert.IsTrue(tailUrlMatchResult.IsMatch);

            Assert.AreEqual(tailUrlMatchResult.Values.Keys.Count, 3);

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("action"));
            Assert.AreEqual(tailUrlMatchResult.Values["action"], "Details");

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("tariffId"));
            Assert.AreEqual(tailUrlMatchResult.Values["tariffId"], "12345");

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("geoCode"));
            Assert.AreEqual(tailUrlMatchResult.Values["geoCode"], "ru-RU");
        }

        [TestMethod]
        public void Test_Match_CheckConstraints_EmptyRegex_Success()
        {
            var pattern = new TailUrlMatchingPattern
            {
                Pattern = "tariff/{geoCode}/{tariffId}",
                Defaults = new Dictionary<string, string> { { "action", "Details" } },
                Constraints = new Dictionary<string, string> { { "tariffId", "^$" }, { "geoCode", "[a-zA-z_\\-]*" } }
            };

            TailUrlMatchResult tailUrlMatchResult = pattern.Match("tariff/ru-RU/");

            Assert.IsTrue(tailUrlMatchResult.IsMatch);

            Assert.AreEqual(tailUrlMatchResult.Values.Keys.Count, 3);

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("action"));
            Assert.AreEqual(tailUrlMatchResult.Values["action"], "Details");

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("tariffId"));
            Assert.AreEqual(tailUrlMatchResult.Values["tariffId"], string.Empty);

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("geoCode"));
            Assert.AreEqual(tailUrlMatchResult.Values["geoCode"], "ru-RU");
        }

        [TestMethod]
        public void Test_Match_CheckConstraints_Fail()
        {
            var pattern = new TailUrlMatchingPattern
            {
                Pattern = "tariff/{geoCode}/{tariffId}",
                Defaults = new Dictionary<string, string> { { "action", "Details" } },
                Constraints = new Dictionary<string, string> { { "tariffId", "^$|\\d{5,9}" }, { "geoCode", "[a-zA-z_\\-]*" } }
            };

            TailUrlMatchResult tailUrlMatchResult = pattern.Match("tariff/ru-RU/qwert");

            Assert.IsFalse(tailUrlMatchResult.IsMatch);
        }


        [TestMethod]
        public void Test_Match_VariativePattern_Success()
        {
            var pattern = new TailUrlMatchingPattern
            {
                Pattern = "{action[Archive|Popular]}/{categoryAlias}"
            };

            TailUrlMatchResult tailUrlMatchResult = pattern.Match("Archive/cat");

            Assert.IsTrue(tailUrlMatchResult.IsMatch);

            Assert.AreEqual(tailUrlMatchResult.Values.Keys.Count, 2);

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("action"));
            Assert.AreEqual(tailUrlMatchResult.Values["action"], "Archive");

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("categoryAlias"));
            Assert.AreEqual(tailUrlMatchResult.Values["categoryAlias"], "cat");

            tailUrlMatchResult = pattern.Match("Popular/cat");

            Assert.IsTrue(tailUrlMatchResult.IsMatch);

            Assert.AreEqual(tailUrlMatchResult.Values.Keys.Count, 2);

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("action"));
            Assert.AreEqual(tailUrlMatchResult.Values["action"], "Popular");

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("categoryAlias"));
            Assert.AreEqual(tailUrlMatchResult.Values["categoryAlias"], "cat");
        }

        [TestMethod]
        public void Test_Match_VariativePattern_Fail()
        {
            var pattern = new TailUrlMatchingPattern
            {
                Pattern = "{action[Archive|Popular]}/{categoryAlias}"
            };

            TailUrlMatchResult tailUrlMatchResult = pattern.Match("HomeOfCat/littleCat");

            Assert.IsFalse(tailUrlMatchResult.IsMatch);
        }

        [TestMethod]
        public void Test_Match_VariativePattern_EmptyVariativePattern()
        {
            var pattern = new TailUrlMatchingPattern
            {
                Pattern = "{action[]}/{categoryAlias}"
            };

            TailUrlMatchResult tailUrlMatchResult = pattern.Match("HomeOfCat/littleCat");

            Assert.IsTrue(tailUrlMatchResult.IsMatch);

            Assert.AreEqual(tailUrlMatchResult.Values.Keys.Count, 2);

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("action"));
            Assert.AreEqual(tailUrlMatchResult.Values["action"], "HomeOfCat");

            Assert.IsTrue(tailUrlMatchResult.Values.ContainsKey("categoryAlias"));
            Assert.AreEqual(tailUrlMatchResult.Values["categoryAlias"], "littleCat");
        }
    }
}
