using System;
using System.Threading;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Web.HttpApplications {
    [TestFixture]
    public class WindsorAsyncHttpApplicationFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [TestCase(10)]
        [TestCase(30)]
        public void Can_WindsorLazyHttpApplication_Request_Handle_In_MultiThread(int threadCount) {
            TestTool.RunTasks(threadCount, () => Can_WindsorLazyHttpApplication_Request_Handle(30));
        }

        [TestCase(10)]
        [TestCase(30)]
        public void Can_WindsorLazyHttpApplication_Request_Handle(int requestCount) {
            var app = new WindsorAsyncHttpApplication();
            app.Should().Not.Be.Null();

            app.Application_Start(app, EventArgs.Empty);
            app.Init();

            for(var i = 0; i < requestCount; i++) {
                // app.OnBeginRequest(this, EventArgs.Empty);

                app.Container.Should("app.Container").Not.Be.Null();
                IoC.IsInitialized.Should("IoC.IsInitialized").Be.True();

                Thread.Sleep(1);

                if(IsDebugEnabled)
                    log.Debug("요청 사항 처리 중...");

                Thread.Sleep(1);

                // app.OnEndRequest(this, EventArgs.Empty);
            }
            app.Dispose();

            app = new WindsorAsyncHttpApplication();
            app.Should().Not.Be.Null();

            app.Init();

            for(var i = 0; i < requestCount; i++) {
                // app.OnBeginRequest(this, EventArgs.Empty);

                app.Container.Should("app.Container").Not.Be.Null();
                IoC.IsInitialized.Should("IoC.IsInitialized").Be.True();

                Thread.Sleep(1);

                if(IsDebugEnabled)
                    log.Debug("요청 사항 처리 중...");

                Thread.Sleep(1);

                // app.OnEndRequest(this, EventArgs.Empty);
            }
            app.Dispose();
        }
    }
}