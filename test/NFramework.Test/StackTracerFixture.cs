using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework {
    [TestFixture]
    public class StackTracerFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string LINE = @"-----------------------";

        [Test]
        public void Test() {
            var stackTrace = StackTracer.GetCurrentStackTraceInfo();

            if(IsDebugEnabled) {
                log.Debug("Current StackTrace Inforation");
                log.Debug(LINE);
                log.Debug(stackTrace);
                log.Debug(LINE);
            }
        }

        [Test]
        public void TestOfCurrentStacks() {
            log.Debug("Current StackTrace Inforation");
            log.Debug(LINE);

            foreach(string methodCall in StackTracer.GetCurrentStackTraces()) {
                if(IsDebugEnabled)
                    log.Debug(methodCall);
            }
        }

        [Test]
        public void MultiThreadMixingTest() {
            TestTool.RunTasks(2, () => {
                                     using(new OperationTimer("MultiThreadMixingTest")) {
                                         Test();
                                         TestOfCurrentStacks();
                                     }
                                 });
        }
    }
}