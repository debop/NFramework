using System;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Repositories {
    [TestFixture]
    public class MongoRepositoryImplFixture : MongoFixtureBase {
        private readonly object _syncLock = new object();

        private MongoRepositoryImpl _repository;

        public MongoRepositoryImpl Repository {
            get {
                if(_repository == null)
                    lock(_syncLock)
                        if(_repository == null) {
                            var repository = new MongoRepositoryImpl(DefaultConnectionString);
                            Thread.MemoryBarrier();
                            _repository = repository;
                        }
                return _repository;
            }
        }

        protected override void OnTestFixtureTearDown() {
            base.OnTestFixtureTearDown();

            Repository.DropAllCollection();
        }

        [Test]
        public void CreateTest() {
            var repository = new MongoRepositoryImpl(DefaultConnectionString);
            repository.Should().Not.Be.Null();
            repository.Database.Should().Not.Be.Null();
            repository.DatabaseName.Should().Be(DefaultDatabaseName);
        }

        [Test]
        public void IsServerConnectedTest() {
            Repository.IsServerConnected.Should().Be.True();
            Repository.SafeMode.Should().Be(Repository.Server.Settings.SafeMode);
        }

        [Test]
        public void ReconnectTest() {
            Repository.IsServerConnected.Should().Be.True();

            Repository.Server.Disconnect();
            Repository.IsServerConnected.Should().Be.False();

            Repository.Server.Reconnect();
            Repository.IsServerConnected.Should().Be.True();
        }

        [Test]
        public void DatabaseExistsTest() {
            Repository.Insert(new BsonDocument());
            Repository.DatabaseExists(DefaultDatabaseName).Should().Be.True();
            Repository.RemoveAll();
        }

        [Test]
        public void DropDatabaseTest() {
            Repository.Insert(new BsonDocument());
            Repository.DatabaseExists(DefaultDatabaseName).Should().Be.True();

            Repository.DropDatabase(DefaultDatabaseName).Ok.Should().Be.True();

            Repository.Insert(new BsonDocument());
            Repository.DatabaseExists(DefaultDatabaseName).Should().Be.True();
            Repository.RemoveAll();
        }

        [Test]
        public void GetDatabaseTest() {
            Repository.Database.Should().Not.Be.Null();
            Repository.DatabaseName.Should().Be(DefaultDatabaseName);

            Repository.Insert(new BsonDocument());
            Repository.DatabaseExists(DefaultDatabaseName).Should().Be.True();
            Repository.RemoveAll();
        }

        [Test]
        [Ignore("CopyDatabase는 MongoServer.CopyDatabase()가 구현되지 않아서, 테스트에서 제외되었습니다.")]
        public void CopyDatabaseTest() {
            //const string CopyDatabaseName = DefaultDatabaseName + "-Copied";

            //// DefaultDatabase가 없다면 새로 생성합니다.
            //Repository.Insert(new BsonDocument());
            //Repository.CopyDatabase(DefaultDatabaseName, CopyDatabaseName);

            //Repository.DatabaseName = CopyDatabaseName;
            //Repository.DatabaseExists(CopyDatabaseName).Should().Be.True();
            //Repository.DropDatabase(CopyDatabaseName).Ok.Should().Be.True();
            //Repository.DatabaseExists(CopyDatabaseName).Should().Be.False();

            //Repository.DatabaseName = DefaultDatabaseName;
            //Repository.RemoveAll().Ok.Should().Be.True();
        }

        [Test]
        public void CreateCollectionTest() {
            Repository.CreateCollection(TestCollectionName).Ok.Should().Be.True();
            Repository.GetCollection(TestCollectionName).Exists().Should().Be.True();
            Repository.DropCollection(TestCollectionName).Ok.Should().Be.True();
        }

        [Test]
        public void DropCollecitonTest() {
            if(Repository.CollectionExists(TestCollectionName))
                Repository.DropCollection(TestCollectionName).Ok.Should().Be.True();

            Repository.CreateCollection(TestCollectionName).Ok.Should().Be.True();
            Repository.GetCollection(TestCollectionName).Exists().Should().Be.True();

            Repository.DropCollection(TestCollectionName).Ok.Should().Be.True();
        }

        [Test]
        public void CollectionExistsTest() {
            Repository.CreateCollection(TestCollectionName).Ok.Should().Be.True();
            Repository.CollectionExists(TestCollectionName).Should().Be.True();
            Repository.DropCollection(TestCollectionName).Ok.Should().Be.True();
        }

        [Test]
        public void GetCollectionTest() {
            if(Repository.CollectionExists(TestCollectionName))
                Repository.DropCollection(TestCollectionName).Ok.Should().Be.True();

            Repository.GetCollection(TestCollectionName).Should().Not.Be.Null();


            var posts = Repository.GetCollection(typeof(Post), "Post");
            posts.Should().Not.Be.Null();
            posts.Insert(new Post());
            posts.Exists().Should().Be.True();

            posts.Drop().Ok.Should().Be.True();

            var postsGeneric = Repository.GetCollectionAs<Post>("PostGeneric");
            postsGeneric.Should().Not.Be.Null();
            postsGeneric.Insert<Post>(new Post());

            postsGeneric.Drop().Ok.Should().Be.True();
        }

        [Test]
        public void RenameCollectionTest() {
            var coll = Repository.GetCollection("src");

            coll.Name.Should().Be("src");
            coll.Insert(new BsonDocument());

            Repository.RenameCollection("src", "dest").Ok.Should().Be.True();

            Repository.CollectionExists("src").Should().Be.False();
            Repository.CollectionExists("dest").Should().Be.True();

            coll = Repository.GetCollection("dest");
            coll.Name.Should().Be("dest");
            coll.Drop();
        }

        [Test]
        public void GetCollectionNamesTest() {
            Repository.CreateCollection("test");

            foreach(var name in Repository.GetAllCollectionNames())
                Console.WriteLine(name);
        }

        [Test]
        public void GetAllCollectionTest() {
            Repository.DropAllCollection();

            int currentCollectionCount = Repository.GetAllCollection().Count();

            const int collectionCount = 100;

            var prefix = Guid.NewGuid().ToString();
            var collectionNames = Enumerable.Range(0, 100).Select(i => prefix + "-" + i.ToString());

            foreach(var name in collectionNames)
                Repository.CreateCollection(name);

            var collections = Repository.GetAllCollection().ToArray();
            collections.Count().Should().Be.GreaterThanOrEqualTo(currentCollectionCount + collectionCount);


            Repository.DropAllCollection();

            collections = Repository.GetAllCollection().ToArray();
            collections.Count().Should().Be(0);
        }

        [Test]
        public void GetAllCollectionSettingsTest() {
            Repository.DropAllCollection();

            var prefix = Guid.NewGuid().ToString();
            var collectionNames = Enumerable.Range(0, 100).Select(i => prefix + "-" + i.ToString());

            foreach(var name in collectionNames)
                Repository.CreateCollection(name);

            var settings = Repository.GetAllCollectionSettings().ToArray();

            settings.All(setting => setting.SafeMode == global::MongoDB.Driver.SafeMode.True).Should().Be.True();
            settings.All(setting => Equals(setting.DefaultDocumentType, typeof(BsonDocument))).Should().Be.True();

            Repository.DropAllCollection();
        }

        [Test]
        public void CountTest() {
            const string CollectionName = "count-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            var coll = Repository.GetCollection(typeof(Person), CollectionName);
            coll.Count().Should().Be(0);

            Repository.CollectionName = CollectionName;
            Repository.Count().Should().Be(0);

            Repository.Insert<Person>(new Person());
            Repository.Count().Should().Be(1);

            Repository.Insert<Person>(new Person());
            Repository.Count().Should().Be(2);


            Repository.DropCollection(CollectionName);
            Repository.Count().Should().Be(0);
        }

        [Test]
        public void CountWithQueryTest() {
            const string CollectionName = "count-test-query";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            var coll = Repository.GetCollection(CollectionName);
            coll.Count().Should().Be(0);

            Repository.CollectionName = CollectionName;
            Repository.Count().Should().Be(0);

            var people = new Person[]
                         {
                             new Person { Name = "person1", Age = 10 },
                             new Person { Name = "person2", Age = 10 },
                             new Person { Name = "person3", Age = 20 },
                             new Person { Name = "person4", Age = 20 },
                             new Person { Name = "person5", Age = 30 }
                         };

            Repository.InsertBatch(people);

            Repository.Count(Query.EQ("Name", "person1")).Should().Be(1);
            Repository.Count(Query.EQ("Name", "person1")).Should().Be(1);

            Repository.Count(Query.In("Name", "person1", "person2")).Should().Be(2);


            Repository.DropCollection(CollectionName);
            Repository.Count().Should().Be(0);
        }

        [Test]
        public void CountTypedCollectionWithQueryTest() {
            const string CollectionName = "count-test-query-person";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            var coll = Repository.GetCollection(typeof(Person), CollectionName);
            coll.Count().Should().Be(0);

            Repository.CollectionName = CollectionName;
            Repository.Count().Should().Be(0);

            var people = new Person[]
                         {
                             new Person { Name = "person1", Age = 10 },
                             new Person { Name = "person2", Age = 10 },
                             new Person { Name = "person3", Age = 20 },
                             new Person { Name = "person4", Age = 20 },
                             new Person { Name = "person5", Age = 30 }
                         };

            Repository.InsertBatch(people);

            Repository.Count(Query.EQ("Name", "person1")).Should().Be(1);
            Repository.Count(Query.EQ("Name", "person2")).Should().Be(1);
            Repository.Count(Query.In("Name", "person1", "person2")).Should().Be(2);
            Repository.Count(Query.EQ("Name", "aaaaa")).Should().Be(0);
            Repository.Count(Query.EQ("test", "test")).Should().Be(0);


            Repository.DropCollection(CollectionName);
            Repository.Count().Should().Be(0);
        }

        [Test]
        public void DistinctTest() {
            const string CollectionName = "distinct-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            var coll = Repository.GetCollection(typeof(Person), CollectionName);
            coll.Count().Should().Be(0);

            Repository.CollectionName = CollectionName;
            Repository.Count().Should().Be(0);

            var people = new Person[]
                         {
                             new Person { Name = "person1", Age = 10 },
                             new Person { Name = "person2", Age = 10 },
                             new Person { Name = "person3", Age = 20 },
                             new Person { Name = "person4", Age = 20 },
                             new Person { Name = "person5", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var ages = Repository.Distinct("Age").ToList();

            ages.Count.Should().Be(3);
            ages.Contains(10).Should().Be.True();
            ages.Contains(20).Should().Be.True();
            ages.Contains(30).Should().Be.True();
            ages.Contains(40).Should().Be.False();


            Repository.DropCollection(CollectionName);
            Repository.Count().Should().Be(0);
        }

        [Test]
        public void DistinctWithQueryTest() {
            const string CollectionName = "distinct-query-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            var coll = Repository.GetCollection(typeof(Person), CollectionName);
            coll.Count().Should().Be(0);

            Repository.CollectionName = CollectionName;
            Repository.Count().Should().Be(0);

            var people = new Person[]
                         {
                             new Person { Name = "person1", Age = 10 },
                             new Person { Name = "person2", Age = 10 },
                             new Person { Name = "person3", Age = 20 },
                             new Person { Name = "person4", Age = 20 },
                             new Person { Name = "person5", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var ages = Repository.Distinct("Age", Query.GTE("Age", 20)).ToList();
            ages.Count.Should().Be(2);
            ages.Contains(10).Should().Be.False();
            ages.Contains(20).Should().Be.True();
            ages.Contains(30).Should().Be.True();
            ages.Contains(40).Should().Be.False();

            Repository.DropCollection(CollectionName);
            Repository.Count().Should().Be(0);
        }

        [Test]
        public void FindOneTest() {
            const string CollectionName = "findone-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            var coll = Repository.GetCollection(typeof(Person), CollectionName);
            coll.Count().Should().Be(0);

            Repository.CollectionName = CollectionName;
            Repository.Count().Should().Be(0);

            var people = new Person[]
                         {
                             new Person { Name = "person1", Age = 10 },
                             new Person { Name = "person2", Age = 10 },
                             new Person { Name = "person3", Age = 20 },
                             new Person { Name = "person4", Age = 20 },
                             new Person { Name = "person5", Age = 30 }
                         };

            Repository.InsertBatch(people);

            foreach(var person in people) {
                var p = Repository.FindOne(Query.EQ("Name", person.Name));

                p.Should().Not.Be.Null();
                p["Name"].AsString.Should().Be(person.Name);
                p["Age"].ToInt32().Should().Be(person.Age);
            }


            Repository.DropCollection(CollectionName);
            Repository.Count().Should().Be(0);
        }

        [Test]
        public void FindOneAsTest() {
            const string CollectionName = "findone-as-generic-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var coll = Repository.GetCollection(typeof(Person), CollectionName);
            coll.Count().Should().Be(0);

            Repository.CollectionName = CollectionName;
            Repository.Count().Should().Be(0);

            var people = new Person[]
                         {
                             new Person { Name = "person1", Age = 10 },
                             new Person { Name = "person2", Age = 10 },
                             new Person { Name = "person3", Age = 20 },
                             new Person { Name = "person4", Age = 20 },
                             new Person { Name = "person5", Age = 30 }
                         };

            Repository.InsertBatch(people);

            foreach(var person in people) {
                var p = (Person)Repository.FindOneAs(typeof(Person), Query.EQ("Name", person.Name));

                p.Should().Not.Be.Null();
                p.Name.Should().Be(person.Name);
                p.Age.Should().Be(person.Age);
            }

            Repository.DropCollection(CollectionName);
            Repository.Count().Should().Be(0);
        }

        [Test]
        public void FindOneAsGenericTest() {
            const string CollectionName = "findone-as-generic-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var coll = Repository.GetCollection(typeof(Person), CollectionName);
            coll.Count().Should().Be(0);

            Repository.CollectionName = CollectionName;
            Repository.Count().Should().Be(0);

            var people = new Person[]
                         {
                             new Person { Name = "person1", Age = 10 },
                             new Person { Name = "person2", Age = 10 },
                             new Person { Name = "person3", Age = 20 },
                             new Person { Name = "person4", Age = 20 },
                             new Person { Name = "person5", Age = 30 }
                         };

            Repository.InsertBatch(people);

            foreach(var person in people) {
                var p = Repository.FindOneAs<Person>(Query.EQ("Name", person.Name));

                p.Should().Not.Be.Null();
                p.Name.Should().Be(person.Name);
                p.Age.Should().Be(person.Age);
            }

            Repository.RemoveAll();
            Repository.Count().Should().Be(0);
        }

        [Test(Description = "값이 NUL 인 경우를 찾으려면, BsonNull.Value 를 사용해야 합니다!!!")]
        public void Find_Uses_BsonNull() {
            const string CollectionName = "find-uses-BsonNull";

            Repository.DropCollection(CollectionName);

            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var people = new Person[]
                         {
                             new Person { Name = null, Age = 10 },
                             new Person { Name = "DDD", Age = 10 },
                             new Person { Name = "BBB", Age = 20 },
                             new Person { Name = "CCC", Age = 20 },
                             new Person { Name = "AAA", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var result = Repository.Find(Query.EQ("Name", BsonNull.Value)).OrderByDescending(p => p["Name"]).Take(2).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(BsonNull.Value, result[0]["Name"]);

            result = Repository.Find(Query.NE("Name", BsonNull.Value)).OrderByDescending(p => p["Name"]).ToArray();
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("DDD", result[0]["Name"].ToString());

            Repository.RemoveAll();
            Repository.DropCollection(CollectionName);
        }

        [Test(Description = "값이 NUL 인 경우를 찾으려면, BsonNull.Value 를 사용해야 합니다!!!")]
        public void FindAs_Uses_BsonNull() {
            const string CollectionName = "find-as-type-BsonNull";

            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var people = new Person[]
                         {
                             new Person { Name = null, Age = 10 },
                             new Person { Name = "DDD", Age = 10 },
                             new Person { Name = "BBB", Age = 20 },
                             new Person { Name = "CCC", Age = 20 },
                             new Person { Name = "AAA", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var result =
                Repository.FindAs(typeof(Person), Query.EQ("Name", BsonNull.Value)).Cast<Person>().OrderByDescending(p => p.Name).Take(2)
                    .ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(null, result[0].Name);

            result =
                Repository.FindAs(typeof(Person), Query.NE("Name", BsonNull.Value)).Cast<Person>().OrderByDescending(p => p.Name).
                    ToArray();
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("DDD", result[0].Name);

            Repository.RemoveAll();
            Repository.DropCollection(CollectionName);
        }

        [Test(Description = "값이 NUL 인 경우를 찾으려면, BsonNull.Value 를 사용해야 합니다!!!")]
        public void FindAsGeneric_Uses_BsonNull() {
            const string CollectionName = "find-as-generic-BsonNull";

            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var people = new Person[]
                         {
                             new Person { Name = null, Age = 10 },
                             new Person { Name = "DDD", Age = 10 },
                             new Person { Name = "BBB", Age = 20 },
                             new Person { Name = "CCC", Age = 20 },
                             new Person { Name = "AAA", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var result = Repository.FindAs<Person>(Query.EQ("Name", BsonNull.Value)).OrderByDescending(p => p.Name).Take(2).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(null, result[0].Name);

            result = Repository.FindAs<Person>(Query.NE("Name", BsonNull.Value)).OrderByDescending(p => p.Name).ToArray();
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("DDD", result[0].Name);

            Repository.RemoveAll();
            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void FindAllTest() {
            const string CollectionName = "find-all";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var people = new Person[]
                         {
                             new Person { Name = null, Age = 10 },
                             new Person { Name = "DDD", Age = 10 },
                             new Person { Name = "BBB", Age = 20 },
                             new Person { Name = "CCC", Age = 20 },
                             new Person { Name = "AAA", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var found = Repository.FindAll().ToArray();

            found.Length.Should().Be(people.Length);
            found[0]["Name"].Should().Be(BsonNull.Value);

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void FindAllAsTest() {
            const string CollectionName = "find-all-as";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var people = new Person[]
                         {
                             new Person { Name = null, Age = 10 },
                             new Person { Name = "DDD", Age = 10 },
                             new Person { Name = "BBB", Age = 20 },
                             new Person { Name = "CCC", Age = 20 },
                             new Person { Name = "AAA", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var found = Repository.FindAllAs(typeof(Person)).Cast<Person>().ToArray();

            found.Length.Should().Be(people.Length);
            found[0].Name.Should().Be.Null();

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void FindAllAsGenericTest() {
            const string CollectionName = "find-all-as-generic";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var people = new Person[]
                         {
                             new Person { Name = null, Age = 10 },
                             new Person { Name = "DDD", Age = 10 },
                             new Person { Name = "BBB", Age = 20 },
                             new Person { Name = "CCC", Age = 20 },
                             new Person { Name = "AAA", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var found = Repository.FindAllAs<Person>().ToArray();

            found.Length.Should().Be(people.Length);
            found[0].Name.Should().Be.Null();

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void FindAndModifyTest() {
            const string CollectionName = "find-and-modify";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var people = new Person[]
                         {
                             new Person { Name = null, Age = 10 },
                             new Person { Name = "DDD", Age = 10 },
                             new Person { Name = "BBB", Age = 20 },
                             new Person { Name = "CCC", Age = 20 },
                             new Person { Name = "AAA", Age = 30 }
                         };

            Repository.InsertBatch(people);

            var query = Query.EQ("Name", "DDD");
            var update = Update.Set("Age", 50);

            var updated = Repository.FindAndModify(query, SortBy.Null, update, true);
            updated.Ok.Should().Be.True();
            updated.ModifiedDocument["Age"].AsInt32.Should().Be(50);

            query = Query.EQ("Name", "UPSERT");
            update = Update.Set("Age", 100);
            updated = Repository.FindAndModify(query, SortBy.Null, update, true, true);

            updated.Ok.Should().Be.True();
            updated.ModifiedDocument["Name"].AsString.Should().Be("UPSERT");
            updated.ModifiedDocument["Age"].AsInt32.Should().Be(100);

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void GroupTest() {
            const string CollectionName = "group-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            Repository.Insert(new BsonDocument("x", 1));
            Repository.Insert(new BsonDocument("x", 1));
            Repository.Insert(new BsonDocument("x", 2));
            Repository.Insert(new BsonDocument("x", 3));
            Repository.Insert(new BsonDocument("x", 3));
            Repository.Insert(new BsonDocument("x", 3));

            var initial = new BsonDocument("count", 0);
            const string reduce = "function(doc, prev) { prev.count += 1; }";
            var results = Repository.Group(Query.Null, "x", initial, reduce, null).ToArray();

            results.Length.Should().Be(3);

            results[0]["x"].ToInt32().Should().Be(1);
            results[0]["count"].ToInt32().Should().Be(2);

            results[1]["x"].ToInt32().Should().Be(2);
            results[1]["count"].ToInt32().Should().Be(1);

            results[2]["x"].ToInt32().Should().Be(3);
            results[2]["count"].ToInt32().Should().Be(3);
        }

        [Test]
        public void GroupByFunctionTest() {
            const string CollectionName = "group-by-function-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            Repository.Insert(new BsonDocument("x", 1));
            Repository.Insert(new BsonDocument("x", 1));
            Repository.Insert(new BsonDocument("x", 2));
            Repository.Insert(new BsonDocument("x", 3));
            Repository.Insert(new BsonDocument("x", 3));
            Repository.Insert(new BsonDocument("x", 3));

            var keyFunction = (BsonJavaScript)"function(doc) { return { x: doc.x }; }";
            var initial = new BsonDocument("count", 0);
            const string reduce = "function(doc, prev) { prev.count += 1; }";
            var results = Repository.Group(Query.Null, keyFunction, initial, reduce, null).ToArray();

            results.Length.Should().Be(3);

            results[0]["x"].ToInt32().Should().Be(1);
            results[0]["count"].ToInt32().Should().Be(2);

            results[1]["x"].ToInt32().Should().Be(2);
            results[1]["count"].ToInt32().Should().Be(1);

            results[2]["x"].ToInt32().Should().Be(3);
            results[2]["count"].ToInt32().Should().Be(3);
        }

        [Test]
        public void GroupByIMongoGroupByTest() {
            const string CollectionName = "group-by-IMongoGroupBy-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            Repository.Insert(new BsonDocument("x", 1));
            Repository.Insert(new BsonDocument("x", 1));
            Repository.Insert(new BsonDocument("x", 2));
            Repository.Insert(new BsonDocument("x", 3));
            Repository.Insert(new BsonDocument("x", 3));
            Repository.Insert(new BsonDocument("x", 3));


            var initial = new BsonDocument("count", 0);
            const string reduce = "function(doc, prev) { prev.count += 1; }";
            var results = Repository.Group(Query.Null, GroupBy.Keys("x"), initial, reduce, null).ToArray();

            results.Length.Should().Be(3);

            results[0]["x"].ToInt32().Should().Be(1);
            results[0]["count"].ToInt32().Should().Be(2);

            results[1]["x"].ToInt32().Should().Be(2);
            results[1]["count"].ToInt32().Should().Be(1);

            results[2]["x"].ToInt32().Should().Be(3);
            results[2]["count"].ToInt32().Should().Be(3);
        }

        [Test]
        public void InsertTest() {
            const string CollectionName = "insert-test";

            const int DocumentCount = 1000;
            var comment = "동해물과 백두산이 ".Replicate(100);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            using(new OperationTimer("Insert document - " + DocumentCount)) {
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new BsonDocument { { "x", x }, { "comment", comment } })
                    .RunEach(doc => Repository.Insert(doc));

                Assert.AreEqual(DocumentCount, Repository.Count());
            }

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void InsertAsTypeTest() {
            const string CollectionName = "insert-as-type-test";

            const int DocumentCount = 1000;
            var comment = "동해물과 백두산이 ".Replicate(100);
            var personType = typeof(Person);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            using(new OperationTimer("InsertAsType document - " + DocumentCount)) {
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x, Address = { Street = comment } })
                    .RunEach(doc => Repository.Insert(personType, doc));

                Assert.AreEqual(DocumentCount, Repository.Count());
            }

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void InsertAsGenericTest() {
            const string CollectionName = "insert-as-generic-test";

            const int DocumentCount = 1000;
            var comment = "동해물과 백두산이 ".Replicate(100);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            using(new OperationTimer("InsertAsType document - " + DocumentCount)) {
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x, Address = { Street = comment } })
                    .RunEach(doc => Repository.Insert<Person>(doc));

                Assert.AreEqual(DocumentCount, Repository.Count());
            }

            Repository.DropCollection(CollectionName);
        }

        [Test(Description = "InsertBatch가 Insert 보다 2배 이상 빠르네요.")]
        public void InsertBatch() {
            const string CollectionName = "insert-batch-test";

            const int DocumentCount = 1000;
            var comment = "동해물과 백두산이 ".Replicate(100);
            var personType = typeof(Person);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            using(new OperationTimer("InsertAsType document - " + DocumentCount)) {
                var people =
                    Enumerable.Range(0, DocumentCount)
                        .AsParallel()
                        .AsOrdered()
                        .Select(x => new Person { Name = "Person " + x, Age = x, Address = { Street = comment } });

                Repository.InsertBatch(personType, people.Cast<object>()).All(result => result.Ok).Should().Be.True();

                Assert.AreEqual(DocumentCount, Repository.Count());
            }

            Repository.DropCollection(CollectionName);
        }

        [Test(Description = "InsertBatch가 Insert 보다 2배 이상 빠르네요.")]
        public void InsertBatchGeneric() {
            const string CollectionName = "insert-batch-generic-test";

            const int DocumentCount = 1000;
            var comment = "동해물과 백두산이 ".Replicate(100);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            using(new OperationTimer("InsertAsType document - " + DocumentCount)) {
                var people =
                    Enumerable.Range(0, DocumentCount)
                        .AsParallel()
                        .AsOrdered()
                        .Select(x => new Person { Name = "Person " + x, Age = x, Address = { Street = comment } });

                Repository.InsertBatch(people).All(result => result.Ok).Should().Be.True();

                Assert.AreEqual(DocumentCount, Repository.Count());
            }

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void SaveAsTypeTest() {
            const string CollectionName = "save-test";

            const int DocumentCount = 1000;
            var comment = "동해물과 백두산이 ".Replicate(100);
            var personType = typeof(Person);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            using(new OperationTimer("InsertAsType document - " + DocumentCount)) {
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x, Address = { Street = comment } })
                    .RunEach(doc => Repository.Save(personType, doc));

                Assert.AreEqual(DocumentCount, Repository.Count());
            }

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void SaveAsTypeGenericTest() {
            const string CollectionName = "save-as-type-generic-test";

            const int DocumentCount = 1000;
            var comment = "동해물과 백두산이 ".Replicate(100);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            using(new OperationTimer("InsertAsType document - " + DocumentCount)) {
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x, Address = { Street = comment } })
                    .RunEach(doc => Repository.Save<Person>(doc));

                Assert.AreEqual(DocumentCount, Repository.Count());
            }

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void RemoveByQueryTest() {
            const string CollectionName = "remove-by-query-test";

            const int DocumentCount = 100;
            var personType = typeof(Person);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();


            var people =
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x });

            Repository.InsertBatch(personType, people.Cast<object>()).All(result => result.Ok).Should().Be.True();

            Assert.AreEqual(DocumentCount, Repository.Count());

            Repository.Remove(Query.GTE("Age", DocumentCount / 2));
            Repository.Count().Should().Be(DocumentCount / 2);

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void RemoveByIdTest() {
            const string CollectionName = "remove-by-id-test";

            const int DocumentCount = 100;
            var personType = typeof(Person);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();


            var people =
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x }).ToArray();

            Repository.InsertBatch(personType, people).All(result => result.Ok).Should().Be.True();

            Assert.AreEqual(DocumentCount, Repository.Count());

            people.Skip(DocumentCount / 2).Take(DocumentCount).RunEach(p => Repository.RemoveById(p.Id));
            Repository.Count().Should().Be(DocumentCount / 2);

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void RemoveByIdAsTypeTest() {
            const string CollectionName = "remove-by-id-as-type-test";

            const int DocumentCount = 100;
            var personType = typeof(Person);

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();


            var people =
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x }).ToArray();

            Repository.InsertBatch(personType, people).All(result => result.Ok).Should().Be.True();

            Assert.AreEqual(DocumentCount, Repository.Count());

            people.Skip(DocumentCount / 2).Take(DocumentCount).RunEach(p => Repository.RemoveByIdAs(personType, p.Id));
            Repository.Count().Should().Be(DocumentCount / 2);

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void RemoveByIdAsTypeGenericTest() {
            const string CollectionName = "remove-by-id-as-type-generic-test";

            const int DocumentCount = 100;

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();


            var people =
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x }).ToArray();

            Repository.InsertBatch<Person>(people).All(result => result.Ok).Should().Be.True();

            Assert.AreEqual(DocumentCount, Repository.Count());

            people.Skip(DocumentCount / 2).Take(DocumentCount).RunEach(p => Repository.RemoveByIdAs<Person>(p.Id));
            Repository.Count().Should().Be(DocumentCount / 2);

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void RemoveAllTest() {
            const string CollectionName = "remove-by-id-as-type-generic-test";

            const int DocumentCount = 100;

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();


            var people =
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Person { Name = "Person " + x, Age = x }).ToArray();

            Repository.InsertBatch<Person>(people).All(result => result.Ok).Should().Be.True();

            Repository.Count().Should().Be(DocumentCount);

            Repository.RemoveAll();
            Repository.Count().Should().Be(0);


            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void RemoveExpires() {
            const string CollectionName = "remove-expires-test";

            const int DocumentCount = 100;

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);
            Repository.RemoveAll();

            var createdDate = DateTime.Now.ToMongoDateTime();

            var posts =
                Enumerable.Range(0, DocumentCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => new Post { Title = "Post " + x, CreatedDate = createdDate.AddMilliseconds(2 * x) }).ToArray();

            Repository.InsertBatch<Post>(posts);
            Repository.Count().Should().Be(DocumentCount);

            var query = Query.LTE("CreatedDate", DateTime.Now.ToMongoDateTime());
            Repository.Remove(query);

            Thread.Sleep(DocumentCount * 5);

            query = Query.LTE("CreatedDate", DateTime.Now.ToMongoDateTime());
            Repository.Remove(query);

            Repository.Count().Should().Be(0);
        }
    }
}