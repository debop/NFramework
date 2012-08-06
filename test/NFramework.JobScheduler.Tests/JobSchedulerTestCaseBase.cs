using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.JobScheduler {
    public abstract class JobSchedulerTestCaseBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            DoTestFixtureSetUp();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown() {
            DoTestFixtureTearDown();
        }

        protected virtual void DoTestFixtureSetUp() {
            if(log.IsInfoEnabled)
                log.Info(@"[{0}] Test를 시작합니다.", GetType().FullName);

            if(IoC.IsNotInitialized)
                IoC.Initialize();
        }

        protected virtual void DoTestFixtureTearDown() {
            if(log.IsInfoEnabled)
                log.Info(@"[{0}] Test가 종료되었습니다!!!", GetType().FullName);
        }
    }
}