using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.UnitOfWorks {
    [TestFixture]
    public class GlobalNonThreadSafeUnitOfWorkFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void Should_be_in_a_unit_of_work() {
            var current = new StubUnitOfWork();
            using(UnitOfWork.RegisterGlobalUnitOfWork(current)) {
                UnitOfWork.Start();
                Assert.IsTrue(UnitOfWork.IsStarted);
                Assert.AreEqual(current, UnitOfWork.Current);
            }
        }

        [Test]
        public void Should_be_to_get_the_current_unit_of_work() {
            using(UnitOfWork.RegisterGlobalUnitOfWork(new StubUnitOfWork())) {
                IUnitOfWork stub = new StubUnitOfWork();
                UnitOfWork.RegisterGlobalUnitOfWork(stub);

                UnitOfWork.Start();

                Assert.AreEqual(stub, UnitOfWork.Current);
            }
        }

        private class StubUnitOfWork : IUnitOfWork {
            #region IUnitOfWork Members

            public void Flush() {}

            public void Clear() {}

            public bool IsInActiveTransaction {
                get { return false; }
            }

            public IUnitOfWorkTransaction BeginTransaction() {
                return null;
            }

            public IUnitOfWorkTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel) {
                return null;
            }

            public void TransactionalFlush() {}

            public void TransactionalFlush(System.Data.IsolationLevel isolationLevel) {}

            #endregion

            #region IDisposable Members

            public void Dispose() {}

            #endregion
        }
    }
}