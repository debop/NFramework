using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.IO;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Parallelism.Samples {
    [TestFixture]
    public class ParallelTestCase : ParallelismFixtureBase {
        [Test]
        public void Parallel_Directory_Size() {
            // var files = Directory.GetFiles(FileTool.GetTempPath(), "*", SearchOption.AllDirectories);

            using(new OperationTimer("Parallel.ForEach")) {
                var files = Directory.GetFiles(FileTool.GetTempPath(), "*", SearchOption.AllDirectories);

                long size = 0;

                var result =
                    Parallel
                        .ForEach<string, long>(files,
                                               () => 0,
                                               (file, loopState, index, localTotalSize) => localTotalSize + new FileInfo(file).Length,
                                               (localTotalSize) => Interlocked.Add(ref size, localTotalSize));

                if(IsDebugEnabled)
                    log.Debug("Parallel.ForEach... " + size.ToString("#,##0") + " bytes");
                Assert.IsTrue(result.IsCompleted);
            }

            using(new OperationTimer("AsParallel")) {
                var files = Directory.GetFiles(FileTool.GetTempPath(), "*", SearchOption.AllDirectories);

                var size = files
                    .AsParallel()
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .Sum(file => new FileInfo(file).Length);

                if(IsDebugEnabled)
                    log.Debug("AsParallel... " + size.ToString("#,##0") + " bytes");
            }

            using(new OperationTimer("All Directory Size")) {
                long size = FileTool.GetTempPath().GetDirectorySize(true);

                if(IsDebugEnabled)
                    log.Debug("Serial calculated... " + size.ToString("#,##0") + " bytes");
            }
        }

        [Test]
        public void AsParallel_Test() {
            using(new OperationTimer("AsParallel")) {
                var powers =
                    Enumerable.Range(0, 1000)
                        .AsParallel()
                        .Select(i => {
                                    WriterParllel(i.ToString(), "AsParallel");
                                    Thread.Sleep(1);
                                    return i * i;
                                })
                        .ToList();

                powers.Any(i => i == 100).Should().Be.True();
            }
        }

        [Test]
        public void Parallel_For_Test() {
            using(new OperationTimer("Parallel.For")) {
                Parallel.For(0,
                             1000,
                             i => {
                                 WriterParllel(i.ToString(), "Parallel.For");
                                 Thread.Sleep(1);
                             });
            }
        }

        private static void WriterParllel(string item, string name) {
            if(IsDebugEnabled)
                log.Debug("item={0}, name={1}, ManagedThreadId={2}, GetHashCode={3}", item, name, Thread.CurrentThread.ManagedThreadId,
                          Thread.CurrentThread.GetHashCode());

            Thread.Sleep(1);
        }
    }
}