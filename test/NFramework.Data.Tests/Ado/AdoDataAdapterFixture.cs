using System.Collections.Generic;
using System.Data;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// DataReader로부터 DataSet이나 DataTable을 Fill하기 위한 DataAdapter의 protected method를 사용할 수 있도록 하였습니다.
    /// </summary>
    [TestFixture]
    public class AdoDataAdapterFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private IDataReader CustomerOrderDetailDataReader() {
            var spName = NorthwindAdoRepository.QueryProvider.GetQuery("CustomerOrdersDetail");
            return NorthwindAdoRepository.ExecuteReaderByProcedure(spName, OrderTestParameter);
        }

        [Test]
        public void Load_CustomerOrderHistory() {
            using(var adapter = new AdoDataAdapter(NorthwindAdoRepository.GetDataAdapter()))
            using(var reader = CustomerOrderDetailDataReader()) {
                Assert.IsNotNull(reader);
                Assert.Greater(reader.FieldCount, 0);

                var dataTable = new DataTable();

                adapter.Fill(new[] { dataTable }, reader, 5, 10);

                Assert.IsFalse(dataTable.HasErrors);
                Assert.AreEqual(10, dataTable.Rows.Count);
            }
        }

        [Test]
        public void Load_TenMostExpensiveProduct() {
            var spName = NorthwindAdoRepository.QueryProvider.GetQuery("TenMostExpensiveProduct");
            Assert.IsNotEmpty(spName);

            if(IsDebugEnabled)
                log.Debug("Execute Procedure... spName=[{0}]", spName);

            using(var adapter = new AdoDataAdapter(NorthwindAdoRepository.GetDataAdapter()))
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure(spName)) {
                Assert.IsNotNull(reader);
                Assert.Greater(reader.FieldCount, 0);

                var dataTable = new DataTable();
                adapter.Fill(new[] { dataTable }, reader, 2, 2);

                Assert.IsFalse(dataTable.HasErrors);
                Assert.AreEqual(2, dataTable.Rows.Count);
            }
        }

        [Test]
        public void Load_Nullable() {
            var spName = NorthwindAdoRepository.QueryProvider.GetQuery("TenMostExpensiveProduct");

            using(var adapter = new AdoDataAdapter(NorthwindAdoRepository.GetDataAdapter()))
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure(spName)) {
                Assert.IsNotNull(reader);

                var dataTable = new DataTable();

                adapter.Fill(new[] { dataTable }, reader, 2, 2);

                Assert.IsFalse(dataTable.HasErrors);
                Assert.AreEqual(2, dataTable.Rows.Count);
            }
        }

        [Test]
        public void Load_MultipleResultSet() {
            var tables = new List<DataTable>();

            var sqlString = string.Concat(SQL_ORDER_SELECT, ";", SQL_ORDER_DETAIL_SELECT);

            using(var adapter = new AdoDataAdapter(NorthwindAdoRepository.GetDataAdapter()))
            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(sqlString)) {
                do {
                    var table = new DataTable();
                    adapter.Fill(new[] { table }, reader, 0, 0);

                    tables.Add(table);
                } while(reader.IsClosed == false && reader.NextResult());
            }

            Assert.AreEqual(2, tables.Count);
            tables.All(table => table.HasErrors == false);
            tables.RunEach(table => {
                               Assert.Greater(table.Rows.Count, 0);
                               if(IsDebugEnabled)
                                   log.Debug(table.CreateDataReader().ToString(true));
                           });
        }
    }
}