using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace AngiesList.Redis.Tests
{
    [TestClass] // MStest
    [TestFixture] // NUnit
    public class SSJsonSerializerTests
    {

        public class TestType
        {
            public string Name { get; set; }
            public string SomeValue { get; set; }
        }

        [TestMethod]
        [Test]
        public void SimpleObjectRoundTrip()
        {
            var serializer = new SSJsonSerializer();

            var testObj = new TestType() { Name = "Foo", SomeValue = "Bar" };

            var serialized = serializer.Serialize(testObj);

            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.TypeOf<TestType>());

            Assert.That(((TestType)deserialized).Name, Is.EqualTo(testObj.Name));
            Assert.That(((TestType)deserialized).SomeValue, Is.EqualTo(testObj.SomeValue));

        }

        [TestMethod]
        [Test]
        public void DeserializeEmptyBytesReturnsNull()
        {
            var serializer = new SSJsonSerializer();
            byte[] bytes = {};

            var deserialized = serializer.Deserialize(bytes);
            Assert.That(deserialized, Is.Null);
        }


    }
}
