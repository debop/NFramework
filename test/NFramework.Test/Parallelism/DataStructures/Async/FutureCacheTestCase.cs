using System;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures.Async {
    /// <summary>
    /// 캐시 값을 구하는 Task를 정의하여, 원할 때에 값이 없다면, Task를 수행하여 결과를 제공해주는 비동기 캐시 !!! 
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class FutureCacheTestCase : ParallelismFixtureBase {
        private const int TestCount = 100;

        [Test]
        public void AsyncCache_Auto_Created() {
            var cache = new FutureCache<int, double>(n => Math.Pow(n, 2));

            for(int i = 0; i < TestCount; i++)
                Assert.AreEqual(Math.Pow(i, 2), cache[i]);
        }

        [Test]
        public void AsyncCache_Lazy_Created() {
            var cache = new FutureCache<int, double>(n => Math.Pow(n, 2));

            for(int i = 0; i < TestCount; i++)
                Assert.AreEqual(Math.Pow(i, 2), cache.GetValue(i));
        }

        [Test]
        public void AsyncCache_Set_Directly_Value() {
            var cache = new FutureCache<int, double>(n => Math.Pow(n, 2));

            for(int i = 0; i < TestCount; i++) {
                var key = i;
                cache.SetValue(key, Math.Pow(key, 3));
            }

            Thread.Sleep(5 * TestCount);

            for(int i = 0; i < TestCount; i++) {
                var key = i;
                Assert.AreEqual(Math.Pow(key, 3), cache[key], "key=" + key);
            }
        }

        [Test]
        public void AsyncCache_Set_Directly_Task() {
            var cache = new FutureCache<int, double>(n => Math.Pow(n, 2));

            for(int i = 0; i < TestCount; i++) {
                var key = i;
                cache.SetValue(key, Task.Factory.StartNew(() => Math.Pow(key, 3)));
            }

            Thread.Sleep(1 * TestCount);

            for(int i = 0; i < TestCount; i++) {
                var key = i;
                Assert.AreEqual(Math.Pow(key, 3), cache[key], "key=" + key);
            }
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(4,
                              AsyncCache_Auto_Created,
                              AsyncCache_Lazy_Created,
                              AsyncCache_Set_Directly_Value,
                              AsyncCache_Set_Directly_Task);
        }
    }
}