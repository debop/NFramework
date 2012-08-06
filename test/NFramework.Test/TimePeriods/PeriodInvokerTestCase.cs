using System;
using System.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.TimePeriods {
    [TestFixture]
    public class PeriodInvokerTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private bool _actionRaised;

        [SetUp]
        public void InitVariables() {
            _actionRaised = false;
        }

        [Test]
        [TestCase("* * * * *")] // 검사할 때 마다 분단위로...
        [TestCase("0,30 * * * *")] // 매시, 매시 30분에 실행
        public void PeriodInvoker_By_PeriodFormat(string periodTimeFormat) {
            using(var periodInvoker = new PeriodInvoker(periodTimeFormat, PeriodAction, true)) {
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            Assert.IsTrue(_actionRaised);
        }

        [TestCase(100)]
        [TestCase(200)]
        public void PeriodInvoker_By_PeriodTimeSpan(int milliSeconds) {
            var periodTimeSpan = TimeSpan.FromMilliseconds(milliSeconds);

            using(var periodInvoker = new PeriodInvoker(string.Empty, periodTimeSpan, PeriodAction, true)) {
                Thread.Sleep(1000);
            }
            Assert.IsTrue(_actionRaised);
        }

        public void PeriodAction(object state) {
            if(IsDebugEnabled)
                log.Debug("반복 주기에 따라 호출된 메소드를 시작합니다...");

            _actionRaised = true;

            if(state is CancellationToken) {
                var token = (CancellationToken)state;

                while(token.IsCancellationRequested == false) {
                    for(int i = 0; i < 5; i++) {
                        if(token.IsCancellationRequested == false && token.WaitHandle != null)
                            token.WaitHandle.WaitOne(1);
                    }
                }
            }

            if(IsDebugEnabled)
                log.Debug("반복 주기에 따라 호출된 메소드를 완료했습니다!!!");
        }
    }
}