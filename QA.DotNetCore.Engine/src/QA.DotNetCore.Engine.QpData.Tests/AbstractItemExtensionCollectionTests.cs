using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace QA.DotNetCore.Engine.QpData.Tests
{
    public class AbstractItemExtensionCollectionTests
    {
        [Theory]
        [InlineData("Key", "Value")]
        public void TestStringValues(string key, object val)
        {
            var extCollection = new AbstractItemExtensionCollection();
            extCollection.Add(key, val);

            Assert.Equal(val, extCollection.Get(key, typeof(string)));
            Assert.Equal(val, extCollection.Get(key.ToLowerInvariant(), typeof(string)));
            Assert.Equal(val, extCollection.Get(key.ToUpperInvariant(), typeof(string)));
        }

        [Theory]
        [InlineData("21", true)]
        [InlineData("-321", true)]
        [InlineData("27.00", false)]
        [InlineData("ewewf", false)]
        public void TestIntValues(string val, bool expectedSuccess)
        {
            var key = "Key";
            var extCollection = new AbstractItemExtensionCollection();
            extCollection.Add(key, val);

            var converted = Int32.TryParse(val, out int intVal);
            Assert.Equal(converted, expectedSuccess);
            if (converted)
            {
                Assert.Equal(intVal, extCollection.Get(key, typeof(int)));
                Assert.Equal(intVal, extCollection.Get(key, typeof(int?)));
            }
            else
            {
                Assert.Throws<FormatException>(() => extCollection.Get(key, typeof(int)));
            }
        }

        [Theory]
        [InlineData("1", false)]
        [InlineData("0", false)]
        [InlineData("true", true)]
        [InlineData("false", true)]
        [InlineData("TRUE", true)]
        [InlineData("FALSE", true)]
        [InlineData("dfgr", false)]
        public void TestBoolValues(string val, bool expectedSuccess)
        {
            var key = "Key";
            var extCollection = new AbstractItemExtensionCollection();
            extCollection.Add(key, val);

            var converted = Boolean.TryParse(val, out bool bVal);
            Assert.Equal(converted, expectedSuccess);
            if (converted)
            {
                Assert.Equal(bVal, extCollection.Get(key, typeof(bool)));
                Assert.Equal(bVal, extCollection.Get(key, typeof(bool?)));
            }
            else
            {
                Assert.Throws<FormatException>(() => extCollection.Get(key, typeof(bool)));
            }
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("1,00", true)]
        [InlineData("-32,4325", true)]
        [InlineData("12.34", false)]
        [InlineData("dfgr", false)]
        public void TestDoubleValues(string val, bool expectedSuccess)
        {
            var key = "Key";
            var extCollection = new AbstractItemExtensionCollection();
            extCollection.Add(key, val);

            var converted = Double.TryParse(val, out double dVal);
            Assert.Equal(converted, expectedSuccess);
            if (converted)
            {
                Assert.Equal(dVal, extCollection.Get(key, typeof(double)));
                Assert.Equal(dVal, extCollection.Get(key, typeof(double?)));
            }
            else
            {
                Assert.Throws<FormatException>(() => extCollection.Get(key, typeof(double)));
            }
        }

        [Theory]
        [InlineData("2014-08-26 00:00:00", true)]
        [InlineData("8/26/2014 12:00:00 AM", false)]
        [InlineData("dfgr", false)]
        public void TestDatetimeValues(string val, bool expectedSuccess)
        {
            var key = "Key";
            var extCollection = new AbstractItemExtensionCollection();
            extCollection.Add(key, val);

            var converted = DateTime.TryParse(val, out DateTime dVal);
            Assert.Equal(converted, expectedSuccess);
            if (converted)
            {
                Assert.Equal(dVal, extCollection.Get(key, typeof(DateTime)));
                Assert.Equal(dVal, extCollection.Get(key, typeof(DateTime?)));
            }
            else
            {
                Assert.Throws<FormatException>(() => extCollection.Get(key, typeof(DateTime)));
            }
        }
    }
}
