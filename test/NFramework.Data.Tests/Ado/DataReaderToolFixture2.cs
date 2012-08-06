using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data {
    [TestFixture]
    public class DataReaderToolFixture2 : AdoFixtureBase {
        [Test]
        public void DataReaderGetByIndex() {
            using(var dr = NorthwindAdoRepository.ExecuteReaderBySqlString("SELECT * FROM [Order Details]")) {
                while(dr.Read()) {
                    var orderId = DataReaderTool.AsInt32(dr, 0, () => 10000);
                    Assert.AreNotEqual(10000, orderId);

                    var productId = DataReaderTool.AsInt32(dr, 1);
                    Assert.AreNotEqual(0, productId);

                    var unitPrice = DataReaderTool.AsDecimal(dr, 2);
                    Assert.AreNotEqual(0M, unitPrice);
                }
            }
        }

        [Test]
        public void CanGetNullableFromOrderDetails() {
            using(var dr = NorthwindAdoRepository.ExecuteReaderBySqlString("SELECT * FROM [Order Details]")) {
                while(dr.Read()) {
                    var orderId = DataReaderTool.AsInt32Nullable(dr, 0);
                    Assert.IsTrue(orderId.HasValue);
                    Assert.AreEqual(orderId, dr.GetInt32(0));

                    var productId = DataReaderTool.AsInt32Nullable(dr, 1);
                    Assert.IsTrue(productId.HasValue);
                    Assert.AreEqual(productId, dr.GetInt32(1));

                    var unitPrice = DataReaderTool.AsDecimalNullable(dr, 2);
                    Assert.IsTrue(unitPrice.HasValue);
                    Assert.AreEqual(unitPrice, dr.GetDecimal(2));

                    var discount = DataReaderTool.AsFloatNullable(dr, "Discount");
                    Assert.IsNotNull(discount);
                }
            }
        }

        [Test]
        public void CanGetNullableFromProducts() {
            using(var dr = NorthwindAdoRepository.ExecuteReaderBySqlString("SELECT * FROM Products")) {
                while(dr.Read()) {
                    var unitPrice = DataReaderTool.AsDecimalNullable(dr, "UnitPrice");
                    Assert.IsTrue(unitPrice.HasValue);
                    //Console.WriteLine("UnitPrice = " + unitPrice);
                    Assert.Greater(unitPrice.Value, 0m);

                    var reorderLevel = DataReaderTool.AsInt16Nullable(dr, "ReorderLevel");
                    Assert.IsTrue(reorderLevel.HasValue);
                    Assert.GreaterOrEqual(reorderLevel.Value, 0);
                }
            }
        }

        [Test]
        public void DataReaderGetByName() {
            using(var dr = NorthwindAdoRepository.ExecuteReaderBySqlString("SELECT * FROM Employees")) {
                while(dr.Read()) {
                    Assert.IsNotEmpty(DataReaderTool.AsString(dr, "Title"));
                    Assert.IsNotNull(DataReaderTool.AsDateTimeNullable(dr, "BirthDate"));
                }
            }
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(5,
                              DataReaderGetByIndex,
                              CanGetNullableFromOrderDetails,
                              CanGetNullableFromProducts,
                              DataReaderGetByName);
        }
    }
}