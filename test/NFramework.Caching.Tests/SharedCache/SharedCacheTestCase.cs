using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace NSoft.NFramework.Caching.SharedCache {
    [TestFixture]
    public class SharedCacheTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly IList<string> taskIdStrs = new List<string>();

        public IndexusProviderBase Cache {
            get { return IndexusDistributionCache.SharedCache; }
        }

        [TestFixtureTearDown]
        public void ClearCache() {
            Cache.Clear();
        }

        [Test]
        public void Can_Clear() {
            if(IsDebugEnabled)
                log.Debug("Clear All Cache Items");

            Cache.Clear();

            // NOTE: 캐시된 요소의 수는 Cache.Count가 아니라 Cache.GetAllKeys().Count 이다.
            Assert.AreEqual(0, Cache.GetAllKeys().Count);
        }

        [Test]
        public void Can_Add_And_Load_Task() {
            Cache.Clear();

            var task = new TaskCacheItem
                       {
                           IsDone = false,
                           Summary = @"Task to cached.",
                           Data = ArrayTool.GetRandomBytes(0x2000)
                       };

            var taskId = task.Id.ToString();

            Cache.Add(taskId, task);
            taskIdStrs.Add(taskId);

            var tasks = Cache.GetAllKeys().Select(key => Cache.Get<TaskCacheItem>(key)).ToList();

            Assert.IsTrue(tasks.Any(t => t.IsDone == false));

            var retrieved = Cache.Get<TaskCacheItem>(taskId);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(taskId, retrieved.Id.ToString());

            Cache.Clear();

            if(Cache.GetAllKeys().Contains(taskId)) {
                retrieved = Cache.Get(taskId) as TaskCacheItem;
                Assert.IsNull(retrieved);
            }
        }

        [Test]
        public void Can_Add_Multiple_And_Clear() {
            Cache.Clear();
            taskIdStrs.Clear();

            const int TaskCount = 1000;

            for(int i = 0; i < TaskCount; i++) {
                var task = new TaskCacheItem()
                           {
                               IsDone = false,
                               Summary = "Task " + i + " to cached.",
                               Data = ArrayTool.GetRandomBytes(0x2000)
                           };

                Cache.Add(task.Id.ToString(), task);
                taskIdStrs.Add(task.Id.ToString());
            }

            Assert.AreEqual(TaskCount, Cache.GetAllKeys().Count);

            foreach(var taskId in taskIdStrs)
                Assert.IsNotNull(Cache.Get(taskId), "캐시에 해당 키의 정보가 없습니다. taskId=" + taskId);
        }
    }
}