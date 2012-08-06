using System;
using NUnit.Framework;

namespace NSoft.NFramework.Caching {
    [TestFixture]
    public class ConcurrentCacheRepositoryFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private ConcurrentCacheRepository _repository;

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();
            _repository = new ConcurrentCacheRepository();
        }

        [Test]
        public void Can_Clear() {
            _repository.Clear();
        }

        [Test]
        public void Can_Add_And_Load_Task() {
            var task = new TaskCacheItem()
                       {
                           IsDone = false,
                           Summary = "Task to cached."
                       };
            _repository.Set(Guid.NewGuid().ToString(), task);
        }
    }
}