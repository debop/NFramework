using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Collections.Concurrent {
    public enum CheckOrderingType {
        InOrder,
        Reversed,
        DontCare
    }

    public static class ConcurrentCollectionStressTestTool {
        public static void AddStress(this IProducerConsumerCollection<int> coll) {
            ParallelTestTool.Repeat(() => {
                                        var amount = -1;
                                        const int count = 10;
                                        const int threads = 5;

                                        ParallelTestTool.ParallelStressTest(coll,
                                                                            q => {
                                                                                var t = Interlocked.Increment(ref amount);
                                                                                for(var i = 0; i < count; i++)
                                                                                    q.TryAdd(t);
                                                                            },
                                                                            threads);

                                        Assert.AreEqual(threads * count, coll.Count);

                                        var values = new int[threads];
                                        int temp;

                                        while(coll.TryTake(out temp)) {
                                            values[temp]++;
                                        }

                                        for(var i = 0; i < threads; i++)
                                            Assert.AreEqual(count, values[i], "#" + i);
                                    });
        }

        public static void RemoveStress(this IProducerConsumerCollection<int> coll) {
            RemoveStress(coll, CheckOrderingType.DontCare);
        }

        public static void RemoveStress(this IProducerConsumerCollection<int> coll, CheckOrderingType order) {
            ParallelTestTool.Repeat(() => {
                                        const int count = 10;
                                        const int threads = 5;
                                        const int delta = 5;

                                        const int totalCount = (count + delta) * threads;

                                        for(var i = 0; i < totalCount; i++)
                                            coll.TryAdd(i);

                                        var state = true;

                                        ParallelTestTool.ParallelStressTest(coll,
                                                                            q => {
                                                                                int t;
                                                                                for(var i = 0; i < count; i++)
                                                                                    state &= q.TryTake(out t);
                                                                            },
                                                                            threads);

                                        Assert.IsTrue(state, "#1");
                                        Assert.AreEqual(delta * threads, coll.Count, "#2");

                                        var builder = new StringBuilder();
                                        int temp;
                                        while(coll.TryTake(out temp)) {
                                            builder.Append(temp);
                                        }

                                        var actual = builder.ToString();

                                        var range = Enumerable.Range(order == CheckOrderingType.Reversed ? 0 : count * threads,
                                                                     delta * threads);
                                        if(order == CheckOrderingType.Reversed)
                                            range = range.Reverse();

                                        var expected = range.Aggregate(string.Empty, (acc, v) => acc + v);

                                        if(order == CheckOrderingType.DontCare)
                                            CollectionAssert.AreEquivalent(expected, actual, "#3");
                                        else
                                            Assert.AreEqual(expected, actual, "#3");
                                    });
        }
    }
}