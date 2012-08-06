using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace NSoft.NFramework.Caching.SharedCache.Benchmark {
    [TestFixture]
    public class BenchmarkFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const int DataSize = 0x1000;

        public IndexusProviderBase Cache {
            get { return IndexusDistributionCache.SharedCache; }
        }

        [TestCase(10)]
        [TestCase(50)]
        public void ExpirationTest(int taskCount) {
            TaskCacheItem item;

            for(var i = 0; i < taskCount; i++) {
                item = new TaskCacheItem()
                       {
                           IsDone = false,
                           Summary = "Task " + i + " to cached.",
                           Data = ArrayTool.GetRandomBytes(DataSize)
                       };

                Cache.Add(item.Id.ToString(), item, DateTime.Now.AddSeconds(1));
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            item = new TaskCacheItem()
                   {
                       IsDone = false,
                       Summary = "Task xxxx to cached.",
                       Data = ArrayTool.GetRandomBytes(DataSize)
                   };

            Cache.Add(item.Id.ToString(), item, DateTime.Now.AddSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            item = new TaskCacheItem()
                   {
                       IsDone = false,
                       Summary = "Task xxxx to cached.",
                       Data = ArrayTool.GetRandomBytes(DataSize)
                   };

            Cache.Add(item.Id.ToString(), item, DateTime.Now.AddSeconds(1));

            var stats = IndexusDistributionCache.SharedCache.GetStats();
        }

        [TestCase(100)]
        [TestCase(200)]
        public void MassiveInsert(int taskCount) {
            for(var i = 0; i < taskCount; i++) {
                var item = new TaskCacheItem
                           {
                               IsDone = false,
                               Summary = "Task " + i + " to cached.",
                               Data = ArrayTool.GetRandomBytes(DataSize)
                           };

                Cache.Add(item.Id.ToString(), item, DateTime.Now.AddSeconds(5));
            }

            var stats = IndexusDistributionCache.SharedCache.GetStats();
        }

        [TestCase(100)]
        [TestCase(200)]
        public void MassiveInsertAsync(int taskCount) {
            var tasks = new List<Task>();

            for(var i = 0; i < taskCount; i++) {
                var item = new TaskCacheItem
                           {
                               IsDone = false,
                               Summary = "Task " + i + " to cached.",
                               Data = ArrayTool.GetRandomBytes(DataSize)
                           };

                var task = Task.Factory.StartNew(() => Cache.Add(item.Id.ToString(), item, DateTime.Now.AddSeconds(5)));
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            var stats = IndexusDistributionCache.SharedCache.GetStats();
        }

        [Test]
        public void Statistics() {
            Console.WriteLine("Item Count =" + Cache.ItemCount());
            Console.WriteLine("ServiceTotalSize=" + Cache.TotalSize());

            foreach(var pair in Cache.ItemUsageList())
                Console.WriteLine("{0}-{1}", pair.Key, pair.Value);
        }
    }
}