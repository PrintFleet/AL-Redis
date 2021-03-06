﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace AngiesList.Redis.Tests
{
    [TestFixture] 
    public class SSJsonSerializerTests
    {

        public class TestType
        {
            public string Name { get; set; }
            public string SomeValue { get; set; }
        }

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


        [Test]
        public void DictionaryRoundTrip()
        {
            var serializer = new SSJsonSerializer();

            var testObj = new Dictionary<String, TestType>();
            testObj.Add("Item1", new TestType() { Name = "Foo1", SomeValue = "Bar2" });
            testObj.Add("Item2", new TestType() { Name = "Foo2", SomeValue = "Bar2" });
            
            var serialized = serializer.Serialize(testObj);

            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.TypeOf<Dictionary<String, TestType>>());

            Assert.That(((Dictionary<String, TestType>)deserialized).Count, Is.EqualTo(2));
            Assert.That(((Dictionary<String, TestType>)deserialized).ContainsKey("Item1"), Is.True);
            Assert.That(((Dictionary<String, TestType>)deserialized)["Item1"].Name, Is.EqualTo("Foo1"));
            Assert.That(((Dictionary<String, TestType>)deserialized)["Item2"].SomeValue, Is.EqualTo("Bar2"));
        }


        //[TestMethod]
        //[Test]
        //public void DictionaryComplexKeyRoundTrip()
        //{
        //    var serializer = new SSJsonSerializer();

        //    var testObj = new Dictionary<TestType, String>();
        //    testObj.Add(new TestType() { Name = "Foo1", SomeValue = "Bar2" }, "Item1");
        //    testObj.Add(new TestType() { Name = "Foo2", SomeValue = "Bar2" }, "Item2");

        //    var serialized = serializer.Serialize(testObj);

        //    NOTE: this fails currently, due to limitations in JSON serializer 
        //    var deserialized = serializer.Deserialize(serialized);

        //    Assert.That(deserialized, Is.TypeOf<Dictionary<TestType, String>>());

        //    Assert.That(((Dictionary<String, TestType>)deserialized).Count, Is.EqualTo(2));
        //}



        [Test]
        public void DeserializeEmptyBytesReturnsNull()
        {
            var serializer = new SSJsonSerializer();
            byte[] bytes = {};

            var deserialized = serializer.Deserialize(bytes);
            Assert.That(deserialized, Is.Null);
        }


        [Test]
        public void EmptyStringRoundTrip()
        {
            var serializer = new SSJsonSerializer();

            string testObj = string.Empty;

            var serialized = serializer.Serialize(testObj);

            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.TypeOf<string>());
            Assert.That(((string)deserialized), Is.EqualTo(string.Empty));
        }
    }
}
