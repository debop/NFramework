using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NSoft.NFramework.Reflections;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Core {

    [TestFixture]
    public class MongoCollectionFixture : MongoFixtureBase {

        private class TestClass {
            public ObjectId Id { get; set; }
            public int X { get; set; }
        }

        private const string CollectionName = "testcollection";
        private static MongoCollection<BsonDocument> Collection;

        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            Collection = Database[CollectionName];
        }

        [Test]
        public void CountTest() {
            Collection.RemoveAll();
            Collection.Count().Should().Be(0);

            Collection.Insert(new BsonDocument());
            Collection.Count().Should().Be(1);
        }

        [Test]
        public void CountWithQueryTest() {
            Collection.RemoveAll();

            Collection.Insert(new BsonDocument("x", 1));
            Collection.Insert(new BsonDocument("x", 2));

            Collection.Count(Query.EQ("x", 1)).Should().Be(1);
            Collection.Count(Query.EQ("x", 2)).Should().Be(1);
            Collection.Count(Query.EQ("x", 100)).Should().Be(0);
        }

        [Test]
        public void CreateIndexTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument());

            Collection.DropAllIndexes();

            var indexes = Collection.GetIndexes().ToArray();
            indexes.Length.Should().Be(1);
            indexes[0].RawDocument["name"].AsString.Should().Be("_id_");

            Collection.DropAllIndexes();

            Collection.CreateIndex("x");
            indexes = Collection.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be("_id_");
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1");

            //! 인덱스의 정렬과 Unique 도 설정할 수 있다
            Collection.DropAllIndexes();
            Collection.CreateIndex(IndexKeys.Ascending("x").Descending("y"), IndexOptions.SetUnique(true));

            indexes = Collection.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be("_id_");
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1_y_-1");
            indexes[1].RawDocument["unique"].ToBoolean().Should().Be.True();

            Collection.DropAllIndexes();
        }

        [Test(Description = "EnsureIndex는 인덱스가 없다면 새로 추가해줍니다.")]
        public void EnsureIndexTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument());

            Collection.DropAllIndexes();

            // 인덱스가 없으면 새로 생성합니다.
            //
            Collection.EnsureIndex("x");
            var indexes = Collection.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be("_id_");
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1");

            Collection.EnsureIndex("x");
            indexes = Collection.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be("_id_");
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1");
        }

        [Test]
        public void DistinctTest() {

            Collection.Insert(new BsonDocument());
            Collection.DropAllIndexes();
            Collection.RemoveAll();

            Collection.Insert(new BsonDocument("x", 1));
            Collection.Insert(new BsonDocument("x", 2));
            Collection.Insert(new BsonDocument("x", 3));
            Collection.Insert(new BsonDocument("x", 3));

            var values = new List<BsonValue>(Collection.Distinct("x"));

            values.Count.Should().Be(3);
            values.Contains(1).Should().Be.True();
            values.Contains(2).Should().Be.True();
            values.Contains(3).Should().Be.True();
            values.Contains(4).Should().Be.False();

            Collection.RemoveAll();
        }

        [Test]
        public void DistinctWithQueryTest() {

            Collection.Insert(new BsonDocument());
            Collection.DropAllIndexes();
            Collection.RemoveAll();

            Collection.Insert(new BsonDocument("x", 1));
            Collection.Insert(new BsonDocument("x", 2));
            Collection.Insert(new BsonDocument("x", 3));
            Collection.Insert(new BsonDocument("x", 3));

            var query = Query.LTE("x", 2);
            var values = new List<BsonValue>(Collection.Distinct("x", query));

            values.Count.Should().Be(2);
            values.Contains(1).Should().Be.True();
            values.Contains(2).Should().Be.True();
            values.Contains(3).Should().Be.False();
            values.Contains(4).Should().Be.False();

            Collection.RemoveAll();
        }

        [Test]
        public void DropAllIndexesTest() {

            Collection.Insert(new BsonDocument());
            Collection.DropAllIndexes();
            Collection.RemoveAll();
        }

        [Test]
        public void DropIndexTest() {
            Collection.Insert(new BsonDocument());
            Collection.DropAllIndexes();
            Collection.RemoveAll();

            var indexes = Collection.GetIndexes().ToArray();
            indexes.Length.Should().Be(1);
            indexes[0].RawDocument["name"].AsString.Should().Be("_id_");

            Assert.Throws<MongoCommandException>(() => Collection.DropIndex("x"));

            Collection.CreateIndex("x");
            Collection.GetIndexes().Count().Should().Be(2);

            Collection.DropIndex("x");
            Collection.GetIndexes().Count().Should().Be(1);
        }

        [Test(Description = "결과를 반환하는게 아니라, 요청 Command가 실행되는 상세 내용을 설명합니다.")]
        public void ExplainTest() {
            Collection.RemoveAll();

            Collection.Insert(new BsonDocument { { "x", 1 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 2 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 3 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 4 }, { "y", 2 } });

            var result = Collection.Find(Query.GT("x", 3)).Explain();
            Console.WriteLine("Explain result=" + result);
        }

        [Test]
        public void FindTest() {
            Collection.RemoveAll();

            Collection.Insert(new BsonDocument { { "x", 1 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 2 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 3 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 4 }, { "y", 2 } });

            var result = Collection.Find(Query.GT("x", 3));
            result.Count().Should().Be(1);
            result.Select(r => r["x"].AsInt32).FirstOrDefault().Should().Be(4);
        }

        [Test]
        public void FindAndModifyTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument { { "_id", 1 }, { "priority", 1 }, { "inprogress", false }, { "name", "abc" } });
            Collection.Insert(new BsonDocument { { "_id", 2 }, { "priority", 2 }, { "inprogress", false }, { "name", "def" } });

            var query = Query.EQ("inprogress", false);
            var sortBy = SortBy.Descending("priority");
            var started = DateTime.UtcNow;
            started = started.ToMongoDateTime(); // adjust for MongoDB DateTime precision

            var update = Update.Set("inprogress", true).Set("started", started);
            var result = Collection.FindAndModify(query, sortBy, update, false); // 마지막 인자가 false이면 기존 값을, true이면 수정된 값을 반환한다.

            result.Ok.Should().Be.True();
            result.ModifiedDocument["_id"].AsInt32.Should().Be(2);
            result.ModifiedDocument["priority"].AsInt32.Should().Be(2);
            result.ModifiedDocument["inprogress"].AsBoolean.Should().Be.False();
            result.ModifiedDocument["name"].AsString.Should().Be("def");
            result.ModifiedDocument.Contains("started").Should().Be.False();

            started = DateTime.UtcNow;
            started = started.ToMongoDateTime(); // adjust for MongoDB DateTime precision
            update = Update.Set("inprogress", true).Set("started", started);
            result = Collection.FindAndModify(query, sortBy, update, true); // 갱신된 값을 반환합니다.

            result.Ok.Should().Be.True();
            result.ModifiedDocument["_id"].AsInt32.Should().Be(1);
            result.ModifiedDocument["priority"].AsInt32.Should().Be(1);
            result.ModifiedDocument["inprogress"].AsBoolean.Should().Be.True();
            result.ModifiedDocument["name"].AsString.Should().Be("abc");
            result.ModifiedDocument.Contains("started").Should().Be.True();
            result.ModifiedDocument["started"].AsDateTime.Should().Be(started);
        }

        [Test]
        public void FindAndModifyNoMatchingDocumentTest() {

            Collection.RemoveAll();

            var query = Query.EQ("inprogress", false);
            var sortBy = SortBy.Descending("priority");
            var started = DateTime.UtcNow;
            started = started.ToMongoDateTime(); // adjust for MongoDB DateTime precision

            var update = Update.Set("inprogress", true).Set("started", started);
            var result = Collection.FindAndModify(query, sortBy, update, false); // return old

            result.Ok.Should().Be.True();
            result.ErrorMessage.Should().Be.NullOrEmpty();

            // 기존 자료가 없었음.
            result.ModifiedDocument.Should().Be.Null();
            result.GetModifiedDocumentAs<FindAndModifyClass>().Should().Be.Null();
        }

        [Test(Description = "Upsert - 매칭되는 정보가 없으면 추가한다")]
        public void FindAndModifyUpsertTest() {

            Collection.RemoveAll();

            var query = Query.EQ("name", "Tom");
            var sortBy = SortBy.Null;
            var update = Update.Inc("count", 1);
            var result = Collection.FindAndModify(query, sortBy, update, true, true); // upsert

            // 새로 추가되었다.
            Collection.Count().Should().Be(1);
            result.ModifiedDocument["name"].AsString.Should().Be("Tom");
            result.ModifiedDocument["count"].AsInt32.Should().Be(1);
        }

        private class FindAndModifyClass {
            [BsonId] public ObjectId Id;

            public int Value;
        }

        [Test]
        public void FindAndModifyTypedTest() {
            Collection.RemoveAll();

            var obj = new FindAndModifyClass { Id = ObjectId.GenerateNewId(), Value = 1 };
            Collection.Insert(obj);

            var query = Query.EQ("_id", obj.Id);
            var sortBy = SortBy.Null;
            var update = Update.Inc("Value", 1);
            var result = Collection.FindAndModify(query, sortBy, update, true); // returnNew

            var rehydrated = result.GetModifiedDocumentAs<FindAndModifyClass>();
            rehydrated.Should().Not.Be.Null();
            rehydrated.Id.Should().Be(obj.Id);
            rehydrated.Value.Should().Be(obj.Value + 1);
            //Assert.AreEqual(obj.Id, rehydrated.Id);
            //Assert.AreEqual(2, rehydrated.Value);
        }

        [Test]
        public void FindAndRemoveNoMatchingDocumentTest() {
            Collection.RemoveAll();

            var query = Query.EQ("inprogress", false);
            var sortBy = SortBy.Descending("priority");
            var result = Collection.FindAndRemove(query, sortBy);

            result.Ok.Should().Be.True();
            result.ErrorMessage.Should().Be.Null();
            result.ModifiedDocument.Should().Be.Null();
            result.GetModifiedDocumentAs<FindAndModifyClass>().Should().Be.Null();
        }

        [Test]
        public void FindNearSphericalFalseTest() {
            if(Collection.Exists())
                Collection.Drop();

            var places = new Place[]
                         {
                             new Place { Location = new[] { -74.0, 40.74 }, Name = "10gen", Type = "Office" },
                             new Place { Location = new[] { -75.0, 40.74 }, Name = "20Two", Type = "Coffee" },
                             new Place { Location = new[] { -74.0, 41.73 }, Name = "30Three", Type = "Coffee" }
                         };

            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var query = Query.Near("Location", -74.0, 40.74);
            var hits = Collection.FindAs<Place>(query).OrderBy(p => p.Name).ToArray();

            hits.Length.Should().Be(places.Length);

            for(int i = 0; i < hits.Length; i++) {
                hits[i].Location.SequenceEqual(places[i].Location).Should().Be.True();
                hits[i].Name.Should().Be(places[i].Name);
                hits[i].Type.Should().Be(places[i].Type);
            }

            // 근접 거리 검색에 최대 거리를 지정함.
            query = Query.Near("Location", -74.0, 40.74, 0.5); // with maxDistance
            hits = Collection.FindAs<Place>(query).OrderBy(p => p.Name).ToArray();

            hits.Length.Should().Be(1);
            hits[0].GetHashCode().Should().Be(places[0].GetHashCode());

            // 아주 먼 거리로 검색
            query = Query.Near("Location", -174.0, 40.74, 0.5); // with no hits
            hits = Collection.FindAs<Place>(query).ToArray();
            Assert.AreEqual(0, hits.Length);
        }

        [Test]
        public void TestFindNearSphericalTrue() {
            if(IsServer_1_7_or_Higher == false)
                return;

            if(Collection.Exists()) {
                Collection.Drop();
            }

            var places = new Place[]
                         {
                             new Place { Location = new[] { -74.0, 40.74 }, Name = "10gen", Type = "Office" },
                             new Place { Location = new[] { -75.0, 40.74 }, Name = "Two", Type = "Coffee" },
                             new Place { Location = new[] { -74.0, 41.73 }, Name = "Three", Type = "Coffee" }
                         };

            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var query = Query.Near("Location", -74.0, 40.74, double.MaxValue, true); // spherical search (거리순으로 정렬)
            var hits = Collection.FindAs<Place>(query).ToArray();
            hits.Length.Should().Be(places.Length);

            for(int i = 0; i < hits.Length; i++) {
                hits[i].Location.SequenceEqual(places[i].Location).Should().Be.True();
                hits[i].Name.Should().Be(places[i].Name);
                hits[i].Type.Should().Be(places[i].Type);
            }

            // 근접 거리 검색에 최대 거리를 지정함.
            query = Query.Near("Location", -74.0, 40.74, 0.5); // with maxDistance
            hits = Collection.FindAs<Place>(query).OrderBy(p => p.Name).ToArray();

            hits.Length.Should().Be(1);
            hits[0].GetHashCode().Should().Be(places[0].GetHashCode());

            // 아주 먼 거리로 검색
            query = Query.Near("Location", -174.0, 40.74, 0.5); // with no hits
            hits = Collection.FindAs<Place>(query).ToArray();
            Assert.AreEqual(0, hits.Length);
        }

        [Test]
        public void FindOneTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument { { "x", 1 }, { "y", 2 } });

            var result = Collection.FindOne();
            result["x"].AsInt32.Should().Be(1);
            result["y"].AsInt32.Should().Be(2);

            // NHibernate의 FindOne과는 다르다. FindFirst 개념이다.
            Collection.Insert(new BsonDocument { { "x", 3 }, { "y", 4 } });

            result = Collection.FindOne();
            result["x"].AsInt32.Should().Be(1);
            result["y"].AsInt32.Should().Be(2);
        }

        [Test]
        public void FindOneAsTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument { { "X", 1 } });
            var result = Collection.FindOneAs(typeof(TestClass));

            result.Should().Be.InstanceOf<TestClass>();
            ((TestClass)result).X.Should().Be(1);
        }

        [Test]
        public void FindOneAsGenericTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument { { "X", 1 } });
            var result = Collection.FindOneAs<TestClass>();
            result.Should().Not.Be.Null();
            result.X.Should().Be(1);
        }

        [Test]
        public void FindOneByIdTest() {
            Collection.RemoveAll();
            var id = ObjectId.GenerateNewId();
            Collection.Insert(new BsonDocument { { "_id", id }, { "x", 1 }, { "y", 2 } });
            var result = Collection.FindOneById(id);
            Assert.AreEqual(1, result["x"].AsInt32);
            Assert.AreEqual(2, result["y"].AsInt32);
        }

        [Test]
        public void FindOneByIdAsTest() {
            Collection.RemoveAll();
            var id = ObjectId.GenerateNewId();
            Collection.Insert(new BsonDocument { { "_id", id }, { "X", 1 } });
            var result = (TestClass)Collection.FindOneByIdAs(typeof(TestClass), id);
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(1, result.X);
        }

        [Test]
        public void FindOneByIdAsGenericTest() {
            Collection.RemoveAll();
            var id = ObjectId.GenerateNewId();
            Collection.Insert(new BsonDocument { { "_id", id }, { "X", 1 } });
            var result = Collection.FindOneByIdAs<TestClass>(id);
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(1, result.X);
        }

        [Test]
        public void FindWithinCircleSphericalFalseTest() {
            if(Collection.Exists()) {
                Collection.Drop();
            }

            var places = new Place[]
                         {
                             new Place { Location = new[] { -74.0, 40.74 }, Name = "10gen", Type = "Office" },
                             new Place { Location = new[] { -75.0, 40.74 }, Name = "Two", Type = "Coffee" },
                             new Place { Location = new[] { -74.0, 41.73 }, Name = "Three", Type = "Coffee" }
                         };

            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var query = Query.WithinCircle("Location", -74.0, 40.74, 1.0, false); // not spherical
            var hits = Collection.FindAs<Place>(query).ToArray();
            Assert.AreEqual(3, hits.Length);
            // note: the hits are unordered

            query = Query.WithinCircle("Location", -74.0, 40.74, 0.5, false); // smaller radius
            hits = Collection.FindAs<Place>(query).ToArray();
            Assert.AreEqual(1, hits.Length);

            query = Query.WithinCircle("Location", -174.0, 40.74, 1.0, false); // different part of the world
            hits = Collection.FindAs<Place>(query).ToArray();
            Assert.AreEqual(0, hits.Length);
        }

        [Test]
        public void FindWithinCircleSphericalTrueTest() {
            if(IsServer_1_7_or_Higher == false)
                return;

            if(Collection.Exists()) {
                Collection.Drop();
            }

            var places = new Place[]
                         {
                             new Place { Location = new[] { -74.0, 40.74 }, Name = "10gen", Type = "Office" },
                             new Place { Location = new[] { -75.0, 40.74 }, Name = "Two", Type = "Coffee" },
                             new Place { Location = new[] { -74.0, 41.73 }, Name = "Three", Type = "Coffee" }
                         };

            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var query = Query.WithinCircle("Location", -74.0, 40.74, 0.1, true); // spherical
            var hits = Collection.FindAs<Place>(query).ToArray(); // NOTE: hits 정보는 정렬되지 않았습니다.
            hits.Length.Should().Be(3);


            query = Query.WithinCircle("Location", -74.0, 40.74, 0.01, false); // smaller radius
            hits = Collection.FindAs<Place>(query).ToArray();
            hits.Length.Should().Be(1);

            query = Query.WithinCircle("Location", -174.0, 40.74, 0.1, false); // different part of the world
            hits = Collection.FindAs<Place>(query).ToArray();
            hits.Length.Should().Be(0);
        }

        [Test]
        public void FindWithinRectangleTest() {
            if(Collection.Exists()) {
                Collection.Drop();
            }

            var places = new Place[]
                         {
                             new Place { Location = new[] { -74.0, 40.74 }, Name = "10gen", Type = "Office" },
                             new Place { Location = new[] { -75.0, 40.74 }, Name = "Two", Type = "Coffee" },
                             new Place { Location = new[] { -74.0, 41.73 }, Name = "Three", Type = "Coffee" }
                         };

            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var query = Query.WithinRectangle("Location", -75.0, 40, -73.0, 42.0);
            var hits = Collection.FindAs<Place>(query).ToArray(); // NOTE: hits 정보는 정렬되지 않았습니다.
            hits.Length.Should().Be(3);
        }

#pragma warning disable 649 // never assigned to
        private class Place {
            public ObjectId Id;
            public double[] Location;
            public string Name;
            public string Type;

            public override int GetHashCode() {
                return HashTool.Compute(Location.CollectionToString(), Name, Type);
            }
        }
#pragma warning restore

        [Test]
        public void GeoNearTest() {
            if(Collection.Exists())
                Collection.Drop();

            var places = new Place[]
                         {
                             new Place { Location = new[] { 1.0, 1.0 }, Name = "One", Type = "Museum" },
                             new Place { Location = new[] { 1.0, 2.0 }, Name = "Two", Type = "Coffee" },
                             new Place { Location = new[] { 1.0, 3.0 }, Name = "Three", Type = "Library" },
                             new Place { Location = new[] { 1.0, 4.0 }, Name = "Four", Type = "Museum" },
                             new Place { Location = new[] { 1.0, 5.0 }, Name = "Five", Type = "Coffee" }
                         };

            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var options = GeoNearOptions
                .SetDistanceMultiplier(1)
                .SetMaxDistance(100);

            var result = Collection.GeoNearAs<Place>(Query.Null, 0.0, 0.0, 100, options);

            result.Ok.Should().Be.True();
            result.Namespace.Should().Be(DefaultDatabaseName + "." + CollectionName);

            Assert.IsTrue(result.Stats.AverageDistance >= 0.0);
            Assert.IsTrue(result.Stats.BTreeLocations >= 0);
            Assert.IsTrue(result.Stats.Duration >= TimeSpan.Zero);
            Assert.IsTrue(result.Stats.MaxDistance >= 0.0);
            Assert.IsTrue(result.Stats.NumberScanned >= 0);
            Assert.IsTrue(result.Stats.ObjectsLoaded >= 0);
            Assert.AreEqual(5, result.Hits.Count);
            Assert.IsTrue(result.Hits[0].Distance > 1.0);
            Assert.AreEqual(1.0, result.Hits[0].RawDocument["Location"].AsBsonArray[0].AsDouble);
            Assert.AreEqual(1.0, result.Hits[0].RawDocument["Location"].AsBsonArray[1].AsDouble);
            Assert.AreEqual("One", result.Hits[0].RawDocument["Name"].AsString);
            Assert.AreEqual("Museum", result.Hits[0].RawDocument["Type"].AsString);

            var place = (Place)result.Hits[1].Document;
            Assert.AreEqual(1.0, place.Location[0]);
            Assert.AreEqual(2.0, place.Location[1]);
            Assert.AreEqual("Two", place.Name);
            Assert.AreEqual("Coffee", place.Type);
        }

        [Test]
        public void GeoNearGenericTest() {
            if(Collection.Exists())
                Collection.Drop();

            var places = new Place[]
                         {
                             new Place { Location = new[] { 1.0, 1.0 }, Name = "One", Type = "Museum" },
                             new Place { Location = new[] { 1.0, 2.0 }, Name = "Two", Type = "Coffee" },
                             new Place { Location = new[] { 1.0, 3.0 }, Name = "Three", Type = "Library" },
                             new Place { Location = new[] { 1.0, 4.0 }, Name = "Four", Type = "Museum" },
                             new Place { Location = new[] { 1.0, 5.0 }, Name = "Five", Type = "Coffee" }
                         };

            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var options = GeoNearOptions
                .SetDistanceMultiplier(1)
                .SetMaxDistance(100);
            var result = Collection.GeoNearAs<Place>(Query.Null, 0.0, 0.0, 100, options);

            result.Ok.Should().Be.True();
            result.Namespace.Should().Be(DefaultDatabaseName + "." + CollectionName);

            Assert.IsTrue(result.Stats.AverageDistance >= 0.0);
            Assert.IsTrue(result.Stats.BTreeLocations >= 0);
            Assert.IsTrue(result.Stats.Duration >= TimeSpan.Zero);
            Assert.IsTrue(result.Stats.MaxDistance >= 0.0);
            Assert.IsTrue(result.Stats.NumberScanned >= 0);
            Assert.IsTrue(result.Stats.ObjectsLoaded >= 0);
            Assert.AreEqual(5, result.Hits.Count);
            Assert.IsTrue(result.Hits[0].Distance > 1.0);
            Assert.AreEqual(1.0, result.Hits[0].RawDocument["Location"].AsBsonArray[0].AsDouble);
            Assert.AreEqual(1.0, result.Hits[0].RawDocument["Location"].AsBsonArray[1].AsDouble);
            Assert.AreEqual("One", result.Hits[0].RawDocument["Name"].AsString);
            Assert.AreEqual("Museum", result.Hits[0].RawDocument["Type"].AsString);

            var place = result.Hits[1].Document;
            Assert.AreEqual(1.0, place.Location[0]);
            Assert.AreEqual(2.0, place.Location[1]);
            Assert.AreEqual("Two", place.Name);
            Assert.AreEqual("Coffee", place.Type);
        }

        [Test]
        public void GeoNearSphericalFalseTest() {
            if(Collection.Exists())
                Collection.Drop();

            var places = new Place[]
                         {
                             new Place { Location = new[] { -74.0, 40.74 }, Name = "10gen", Type = "Office" },
                             new Place { Location = new[] { -75.0, 40.74 }, Name = "Two", Type = "Coffee" },
                             new Place { Location = new[] { -74.0, 41.73 }, Name = "Three", Type = "Coffee" }
                         };
            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var options = GeoNearOptions.SetSpherical(false);
            var result = Collection.GeoNearAs<Place>(Query.Null, -74.0, 40.74, 100, options);

            result.Ok.Should().Be.True();
            result.Namespace.Should().Be(DefaultDatabaseName + "." + CollectionName);

            Assert.IsTrue(result.Stats.AverageDistance >= 0.0);
            Assert.IsTrue(result.Stats.BTreeLocations >= 0);
            Assert.IsTrue(result.Stats.Duration >= TimeSpan.Zero);
            Assert.IsTrue(result.Stats.MaxDistance >= 0.0);
            Assert.IsTrue(result.Stats.NumberScanned >= 0);
            Assert.IsTrue(result.Stats.ObjectsLoaded >= 0);
            Assert.AreEqual(3, result.Hits.Count);

            var hit0 = result.Hits[0];
            Assert.IsTrue(Math.Abs(hit0.Distance - 0.0) < double.Epsilon);
            Assert.AreEqual(-74.0, hit0.RawDocument["Location"].AsBsonArray[0].AsDouble);
            Assert.AreEqual(40.74, hit0.RawDocument["Location"].AsBsonArray[1].AsDouble);
            Assert.AreEqual("10gen", hit0.RawDocument["Name"].AsString);
            Assert.AreEqual("Office", hit0.RawDocument["Type"].AsString);

            // with spherical false "Three" is slightly closer than "Two"
            var hit1 = result.Hits[1];
            Assert.IsTrue(hit1.Distance > 0.0);
            Assert.AreEqual(-74.0, hit1.RawDocument["Location"].AsBsonArray[0].AsDouble);
            Assert.AreEqual(41.73, hit1.RawDocument["Location"].AsBsonArray[1].AsDouble);
            Assert.AreEqual("Three", hit1.RawDocument["Name"].AsString);
            Assert.AreEqual("Coffee", hit1.RawDocument["Type"].AsString);

            var hit2 = result.Hits[2];
            Assert.IsTrue(hit2.Distance > 0.0);
            Assert.IsTrue(hit2.Distance > hit1.Distance);
            Assert.AreEqual(-75.0, hit2.RawDocument["Location"].AsBsonArray[0].AsDouble);
            Assert.AreEqual(40.74, hit2.RawDocument["Location"].AsBsonArray[1].AsDouble);
            Assert.AreEqual("Two", hit2.RawDocument["Name"].AsString);
            Assert.AreEqual("Coffee", hit2.RawDocument["Type"].AsString);
        }

        [Test]
        public void GeoNearSphericalTrueTest() {
            if(IsServer_1_7_or_Higher == false)
                return;

            if(Collection.Exists())
                Collection.Drop();

            var places = new Place[]
                         {
                             new Place { Location = new[] { -74.0, 40.74 }, Name = "10gen", Type = "Office" },
                             new Place { Location = new[] { -75.0, 40.74 }, Name = "Two", Type = "Coffee" },
                             new Place { Location = new[] { -74.0, 41.73 }, Name = "Three", Type = "Coffee" }
                         };
            Collection.InsertBatch<Place>(places);
            Collection.CreateIndex(IndexKeys.GeoSpatial("Location"));

            var options = GeoNearOptions.SetSpherical(true);
            var result = Collection.GeoNearAs<Place>(Query.Null, -74.0, 40.74, 100, options);

            result.Ok.Should().Be.True();
            result.Namespace.Should().Be(DefaultDatabaseName + "." + CollectionName);

            Assert.IsTrue(result.Stats.AverageDistance >= 0.0);
            Assert.IsTrue(result.Stats.BTreeLocations >= 0);
            Assert.IsTrue(result.Stats.Duration >= TimeSpan.Zero);
            Assert.IsTrue(result.Stats.MaxDistance >= 0.0);
            Assert.IsTrue(result.Stats.NumberScanned >= 0);
            Assert.IsTrue(result.Stats.ObjectsLoaded >= 0);
            Assert.AreEqual(3, result.Hits.Count);

            var hit0 = result.Hits[0];
            Assert.IsTrue(Math.Abs(hit0.Distance - 0.0) < double.Epsilon);
            Assert.AreEqual(-74.0, hit0.RawDocument["Location"].AsBsonArray[0].AsDouble);
            Assert.AreEqual(40.74, hit0.RawDocument["Location"].AsBsonArray[1].AsDouble);
            Assert.AreEqual("10gen", hit0.RawDocument["Name"].AsString);
            Assert.AreEqual("Office", hit0.RawDocument["Type"].AsString);

            // with spherical true "Two" is considerably closer than "Three"
            var hit1 = result.Hits[1];
            Assert.IsTrue(hit1.Distance > 0.0);
            Assert.AreEqual(-75.0, hit1.RawDocument["Location"].AsBsonArray[0].AsDouble);
            Assert.AreEqual(40.74, hit1.RawDocument["Location"].AsBsonArray[1].AsDouble);
            Assert.AreEqual("Two", hit1.RawDocument["Name"].AsString);
            Assert.AreEqual("Coffee", hit1.RawDocument["Type"].AsString);

            var hit2 = result.Hits[2];
            Assert.IsTrue(hit2.Distance > 0.0);
            Assert.IsTrue(hit2.Distance > hit1.Distance);
            Assert.AreEqual(-74.0, hit2.RawDocument["Location"].AsBsonArray[0].AsDouble);
            Assert.AreEqual(41.73, hit2.RawDocument["Location"].AsBsonArray[1].AsDouble);
            Assert.AreEqual("Three", hit2.RawDocument["Name"].AsString);
            Assert.AreEqual("Coffee", hit2.RawDocument["Type"].AsString);
        }

        [Test]
        public void GetIndexesTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument());
            Collection.DropAllIndexes();
            var indexes = Collection.GetIndexes().ToArray();
            Assert.AreEqual(1, indexes.Length);
            Assert.AreEqual("_id_", indexes[0].RawDocument["name"].AsString);

            Collection.RemoveAll();
        }

        [Test]
        public void GetMoreTest() {
            //! 동일 Connection으로 내부 코드를 처리하도록 하기 위해서
            //
            using(Server.RequestStart(Database)) {
                Collection.RemoveAll();
                var count = Server.Primary.MaxMessageLength / 100000;

                if(IsDebugEnabled)
                    log.Debug("Database에 [{0}]개의 Document를 추가합니다.", count);

                for(var i = 0; i < count; i++) {
                    var document = new BsonDocument("data", new BsonBinaryData(new byte[1000000]));
                    Collection.Insert(document);
                }

                var list = Collection.FindAll().ToList();
                list.Count.Should().Be(count);
            }
            Collection.RemoveAll();
        }

        [Test]
        public void GroupTest() {
            Collection.RemoveAll();

            Collection.Insert(new BsonDocument("x", 1));
            Collection.Insert(new BsonDocument("x", 1));
            Collection.Insert(new BsonDocument("x", 2));
            Collection.Insert(new BsonDocument("x", 3));
            Collection.Insert(new BsonDocument("x", 3));
            Collection.Insert(new BsonDocument("x", 3));

            var initial = new BsonDocument("count", 0);
            const string reduce = "function(doc, prev) { prev.count += 1 }";
            var results = Collection.Group(Query.Null, "x", initial, reduce, null).ToArray();

            results.Length.Should().Be(3);

            results[0]["x"].ToInt32().Should().Be(1);
            results[0]["count"].ToInt32().Should().Be(2);

            results[1]["x"].ToInt32().Should().Be(2);
            results[1]["count"].ToInt32().Should().Be(1);

            results[2]["x"].ToInt32().Should().Be(3);
            results[2]["count"].ToInt32().Should().Be(3);
        }

        [Test]
        public void TestGroupByFunction() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument("x", 1));
            Collection.Insert(new BsonDocument("x", 1));
            Collection.Insert(new BsonDocument("x", 2));
            Collection.Insert(new BsonDocument("x", 3));
            Collection.Insert(new BsonDocument("x", 3));
            Collection.Insert(new BsonDocument("x", 3));

            var keyFunction = (BsonJavaScript)"function(doc) { return { x : doc.x }; }";
            var initial = new BsonDocument("count", 0);
            var reduce = (BsonJavaScript)"function(doc, prev) { prev.count += 1 }";

            var results = Collection.Group(Query.Null, keyFunction, initial, reduce, null).ToArray();

            results.Length.Should().Be(3);

            results[0]["x"].ToInt32().Should().Be(1);
            results[0]["count"].ToInt32().Should().Be(2);

            results[1]["x"].ToInt32().Should().Be(2);
            results[1]["count"].ToInt32().Should().Be(1);

            results[2]["x"].ToInt32().Should().Be(3);
            results[2]["count"].ToInt32().Should().Be(3);
        }

        [Test]
        public void IndexExistsTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument());

            Collection.DropAllIndexes();
            Assert.AreEqual(false, Collection.IndexExists("x"));

            Collection.CreateIndex("x");
            Assert.AreEqual(true, Collection.IndexExists("x"));

            Collection.CreateIndex(IndexKeys.Ascending("y"));
            Assert.AreEqual(true, Collection.IndexExists(IndexKeys.Ascending("y")));

            Collection.RemoveAll();
        }

#pragma warning disable 0649 // never assigned to
        private class TestMapReduceDocument {
            public string Id;

            [BsonElement("value")] public TestMapReduceValue Value;
        }

        private class TestMapReduceValue {
            [BsonElement("count")] public int Count;
        }
#pragma warning restore

        [Test]
        public void MapReduceTest() {
            // this is Example 1 on p. 87 of MongoDB: The Definitive Guide
            // by Kristina Chodorow and Michael Dirolf

            Collection.RemoveAll();
            Collection.Insert(new BsonDocument { { "A", 1 }, { "B", 2 } });
            Collection.Insert(new BsonDocument { { "B", 1 }, { "C", 2 } });
            Collection.Insert(new BsonDocument { { "X", 1 }, { "B", 2 } });

            const string map = "function() {\n" +
                               "    for (var key in this) {\n" +
                               "        emit(key, {count : 1});\n" +
                               "    }\n" +
                               "}\n";

            const string reduce = "function(key, emits) {\n" +
                                  "    total = 0;\n" +
                                  "    for (var i in emits) {\n" +
                                  "        total += emits[i].count;\n" +
                                  "    }\n" +
                                  "    return {count : total};\n" +
                                  "}\n";

            var options = MapReduceOptions.SetOutput("mrout");
            var result = Collection.MapReduce(map, reduce, options);
            Assert.IsTrue(result.Ok);
            Assert.IsTrue(result.Duration >= TimeSpan.Zero);
            Assert.AreEqual(9, result.EmitCount);
            Assert.AreEqual(5, result.OutputCount);
            Assert.AreEqual(3, result.InputCount);
            Assert.IsNotNullOrEmpty(result.CollectionName);

            var expectedCounts = new Dictionary<string, int>
                                 {
                                     { "A", 1 },
                                     { "B", 3 },
                                     { "C", 1 },
                                     { "X", 1 },
                                     { "_id", 3 }
                                 };

            // read output collection ourselves
            foreach(var document in Database[result.CollectionName].FindAll()) {
                var key = document["_id"].AsString;
                var count = document["value"].AsBsonDocument["count"].ToInt32();
                Assert.AreEqual(expectedCounts[key], count);
            }

            // test GetResults
            foreach(var document in result.GetResults()) {
                var key = document["_id"].AsString;
                var count = document["value"].AsBsonDocument["count"].ToInt32();
                Assert.AreEqual(expectedCounts[key], count);
            }

            // test GetResultsAs<>
            foreach(var document in result.GetResultsAs<TestMapReduceDocument>()) {
                Assert.AreEqual(expectedCounts[document.Id], document.Value.Count);
            }
        }

        [Test]
        public void MapReduceInlineTest() {
            // this is Example 1 on p. 87 of MongoDB: The Definitive Guide
            // by Kristina Chodorow and Michael Dirolf

            if(Server.BuildInfo.Version >= new Version(1, 7, 4, 0)) {
                Collection.RemoveAll();
                Collection.Insert(new BsonDocument { { "A", 1 }, { "B", 2 } });
                Collection.Insert(new BsonDocument { { "B", 1 }, { "C", 2 } });
                Collection.Insert(new BsonDocument { { "X", 1 }, { "B", 2 } });

                const string map = "function() {\n" +
                                   "    for (var key in this) {\n" +
                                   "        emit(key, {count : 1});\n" +
                                   "    }\n" +
                                   "}\n";

                const string reduce = "function(key, emits) {\n" +
                                      "    total = 0;\n" +
                                      "    for (var i in emits) {\n" +
                                      "        total += emits[i].count;\n" +
                                      "    }\n" +
                                      "    return {count : total};\n" +
                                      "}\n";

                var result = Collection.MapReduce(map, reduce);
                Assert.IsTrue(result.Ok);
                Assert.IsTrue(result.Duration >= TimeSpan.Zero);
                Assert.AreEqual(9, result.EmitCount);
                Assert.AreEqual(5, result.OutputCount);
                Assert.AreEqual(3, result.InputCount);
                Assert.IsNullOrEmpty(result.CollectionName);

                var expectedCounts = new Dictionary<string, int>
                                     {
                                         { "A", 1 },
                                         { "B", 3 },
                                         { "C", 1 },
                                         { "X", 1 },
                                         { "_id", 3 }
                                     };

                // test InlineResults as BsonDocuments
                foreach(var document in result.InlineResults) {
                    var key = document["_id"].AsString;
                    var count = document["value"].AsBsonDocument["count"].ToInt32();
                    Assert.AreEqual(expectedCounts[key], count);
                }

                // test InlineResults as TestInlineResultDocument
                foreach(var document in result.GetInlineResultsAs<TestMapReduceDocument>()) {
                    var key = document.Id;
                    var count = document.Value.Count;
                    Assert.AreEqual(expectedCounts[key], count);
                }

                // test GetResults
                foreach(var document in result.GetResults()) {
                    var key = document["_id"].AsString;
                    var count = document["value"].AsBsonDocument["count"].ToInt32();
                    Assert.AreEqual(expectedCounts[key], count);
                }

                // test GetResultsAs<>
                foreach(var document in result.GetResultsAs<TestMapReduceDocument>()) {
                    Assert.AreEqual(expectedCounts[document.Id], document.Value.Count);
                }
            }
        }

        [Test]
        public void MapReduceInlineWithQueryTest() {
            // this is Example 1 on p. 87 of MongoDB: The Definitive Guide
            // by Kristina Chodorow and Michael Dirolf

            if(Server.BuildInfo.Version >= new Version(1, 7, 4, 0)) {
                Collection.RemoveAll();
                Collection.Insert(new BsonDocument { { "A", 1 }, { "B", 2 } });
                Collection.Insert(new BsonDocument { { "B", 1 }, { "C", 2 } });
                Collection.Insert(new BsonDocument { { "X", 1 }, { "B", 2 } });

                var query = Query.Exists("B", true);

                const string map = "function() {\n" +
                                   "    for (var key in this) {\n" +
                                   "        emit(key, {count : 1});\n" +
                                   "    }\n" +
                                   "}\n";

                const string reduce = "function(key, emits) {\n" +
                                      "    total = 0;\n" +
                                      "    for (var i in emits) {\n" +
                                      "        total += emits[i].count;\n" +
                                      "    }\n" +
                                      "    return {count : total};\n" +
                                      "}\n";

                var result = Collection.MapReduce(query, map, reduce);
                Assert.IsTrue(result.Ok);
                Assert.IsTrue(result.Duration >= TimeSpan.Zero);
                Assert.AreEqual(9, result.EmitCount);
                Assert.AreEqual(5, result.OutputCount);
                Assert.AreEqual(3, result.InputCount);
                Assert.IsNullOrEmpty(result.CollectionName);

                var expectedCounts = new Dictionary<string, int>
                                     {
                                         { "A", 1 },
                                         { "B", 3 },
                                         { "C", 1 },
                                         { "X", 1 },
                                         { "_id", 3 }
                                     };

                // test InlineResults as BsonDocuments
                foreach(var document in result.InlineResults) {
                    var key = document["_id"].AsString;
                    var count = document["value"].AsBsonDocument["count"].ToInt32();
                    Assert.AreEqual(expectedCounts[key], count);
                }

                // test InlineResults as TestInlineResultDocument
                foreach(var document in result.GetInlineResultsAs<TestMapReduceDocument>()) {
                    var key = document.Id;
                    var count = document.Value.Count;
                    Assert.AreEqual(expectedCounts[key], count);
                }

                // test GetResults
                foreach(var document in result.GetResults()) {
                    var key = document["_id"].AsString;
                    var count = document["value"].AsBsonDocument["count"].ToInt32();
                    Assert.AreEqual(expectedCounts[key], count);
                }

                // test GetResultsAs<>
                foreach(var document in result.GetResultsAs<TestMapReduceDocument>()) {
                    Assert.AreEqual(expectedCounts[document.Id], document.Value.Count);
                }
            }
        }

        [Test]
        public void ReIndexTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument("x", 1));
            Collection.Insert(new BsonDocument("x", 2));

            Collection.DropAllIndexes();
            Collection.CreateIndex("x");

            // note: prior to 1.8.1 the reIndex command was returning duplicate ok elements
            try {
                var result = Collection.ReIndex();
                Assert.AreEqual(2, result.Response["nIndexes"].ToInt32());
                Assert.AreEqual(2, result.Response["nIndexesWas"].ToInt32());
            }
            catch(InvalidOperationException ex) {
                Assert.AreEqual("Duplicate element name 'ok'.", ex.Message);
            }
        }

        [Test]
        public void SetFieldsTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument { { "x", 1 }, { "y", 2 } });

            var result = Collection.FindAll().SetFields("x").FirstOrDefault();

            // Id 와 설정한 필드만 가져온다.
            Assert.AreEqual(2, result.ElementCount);
            Assert.AreEqual("_id", result.GetElement(0).Name);
            Assert.AreEqual("x", result.GetElement(1).Name);
        }

        [Test]
        public void SetHintTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument { { "x", 1 }, { "y", 2 } });
            Collection.DropAllIndexes();

            Collection.CreateIndex(IndexKeys.Ascending("x"));

            var query = Query.EQ("x", 1);
            var cursor = Collection.Find(query).SetHint(new BsonDocument("x", 1));

            var count = 0;
            foreach(var document in cursor) {
                Assert.AreEqual(1, ++count);
                Assert.AreEqual(1, document["x"].AsInt32);
            }
        }

        [Test]
        public void SetHintByIndexNameTest() {
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument { { "x", 1 }, { "y", 2 } });
            Collection.DropAllIndexes();

            Collection.CreateIndex(IndexKeys.Ascending("x"), IndexOptions.SetName("xIndex"));

            var query = Query.EQ("x", 1);
            var cursor = Collection.Find(query).SetHint("xIndex");
            var count = 0;
            foreach(var document in cursor) {
                Assert.AreEqual(1, ++count);
                Assert.AreEqual(1, document["x"].AsInt32);
            }
        }

        [Test]
        public void SortAndLimitTest() {
            Collection.RemoveAll();

            Collection.Insert(new BsonDocument { { "x", 4 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 2 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 3 }, { "y", 2 } });
            Collection.Insert(new BsonDocument { { "x", 1 }, { "y", 2 } });

            var result = Collection.FindAll().SetSortOrder("x").SetLimit(3).Select(doc => doc["x"].AsInt32);

            Assert.AreEqual(3, result.Count());
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
        }

        [Test]
        [Ignore("collstats command 를 아직 지원하지 않습니다.")]
        public void GetStatsTest() {
            Collection.RemoveAll();
            var stats = Collection.GetStats();

            stats.Ok.Should().Be.True();
            stats.ErrorMessage.Should().Not.Be.NullOrEmpty();

            if(IsDebugEnabled)
                log.Debug("StorageSize=[{0}], TotalIndexSize=[{1}]", stats.StorageSize, stats.TotalIndexSize);
        }

        [Test]
        [Ignore("collstats command 를 아직 지원하지 않습니다.")]
        public void TotalDataSizeTest() {
            var dataSize = Collection.GetTotalDataSize();
            dataSize.Should().Be.GreaterThanOrEqualTo(0);
        }

        [Test]
        [Ignore("collstats command 를 아직 지원하지 않습니다.")]
        public void TotalStorageSizeTest() {
            var storageSize = Collection.GetTotalStorageSize();
            storageSize.Should().Be.GreaterThan(0);
        }

        [Test]
        public void ValidateTest() {
            // ensure collection exists
            Collection.RemoveAll();
            Collection.Insert(new BsonDocument("x", 1));

            var result = Collection.Validate();
            var ns = result.Namespace;
            var firstExtent = result.FirstExtent;
            var lastExtent = result.LastExtent;
            var extentCount = result.ExtentCount;
            var dataSize = result.DataSize;
            var nrecords = result.RecordCount;
            var lastExtentSize = result.LastExtentSize;
            var padding = result.Padding;
            var firstExtentDetails = result.FirstExtentDetails;
            var loc = firstExtentDetails.Loc;
            var xnext = firstExtentDetails.XNext;
            var xprev = firstExtentDetails.XPrev;
            var nsdiag = firstExtentDetails.NSDiag;
            var size = firstExtentDetails.Size;
            var firstRecord = firstExtentDetails.FirstRecord;
            var lastRecord = firstExtentDetails.LastRecord;
            var deletedCount = result.DeletedCount;
            var deletedSize = result.DeletedSize;
            var nindexes = result.IndexCount;
            var keysPerIndex = result.KeysPerIndex;
            var valid = result.IsValid;
            var errors = result.Errors;
            var warning = result.Warning;
        }
    }
}