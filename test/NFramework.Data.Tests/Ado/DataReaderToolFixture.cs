using System.Data;
using NUnit.Framework;

namespace NSoft.NFramework.Data {
    [TestFixture]
    public class DataReaderToolFixture : AdoFixtureBase {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private IDataReader CustomerOrderDetailDataReader() {
            var spName = NorthwindAdoRepository.QueryProvider.GetQuery("CustomerOrdersDetail");
            return NorthwindAdoRepository.ExecuteReaderByProcedure(spName, OrderTestParameter);
        }

        [Test]
        public void Load_CustomerOrderHistory() {
            using(var reader = CustomerOrderDetailDataReader()) {
                Assert.IsNotNull(reader);
                Assert.Greater(reader.FieldCount, 0);

                Assert.IsTrue(reader.Read());
            }
        }

        [Test]
        public void Load_TenMostExpensiveProduct() {
            var spName = NorthwindAdoRepository.QueryProvider.GetQuery("TenMostExpensiveProduct");
            Assert.IsNotEmpty(spName);

            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure(spName)) {
                Assert.IsNotNull(reader);
                Assert.Greater(reader.FieldCount, 0);

                while(reader.Read()) {
                    var unitPrice = reader.AsDecimalNullable("UnitPrice");
                    Assert.IsTrue(unitPrice.HasValue);
                    Assert.GreaterOrEqual(unitPrice.GetValueOrDefault(0m), 0m);
                }
            }
        }

        [Test]
        public void Load_Nullable() {
            var spName = NorthwindAdoRepository.QueryProvider.GetQuery("TenMostExpensiveProduct");
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure(spName)) {
                Assert.IsNotNull(reader);

                while(reader.Read()) {
                    var unitPrice = reader.AsDecimalNullable("UnitPrice");
                    Assert.IsTrue(unitPrice.HasValue);
                    Assert.Greater(unitPrice.GetValueOrDefault(0m), 0m);
                }
            }
        }
    }
}