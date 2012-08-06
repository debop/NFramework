using System;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Core {
    [TestFixture]
    public class MongoDatabaseFixture : MongoFixtureBase {
        [Test]
        public void CollectionExistsTest() {
            const string collectionName = "testCollectionExists";

            Database.CollectionExists(collectionName).Should().Be.False();

            Database[collectionName].Insert(new BsonDocument());
            Database.CollectionExists(collectionName).Should().Be.True();
        }

        public void CreateCollectionTest() {
            const string collectionName = "testCreateCollection";

            Database.CollectionExists(collectionName).Should().Be.False();

            Database.CreateCollection(collectionName).Ok.Should().Be.True();
            Database.CollectionExists(collectionName).Should().Be.True();
        }

        public void DropCollectionTest() {
            const string collectionName = "testDropCollection";

            Database.CollectionExists(collectionName).Should().Be.False();

            Database.CreateCollection(collectionName).Ok.Should().Be.True();
            Database.CollectionExists(collectionName).Should().Be.True();

            Database.DropCollection(collectionName).Ok.Should().Be.True();
            Database.CollectionExists(collectionName).Should().Be.False();
        }

        [Test]
        public void EvalNoArgsTest() {
            const string code = "function() { return 1; }";

            var result = Database.Eval(code);
            result.ToInt32().Should().Be(1);
        }

        [Test]
        public void EvalWithArgsTest() {
            const string code = "function(x, y) { return (x / y); }";

            var result = Database.Eval(code, new object[] { 6, 2 });
            result.ToInt32().Should().Be(3);

            result = Database.Eval(EvalFlags.None, code, new object[] { 6, 2 });
            result.ToInt32().Should().Be(3);
        }

        [Test(Description = "NHibernate의 Example 로 검색하는 기능과 같다.")]
        public void FetchDBRefTest() {
            const string collectionName = "testDBRef";

            var collection = Database.GetCollection(collectionName);
            var document = new BsonDocument
                           {
                               { "_id", ObjectId.GenerateNewId() },
                               { "P", "x" }
                           };
            collection.Insert(document);

            var dbRef = new MongoDBRef(collectionName, document["_id"].AsObjectId);
            var fetched = Database.FetchDBRef(dbRef);
            Assert.AreEqual(document, fetched);
            Assert.AreEqual(document.ToJson(), fetched.ToJson());

            var dbRefWithDatabaseName = new MongoDBRef(Database.Name, collectionName, document["_id"].AsObjectId);
            fetched = Server.FetchDBRef(dbRefWithDatabaseName);
            Assert.AreEqual(document, fetched);
            Assert.AreEqual(document.ToJson(), fetched.ToJson());
            Assert.Throws<ArgumentException>(() => Server.FetchDBRef(dbRef));
        }

        [Test]
        public void GetCollectionTest() {
            const string collectionName = "testgetcollection";

            var collection = Database.GetCollection(typeof(BsonDocument), collectionName);

            collection.Should().Not.Be.Null();
            collection.Database.Should().Be(Database);
            collection.FullName.Should().Be(Database.Name + "." + collectionName);
            collection.Name.Should().Be(collectionName);
            collection.Settings.SafeMode.Should().Be(Database.Settings.SafeMode);
        }

        [Test]
        public void GetGenericCollectionTest() {
            const string collectionName = "testgetgenericcollection";

            var collection = Database.GetCollection<BsonDocument>(collectionName);

            collection.Should().Not.Be.Null();
            collection.Database.Should().Be(Database);
            collection.FullName.Should().Be(Database.Name + "." + collectionName);
            collection.Name.Should().Be(collectionName);
            collection.Settings.SafeMode.Should().Be(Database.Settings.SafeMode);
        }

        [Test]
        public void RenameCollectionTest() {
            const string collectionName1 = "renamecollection1";
            const string collectionName2 = "renamecollection2";

            Database.CollectionExists(collectionName1).Should().Be.False();
            Database.CollectionExists(collectionName2).Should().Be.False();

            Database[collectionName1].Insert(new BsonDocument());
            Database.CollectionExists(collectionName1).Should().Be.True();
            Database.CollectionExists(collectionName2).Should().Be.False();

            Database.RenameCollection(collectionName1, collectionName2);
            Database.CollectionExists(collectionName1).Should().Be.False();
            Database.CollectionExists(collectionName2).Should().Be.True();
        }

        [Test]
        [Ignore("아직 개발 중인 내용임.")]
        public void RenameCollectionDropTargetTest() {
            const string collectionName1 = "renamecollection1";
            const string collectionName2 = "renamecollection2";

            Database.CollectionExists(collectionName1).Should().Be.False();
            Database.CollectionExists(collectionName2).Should().Be.False();

            Database[collectionName1].Insert(new BsonDocument());
            Database[collectionName2].Insert(new BsonDocument());

            Database.CollectionExists(collectionName1).Should().Be.True();
            Database.CollectionExists(collectionName2).Should().Be.True();

            Assert.Throws<MongoCommandException>(() => Database.RenameCollection(collectionName1, collectionName2));

            // Database.RenameCollection(collectionName1, collectionName2, true);
            Database.CollectionExists(collectionName1).Should().Be.False();
            Database.CollectionExists(collectionName2).Should().Be.True();
        }

        [Test]
        public void UserMethodsTest() {
            var collection = Database["system.users"];
            collection.RemoveAll();

            Database.AddUser(new MongoCredentials("username", "password"), true);
            collection.Count().Should().Be(1);

            var user = Database.FindUser("username");
            user.Username.Should().Be("username");
            user.PasswordHash.Should().Be(MongoUtils.Hash("username:mongo:password"));
            user.IsReadOnly.Should().Be.True();

            var users = Database.FindAllUsers();
            users.Length.Should().Be(1);
            users[0].Username.Should().Be("username");
            users[0].PasswordHash.Should().Be(MongoUtils.Hash("username:mongo:password"));
            users[0].IsReadOnly.Should().Be.True();

            Database.RemoveUser(user);
            collection.Count().Should().Be(0);
        }
    }
}