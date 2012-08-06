using System;
using System.Collections;
using System.Web.UI;
using Castle.Core;
using Castle.MicroKernel.Registration;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Web.PageStatePersisters;
using NUnit.Framework;

namespace NSoft.NFramework.Web.PageAdapters {
    [TestFixture]
    public class IoCPageAdapterFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// ResolvePageStatePersister() 메소드가 제대로 작동하는지, 확인합니다. 
        /// </summary>
        [Test]
        public void Resolve_CachePageStatePersister() {
            if(IoC.IsInitialized)
                IoC.Reset();

            IoC.InitializeWithEmptyContainer();
            IoC.Container.Register(Component
                                       .For<IServerPageStatePersister>()
                                       .ImplementedBy(typeof(SysCachePageStatePersister))
                                       .LifeStyle.Is(LifestyleType.Transient));

            var page = new TestPage();
            var persister = ResolvePageStatePersister(page);

            Assert.IsNotNull(persister);
            Assert.IsInstanceOf<SysCachePageStatePersister>(persister);
        }

        [Test]
        public void Resolve_CompressableSessionPageStatePersister() {
            if(IoC.IsInitialized)
                IoC.Reset();

            IoC.InitializeWithEmptyContainer();
            IoC.Container.Register(Component
                                       .For<IServerPageStatePersister>()
                                       .ImplementedBy<CompressableSessionPageStatePersister>()
                                       .LifeStyle.Is(LifestyleType.Transient));

            var persister = ResolvePageStatePersister(new TestPage());
            Assert.IsNotNull(persister);
            Assert.IsTrue(persister is IServerPageStatePersister);
        }

        //[Test]
        //public void MockTest()
        //{
        //    var mock = new Mock<IServerPageStatePersister>();

        //    mock.Setup(p => p.Save());
        //    mock.Setup(p => p.CompressThreshold).Returns(() => 1024);
        //    //mock.Setup(p => p.StateKeyName).Returns(() => RealPageStatePersisterBase.StateKey);

        //    IServerPageStatePersister persister = mock.Object;
        //    Assert.AreEqual(1024, persister.CompressThreshold);
        //    //Assert.AreEqual(RealPageStatePersisterBase.StateKey, persister.StateKeyName);
        //}

        internal static PageStatePersister ResolvePageStatePersister(Page page) {
            page.ShouldNotBeNull("page");

            if(IsDebugEnabled)
                log.Debug(
                    "Castle.Windsor의 Container로부터 PageStatePersister를 Resolve합니다. PageStatePersister의 Lifestyle은 항상 transient해야 합니다.");

            IServerPageStatePersister persister;
            try {
                // 컨테이너에서 PageStatePersister를 Resolve합니다.
                //
                var arguments = new Hashtable { { "page", page } };
                persister = IoC.Resolve<IServerPageStatePersister>(arguments);

                persister.ShouldNotBeNull("persister");

                if(IsDebugEnabled)
                    log.Debug("Castle.Windsor Container로부터 PageStatePersister를 Resolve 했습니다. PageStatePersister=" +
                              persister.GetType().FullName);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("PageStatePersister를 Resolve하는데 실패했습니다. 환경설정이 제대로 되었는지 확인하시기 바랍니다.", ex);

                if(IsDebugEnabled)
                    log.Debug("기본 PageStatePersister로 CompressHiddenFieldPersister를 제공합니다.");

                persister = new CompressHiddenFieldPersister(page);
            }

            return (PageStatePersister)persister;
        }

        internal class TestPage : System.Web.UI.Page {
            public TestPage() {}
        }
    }
}