using System;
using System.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.Threading.ComputerBoundAsync {
    [TestFixture]
    public class TimerCase : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private static System.Threading.Timer _timer;
        private static ManualResetEventSlim _mre;

        [Test]
        public void PeriodicExecution() {
            log.Debug("메인 스레드: 타이머 시작...");

            _mre = new ManualResetEventSlim(false);

            try {
                _timer = new System.Threading.Timer(ComputeBoundOp, 5, 0, Timeout.Infinite);

                log.Debug("메인 스레드: 타이머 작업 중...");
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
            finally {
                _mre.Wait();
                if(_timer != null)
                    _timer.Dispose();
            }

            log.Debug("메인 스레드: 타이머 종료!!!");
        }

        private static void ComputeBoundOp(object state) {
            _mre.Reset();

            log.Debug("계산 작업 중... state=" + state);

            Thread.Sleep(1000);

            // 2초후에 다시 Timer 호출
            _timer.Change(2000, Timeout.Infinite);

            _mre.Set();
        }
    }
}