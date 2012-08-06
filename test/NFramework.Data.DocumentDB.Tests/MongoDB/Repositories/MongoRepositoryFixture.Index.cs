using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Repositories {
    [TestFixture]
    public class MongoRepositoryFixture_Index : MongoFixtureBase {
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
        public void PingTest() {
            Repository.Ping();
        }

        [Test]
        public void CreateIndex_DropIndex_Test() {
            const string CollectionName = "create-index-drop-index-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            Repository.Insert(new BsonDocument());
            Repository.DropAllIndexes();

            var indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(1);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);

            Repository.CreateIndex("x");
            indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1");

            Repository.DropIndexByName("x_1");
            indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(1);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);

            Repository.CreateIndex(IndexKeys.Ascending("x").Descending("y"), IndexOptions.SetUnique(true));

            indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1_y_-1");
            indexes[1].RawDocument["unique"].ToBoolean().Should().Be.True();

            Repository.DropAllIndexes();
            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void DropAllIndexesTest() {
            const string CollectionName = "drop-all-indexes-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            Repository.Insert(new BsonDocument());
            Repository.DropAllIndexes();

            var indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(1);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);

            Repository.CreateIndex("x");
            indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1");


            Repository.DropAllIndexes();

            indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(1);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void GetIndexesTest() {
            const string CollectionName = "get-indexes-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            Repository.Insert(new BsonDocument());
            Repository.DropAllIndexes();

            var indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(1);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);

            Repository.DropCollection(CollectionName);
        }

        [Test]
        public void EnsureIndexTest() {
            const string CollectionName = "ensure-inde-test";

            Repository.DropCollection(CollectionName);
            Repository.CreateCollection(CollectionName);

            Repository.Insert(new BsonDocument());
            Repository.DropAllIndexes();

            var indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(1);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);

            Repository.EnsureIndex("x");
            indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1");

            Repository.EnsureIndex("x");
            indexes = Repository.GetIndexes().ToArray();
            indexes.Length.Should().Be(2);
            indexes[0].RawDocument["name"].AsString.Should().Be(MongoTool.IdIndexName);
            indexes[1].RawDocument["name"].AsString.Should().Be("x_1");

            Repository.DropCollection(CollectionName);
        }
    }
}