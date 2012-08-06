using System.Threading;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Web.HttpApplications {
    [TestFixture]
    public class WindsorHttpApplicationFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const int RequestCount = 15;

        [TestCase(10)]
        [TestCase(30)]
        public void MultiThread_Can_IoCHttpApplication_Initialized(int threadCount) {
            TestTool.RunTasks(threadCount, Can_IoCHttpApplication_Initialized);
        }

        [Test]
        public void Can_IoCHttpApplication_Initialized() {
            using(var app = new WindsorHttpApplication()) {
                Assert.IsNotNull(app);

                for(var i = 0; i < RequestCount; i++) {
                    //app.OnBeginRequest(this, EventArgs.Empty);

                    Assert.IsNotNull(app.Container);
                    Assert.IsTrue(IoC.IsInitialized);

                    Thread.Sleep(1);

                    if(IsDebugEnabled)
                        log.Debug("요청 사항 처리 중");

                    // app.OnEndRequest(this, EventArgs.Empty);
                }
            }

            using(var app = new WindsorHttpApplication()) {
                Assert.IsNotNull(app);

                for(var i = 0; i < RequestCount; i++) {
                    //app.OnBeginRequest(this, EventArgs.Empty);

                    Assert.IsNotNull(app.Container);
                    Assert.IsTrue(IoC.IsInitialized);

                    Thread.Sleep(1);

                    if(IsDebugEnabled)
                        log.Debug("요청 사항 처리 중");

                    //app.OnEndRequest(this, EventArgs.Empty);
                }
            }
        }
    }
}