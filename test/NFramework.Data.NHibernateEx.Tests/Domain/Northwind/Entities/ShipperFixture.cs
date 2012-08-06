using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class ShipperFixture : NorthwindDbTestFixtureBase {
        private static INHRepository<Shipper> ShipperRepository {
            get { return GetRepository<Shipper>(); }
        }

        [Test]
        public void FindAll() {
            var shippers = ShipperRepository.FindAll();
            Assert.AreEqual(shippers.Count, ShipperRepository.Count());
        }

        [TestCase(0, 10)]
        [TestCase(2, 100)]
        public void FindAllByPaging(int firstResult, int maxResults) {
            var shippers = ShipperRepository.FindAll(firstResult, maxResults);
            Assert.Greater(shippers.Count, 0, "Shippers count=" + shippers.Count);
        }
    }
}