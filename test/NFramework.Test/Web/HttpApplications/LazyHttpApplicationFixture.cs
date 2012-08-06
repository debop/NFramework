using System;
using System.Threading;
using System.Web;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Web.HttpApplications {
    [TestFixture]
    public class AsyncHttpApplicationFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public class TestAsyncHttpApplication : AsyncHttpApplicationBase {
            protected override void ApplicationStartAfter(HttpContext context) {
                base.ApplicationStartAfter(context);

                if(IsDebugEnabled)
                    log.Debug("FutureTask At Application Start");
            }

            protected override void ApplicationInitAfter(HttpContext context) {
                base.ApplicationInitAfter(context);

                if(IsDebugEnabled)
                    log.Debug("FutureTask At Application Init");
            }
        }

        public const int RequestCount = 15;

        [Test]
        public void LazyHttpApplication_MultiThread() {
            TestTool.RunTasks(5, Can_Handle_Request_Response, Can_Handle_Request_Response, Can_Handle_Request_Response);
        }

        [Test]
        public void Can_Handle_Request_Response() {
            using(var app = new TestAsyncHttpApplication()) {
                app.Application_Start(app, EventArgs.Empty);

                Assert.IsNotNull(app);

                for(var i = 0; i < RequestCount; i++) {
                    // app.OnBeginRequest(this, EventArgs.Empty);

                    Thread.Sleep(1);

                    if(IsDebugEnabled)
                        log.Debug("요청 사항 처리 중... request=" + i);

                    // app.OnEndRequest(this, EventArgs.Empty);
                }
            }

            using(var app = new TestAsyncHttpApplication()) {
                Assert.IsNotNull(app);

                for(var i = 0; i < RequestCount; i++) {
                    // app.OnBeginRequest(this, EventArgs.Empty);

                    Thread.Sleep(1);

                    if(IsDebugEnabled)
                        log.Debug("요청 사항 처리 중... request=" + i);

                    // app.OnEndRequest(this, EventArgs.Empty);
                }
            }
        }
    }
}