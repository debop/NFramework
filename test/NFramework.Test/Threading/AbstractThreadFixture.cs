using System.Diagnostics;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Threading {
    [TestFixture]
    public class AbstractThreadFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private const int MillisecondsTimeout = 300;

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(15,
                              () => {
                                  SleepTest();
                                  SleepPerfCounterTest();
                              });
        }

        [Test]
        public void SleepTest() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try {
                ThreadTool.Sleep(MillisecondsTimeout);
            }
            catch(ThreadInterruptedException ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("쓰레드가 중단되었습니다.", ex);
            }

            stopwatch.Stop();
            var ts = stopwatch.Elapsed;

            if(IsDebugEnabled)
                log.Debug("AbstractThread.Sleep({0})  ==> {1} msec by DateTime", (object)MillisecondsTimeout,
                          (object)ts.TotalMilliseconds);
        }

        [Test]
        public void SleepPerfCounterTest() {
            using(new OperationTimer("AbstractThread")) {
                try {
                    ThreadTool.Sleep(MillisecondsTimeout);
                }
                catch(ThreadInterruptedException tie) {
                    if(log.IsWarnEnabled)
                        log.WarnException("Thread.Sleep 중에 Interrupted 되었습니다.", tie);
                }
            }
        }
    }
}