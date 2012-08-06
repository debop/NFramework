using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class SupplierFixture : NorthwindDbTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void FindAll() {
            var suppliers = Repository<Supplier>.FindAll();

            Repository<Supplier>.Count().Should().Be.GreaterThan(0);
            suppliers.Count.Should().Be.GreaterThan(0);
        }
    }
}