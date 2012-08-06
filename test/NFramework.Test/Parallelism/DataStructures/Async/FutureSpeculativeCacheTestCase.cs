using System;
using System.Linq;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures.Async {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class FutureSpeculativeCacheTestCase : ParallelismFixtureBase {
#if !SILVERLIGHT
        public const int AdditionalCount = 5;

        // 숫자의 제곱을 구하는데, 계산된 숫자로부터 +5까지 Background에서 자동으로 계산하도록 합니다.
        //

        private static FutureSpeculativeCache<int, double> GetFutureSpeculativeCache() {
            return
                new FutureSpeculativeCache<int, double>(
                    key1 => {
                        if(IsDebugEnabled)
                            log.Debug("Create Future Value. key=[{0}]", key1);

                        return Math.Pow(key1, 2);
                    },
                    key2 => Enumerable.Range(key2 + 1, AdditionalCount));
        }

        [Test]
        public void Can_Speculative_Cache_In_Background() {
            var cache = GetFutureSpeculativeCache();

            Thread.Sleep(1);

            Assert.AreEqual(1, cache.GetValue(1));

            while(cache.Count < AdditionalCount + 1) {
                Thread.Sleep(1);
            }

            // 1 이외의 키는 지정하지 않았다. 위의 key 에 따른 background Key에 해당하는 2~6 까지의 키와 같이 이미 계산되고 있다.
            if(IsDebugEnabled)
                log.Debug("Cache Count=[{0}]", cache.Count);

            foreach(var k in Enumerable.Range(1, AdditionalCount))
                Assert.IsTrue(cache.ContainsKey(k), "key=[{0}] is not exists", k);
        }

        [Test]
        public void Can_Speculative_Cache_Set_Value() {
            var cache = GetFutureSpeculativeCache();

            cache.SetValue(1, 1);

            while(cache.Count < AdditionalCount + 1) {
                Thread.Sleep(1);
            }

            // 1 이외의 키는 지정하지 않았다. 위의 key 에 따른 background Key에 해당하는 2~6 까지의 키와 같이 이미 계산되고 있다.
            Console.WriteLine("Cache Count=" + cache.Count);

            foreach(var k in Enumerable.Range(1, AdditionalCount))
                Assert.IsTrue(cache.ContainsKey(k), "key={0} is not exists", k);
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(4,
                              Can_Speculative_Cache_In_Background,
                              Can_Speculative_Cache_Set_Value);
        }
#endif
    }
}