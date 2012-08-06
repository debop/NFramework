using System;
using System.IO;
using System.Threading;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NSoft.NFramework.Web.HttpApplications;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.HttpModules {
    [TestFixture]
    public class UnitOfWorkHttpApplicationFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            IoC.Reset();
        }

        [Test]
        public void FirstRequestForUnitOfWorkApplication_WillInitializeContainer() {
            using(var app = new UnitOfWorkHttpApplication()) {
                Assert.IsNotNull(app);
                //app.OnBeginRequest(this, EventArgs.Empty);

                Assert.IsTrue(IoC.IsInitialized);
                Assert.IsNotNull(IoC.Container);
            }
        }

        [Test]
        public void ReadingFileWillNotLockIt() {
            using(var app = new UnitOfWorkHttpApplication()) {
                Assert.IsNotNull(app);
                //app.OnBeginRequest(this, EventArgs.Empty);

                if(File.Exists("Windsor.config"))
                    File.AppendAllText("Windsor.config", Environment.NewLine);
            }
        }

        [Test]
        public void MultiThreadTesting() {
            TestTool.RunTasks(3,
                              () => {
                                  for(var n = 0; n < 3; n++) {
                                      var app = new UnitOfWorkHttpApplication();

                                      Assert.IsNotNull(app);

                                      for(int i = 0; i < 5; i++) {
                                          //app.OnBeginRequest(this, EventArgs.Empty);

                                          Assert.IsTrue(IoC.IsInitialized);
                                          Assert.IsNotNull(IoC.Container);

                                          // UnitOfWorkHttpApplication에 제대로 된 요청이 있어야만 UnitOfWork가 시작됩니다. (aspx 나 ashx, ascx 등 script)
                                          Assert.IsFalse(UnitOfWork.IsStarted);

                                          log.Debug("사용자 요청을 처리중입니다.........");

                                          Thread.Sleep(1);

                                          //app.OnEndRequest(this, EventArgs.Empty);

                                          log.Debug("사용자 요청을 처리를 완료했습니다.");
                                      }

                                      var app2 = new WindsorHttpApplication();

                                      Assert.IsNotNull(app2);

                                      for(int i = 0; i < 5; i++) {
                                          //app2.OnBeginRequest(this, EventArgs.Empty);

                                          Assert.IsTrue(IoC.IsInitialized);
                                          Assert.IsNotNull(IoC.Container);

                                          log.Debug("사용자 요청을 처리중입니다.........");

                                          Thread.Sleep(1);

                                          //app2.OnEndRequest(this, EventArgs.Empty);

                                          log.Debug("사용자 요청을 처리를 완료했습니다.");
                                      }
                                  }
                              });
        }
    }
}