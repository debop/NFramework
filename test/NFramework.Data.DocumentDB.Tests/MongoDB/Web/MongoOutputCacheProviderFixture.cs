using System;
using System.Threading;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Web {
    [TestFixture]
    public class MongoOutputCacheProviderFixture : MongoFixtureBase {
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

        public static readonly string CacheKey = Guid.NewGuid().ToString();

        protected override void OnTestFixtureTearDown() {
            base.OnTestFixtureTearDown();

            Repository.DropAllCollection();
        }

        [Test]
        public void GetTest() {
            Repository.Save<MongoAspOutputCacheEntry>(new MongoAspOutputCacheEntry(CacheKey, new object())).Ok.Should().Be.True();
            var entry = Repository.FindOneByIdAs<MongoAspOutputCacheEntry>(CacheKey);
            entry.Should().Not.Be.Null();
            entry.Id.Should().Be(CacheKey);
        }

        [Test]
        public void AddTest() {
            Repository.Save<MongoAspOutputCacheEntry>(new MongoAspOutputCacheEntry(CacheKey, new object())).Ok.Should().Be.True();
        }

        [Test]
        public void SetTest() {
            Repository.Save<MongoAspOutputCacheEntry>(new MongoAspOutputCacheEntry(CacheKey, new object())).Ok.Should().Be.True();
        }

        [Test]
        public void RemoveTest() {
            Repository.Save<MongoAspOutputCacheEntry>(new MongoAspOutputCacheEntry(CacheKey, new object())).Ok.Should().Be.True();
            Repository.RemoveByIdAs<MongoAspOutputCacheEntry>(CacheKey).Ok.Should().Be.True();
        }
    }
}