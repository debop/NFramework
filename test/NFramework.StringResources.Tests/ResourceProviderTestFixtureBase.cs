using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources {
    public abstract class ResourceProviderTestFixtureBase {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [TestFixtureSetUp]
        public void ClassSetUp() {
            OnClassSetUp();
        }

        protected virtual void OnClassSetUp() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();

            if(UnitOfWork.IsStarted == false)
                UnitOfWork.Start();
        }
    }
}