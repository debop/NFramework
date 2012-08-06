using System;
using System.Threading.Tasks;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Algorithms {
    /// <summary>
    /// False Sharing 이란 다중 코어에서 캐시 시스템을 운용할 때, 인접한 메모리에 대한 변화를 각각의 Core가 유지해야 하는 부담을 나타낸다.
    /// 즉 동시성이 아니더라도, 인접한 메모리에 대한 처리는 다중 코어에서는 캐시로 처리하고, 이는 캐시 정보의 동기화에 많은 시간을 소비하게 됨을 의미한다.
    /// 그러므로, 메모리가 인접한 영역에 대한 처리를 직접적으로 병렬로 처리하는 것은 비효율적이다. Core별로 local 변수로 따로 처리한 후, 최종적으로 갱신하는 것이 가장 효과가 좋다.
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class FalseSharingTestCase : ParallelismFixtureBase {
        private const int IterationCount = 99999;

        [Test]
        public void FalseSharing_Serial() {
            using(new OperationTimer("False sharing by Serial")) {
                int cores = Environment.ProcessorCount;
                var data = new int[cores];

                for(int i = 0; i < cores; i++)
                    for(var j = 0; j < IterationCount; j++)
                        data[i] = data[i] + 3;
            }
        }

        [Test]
        public void FalseSharing_ParallelFor() {
            using(new OperationTimer("False sharing by ParallelFor")) {
                var cores = Environment.ProcessorCount;
                var data = new int[cores];

                Parallel.For(0, cores,
                             n => {
                                 for(var j = 0; j < IterationCount; j++)
                                     data[n] = data[n] + 3;
                             });
            }
        }

        [Test]
        public void FalseSharing_Fix_By_LocalValue() {
            using(new OperationTimer("False sharing by ParallelFor and LocalValue")) {
                var cores = Environment.ProcessorCount;
                var data = new int[cores];

                Parallel.For(0, cores,
                             n => {
                                 // 스레드별 로컬 변수로 전체 작업을 수행하고, 마지막에 한번에 반영한다.
                                 var localCount = data[n];

                                 for(var j = 0; j < IterationCount; j++)
                                     localCount = localCount + 3;

                                 data[n] = localCount;
                             });
            }
        }
    }
}