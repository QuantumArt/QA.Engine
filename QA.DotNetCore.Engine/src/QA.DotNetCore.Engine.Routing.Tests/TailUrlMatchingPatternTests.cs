using Microsoft.VisualStudio.TestTools.UnitTesting;
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
