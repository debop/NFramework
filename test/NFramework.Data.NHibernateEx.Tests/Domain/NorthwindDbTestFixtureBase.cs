using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using NHibernate.Type;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    public abstract class NorthwindDbTestFixtureBase : FluentDatabaseTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [TestFixtureSetUp]
        public void DatabaseSetUp() {
            if(IoC.IsNotInitialized)
                IoC.Initialize(new WindsorContainer(@".\Domain\Windsor.Northwind.config"));

            if(UnitOfWork.IsNotStarted)
                UnitOfWork.Start();
        }

        [TestFixtureTearDown]
        public void DatabaseClearUp() {
            UnitOfWork.Stop();
            IoC.Reset();
        }

        public Customer TestCustomer {
            get {
                return new Customer("CACTU", "Cactus Comidas para llevar")
                       {
                           ContactName = "Patricio Simpson"
                       };
            }
        }

        public INHParameter CustomerParameter {
            get { return new NHParameter("CustomerId", "ANATR", TypeFactory.GetStringType(4000)); }
        }

        public static INHRepository<T> GetRepository<T>() where T : class, IDataObject {
            return IoC.Resolve<INHRepository<T>>();
        }

        public static void Print<T>(IEnumerable<T> collection) {
            Print(collection, 5);
        }

        public static void Print<T>(IEnumerable<T> collection, int? count) {
            if(IsDebugEnabled) {
                log.Debug("최대 [{0}]개만 찍는다...", count.GetValueOrDefault(5));
                log.Debug("---------------------------------------------------------");
                collection.Take(count.GetValueOrDefault(5)).RunEach(x => log.Debug(x));
                log.Debug("---------------------------------------------------------");
            }
        }
    }
}