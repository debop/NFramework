﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using NUnit.Framework;

namespace NSoft.NFramework.Data.MongoDB.Samples {
    [TestFixture]
    public class MagicDiscriminatorFixture : MongoFixtureBase {
        [BsonKnownTypes(typeof(B), typeof(C))]
        private class A {
            static A() {
                BsonDefaultSerializer.RegisterDiscriminatorConvention(typeof(A), new MagicDiscriminatorConvention());
            }

            public string InA { get; set; }
        }

        [BsonIgnoreExtraElements] // ignore _id
        private class B : A {
            public string OnlyInB { get; set; }
        }

        [BsonIgnoreExtraElements] // ignore _id
        private class C : A {
            public string OnlyInC { get; set; }
        }

        private class MagicDiscriminatorConvention : IDiscriminatorConvention {
            public string ElementName {
                get { return null; }
            }

            public Type GetActualType(
                BsonReader bsonReader,
                Type nominalType
                ) {
                var bookmark = bsonReader.GetBookmark();
                bsonReader.ReadStartDocument();
                var actualType = nominalType;
                while(bsonReader.ReadBsonType() != BsonType.EndOfDocument) {
                    var name = bsonReader.ReadName();
                    if(name == "OnlyInB") {
                        actualType = typeof(B);
                        break;
                    }
                    else if(name == "OnlyInC") {
                        actualType = typeof(C);
                        break;
                    }
                    bsonReader.SkipValue();
                }
                bsonReader.ReturnToBookmark(bookmark);
                return actualType;
            }

            public BsonValue GetDiscriminator(
                Type nominalType,
                Type actualType
                ) {
                return null;
            }
        }

        private MongoServer server;
        private MongoDatabase database;
        private MongoCollection<A> collection;

        [TestFixtureSetUp]
        public void TestFixtureSetup() {
            server = MongoServer.Create("mongodb://localhost/?safe=true");
            database = server["test"];
            collection = database.GetCollection<A>("magicdiscriminator");
        }

        [Test]
        public void TestBAsA() {
            var b = new B { InA = "a", OnlyInB = "b" };

            var json = b.ToJson();
            var expected = "{ 'InA' : 'a', 'OnlyInB' : 'b' }".Replace("'", "\""); // note: no _t discriminator!
            Assert.AreEqual(expected, json);

            collection.RemoveAll();
            collection.Insert(b);
            var copy = (B)collection.FindOne();
            Assert.IsInstanceOf<B>(copy);
            Assert.AreEqual("a", copy.InA);
            Assert.AreEqual("b", copy.OnlyInB);
        }

        [Test]
        public void TestCAsA() {
            var c = new C { InA = "a", OnlyInC = "c" };

            var json = c.ToJson();
            var expected = "{ 'InA' : 'a', 'OnlyInC' : 'c' }".Replace("'", "\""); // note: no _t discriminator!
            Assert.AreEqual(expected, json);

            collection.RemoveAll();
            collection.Insert(c);
            var copy = (C)collection.FindOne();
            Assert.IsInstanceOf<C>(copy);
            Assert.AreEqual("a", copy.InA);
            Assert.AreEqual("c", copy.OnlyInC);
        }
    }
}