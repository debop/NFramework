using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures.Async {
    /// <summary>
    /// 캐시 값을 구하는 Task를 정의하여, 원할 때에 값이 없다면, Task를 수행하여 결과를 제공해주는 비동기 캐시 !!! 
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class AsyncCacheFixture : ParallelismFixtureBase {
        private const int TestCount = 100;

        private readonly AsyncCache<int, double> _cache =
            new AsyncCache<int, double>(n =>
                                        Task<double>.Factory
                                            .StartNew(() => {
                                                          Thread.Sleep(1);
                                                          return Math.Pow(n, 2);

                                                          // 여기에 WebClient Download를 하게 되면, 좋은 예가 될 것이다.
                                                      },
                                                      TaskCreationOptions.None));

        [Test]
        public void AsyncCache_Auto_Created() {
            _cache.Clear();

            for(var i = 0; i < TestCount; i++)
                Assert.AreEqual(Math.Pow(i, 2), _cache[i]);
        }

        [Test]
        public void AsyncCache_Lazy_Created() {
            _cache.Clear();

            for(var i = 0; i < TestCount; i++)
                Assert.AreEqual(Math.Pow(i, 2), _cache.GetValue(i).Result);
        }

        [Test]
        public void AsyncCache_Set_Directly_Value() {
            _cache.Clear();

            for(var i = 0; i < TestCount; i++) {
                var key = i;
                _cache.SetValue(key, Math.Pow(key, 3));
            }
            for(var i = 0; i < TestCount; i++) {
                var key = i;
                Assert.AreEqual(Math.Pow(key, 3), _cache[key], "key=" + key);
            }
        }

        [Test]
        public void AsyncCache_Set_Directly_Task() {
            _cache.Clear();

            for(var i = 0; i < TestCount; i++) {
                var key = i;
                _cache.SetValue(key, Task.Factory.StartNew(() => Math.Pow(key, 3)));
            }

            Thread.Sleep(500);

            for(var i = 0; i < TestCount; i++) {
                var key = i;
                Assert.AreEqual(Math.Pow(key, 3), _cache[key], "key=" + key);
            }
        }
    }
}