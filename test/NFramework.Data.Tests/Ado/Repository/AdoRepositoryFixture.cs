using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Threading;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Repositories {
    [TestFixture]
    public class AdoRepositoryFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string GetCustomerOrderHistorySql = "CustOrderHist";
        private const string GetOrdersSql = "SELECT * FROM Orders";
        private const string GetOrderDetailsSql = "SELECT * FROM [Order Details]";

        private const string GetOrderByOrderDateAndFreightSql =
            @"SELECT * FROM Orders WHERE OrderDate < @OrderDate AND Freight < @Freight";

        private const string GetOrderByOrderDateAndFreightAndCustomerSql =
            @"SELECT * From Orders where OrderDate < @OrderDate and Freight < @Freight and CustomerID = @CustomerID";

        [TestFixtureTearDown]
        public void ClassTearDown() {
            Thread.Sleep(1);
        }

        [Test]
        public void Can_Load_Current_Repository() {
            var current = AdoRepository.Current;
            Assert.IsNotNull(current);
            Assert.AreEqual(AdoTool.DefaultDatabaseName, current.DbName);
        }

        #region << GetNamedQueryCommand >>

        [TestCase("Order, GetAll")]
        [TestCase("Employees, GetAll")]
        [TestCase("Invoices, GetAll")]
        public void Can_Create_Command_By_NamedQueryCommand_With_QueryKey(string queryKey) {
            if(NorthwindAdoRepository.QueryProvider != null) {
                using(var cmd = NorthwindAdoRepository.GetNamedQueryCommand(queryKey))
                using(var dataTable = NorthwindAdoRepository.ExecuteDataTable(cmd)) {
                    Assert.IsNotNull(dataTable);
                    Assert.IsFalse(dataTable.HasErrors);
                    Assert.IsTrue(dataTable.Rows.Count > 0);
                }

                using(var dataTable = NorthwindAdoRepository.ExecuteDataTable(queryKey)) {
                    Assert.IsNotNull(dataTable);
                    Assert.IsFalse(dataTable.HasErrors);
                    Assert.IsTrue(dataTable.Rows.Count > 0);
                }
            }
        }

        [TestCase("Order", "GetAll")]
        [TestCase("Employees", "GetAll")]
        [TestCase("Invoices", "GetAll")]
        public void Can_Create_Command_By_NamedQueryCommand_With_Section_And_QueryName(string section, string queryName) {
            if(NorthwindAdoRepository.QueryProvider != null) {
                using(var cmd = NorthwindAdoRepository.GetNamedQueryCommand(section, queryName))
                using(var dataTable = NorthwindAdoRepository.ExecuteDataTable(cmd)) {
                    Assert.IsNotNull(dataTable);
                    Assert.IsFalse(dataTable.HasErrors);
                    Assert.IsTrue(dataTable.Rows.Count > 0);
                }

                var queryKey = section + "," + queryName;
                using(var dataTable = NorthwindAdoRepository.ExecuteDataTable(queryKey)) {
                    Assert.IsNotNull(dataTable);
                    Assert.IsFalse(dataTable.HasErrors);
                    Assert.IsTrue(dataTable.Rows.Count > 0);
                }
            }
        }

        #endregion

        #region << DataSet >>

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSet(int firstResult, int maxResults) {
            DataSet ds;

            using(var cmd = NorthwindAdoRepository.GetCommand(GetOrderDetailsSql))
            using(ds = NorthwindAdoRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 1);
                Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }

            using(ds = NorthwindAdoRepository
                           .ExecuteDataSetBySqlString(GetOrderByOrderDateAndFreightAndCustomerSql,
                                                      firstResult,
                                                      maxResults,
                                                      new AdoParameter("OrderDate", DateTime.Today),
                                                      new AdoParameter("Freight", 2),
                                                      CustomerTestParameter)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                // Assert.Greater(ds.Tables[0].Rows.Count, 1);
                Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSetByQuery(int firstResult, int maxResults) {
            DataSet ds = null;

            using(ds = NorthwindAdoRepository.ExecuteDataSet(GetOrderDetailsSql, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 1);
                Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSetInTransactionScope() {
            Action<int, int> loadMethod = (firstResult, maxResults) => {
                                              using(var cmd = NorthwindAdoRepository.GetCommand(GetOrderDetailsSql))
                                              using(var ds = NorthwindAdoRepository.ExecuteDataSet(cmd, 5, 5)) {
                                                  Assert.AreEqual(ds.Tables.Count, 1);
                                                  Assert.Greater(ds.Tables[0].Rows.Count, 1);
                                                  Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
                                              }
                                          };
            AdoWith.TransactionScope(delegate {
                                         loadMethod(5, 10);
                                         loadMethod(5, 5);
                                         loadMethod(1, 1);
                                     },
                                     delegate {
                                         loadMethod(5, 10);
                                         loadMethod(5, 5);
                                         loadMethod(1, 1);
                                     },
                                     delegate {
                                         loadMethod(5, 10);
                                         loadMethod(5, 5);
                                         loadMethod(1, 1);
                                     });
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSetByPaging(int firstResult, int maxResults) {
            DataSet ds = null;

            using(var cmd = NorthwindAdoRepository.GetCommand(GetOrderDetailsSql))
            using(ds = NorthwindAdoRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                Console.WriteLine("RowCount=[{0}]", ds.Tables[0].Rows.Count);
            }

            using(ds = NorthwindAdoRepository.ExecuteDataSetBySqlString(GetOrderDetailsSql, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                Console.WriteLine("RowCount=[{0}]", ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSetByProcedure() {
            using(var ds = NorthwindAdoRepository.ExecuteDataSetByProcedure(GetCustomerOrderHistorySql, CustomerTestParameter)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                Console.WriteLine("RowCount=[{0}]", ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSet_MultiResultSet_BySqlString() {
            var sql = string.Concat(GetOrdersSql, ";", GetOrderDetailsSql);

            using(var cmd = NorthwindAdoRepository.GetSqlStringCommand(sql))
            using(var ds = NorthwindAdoRepository.ExecuteDataSet(cmd)) {
                Assert.AreEqual(2, ds.Tables.Count);
                ds.Tables.Cast<DataTable>().All(table => table.HasErrors == false).Should().Be.True();
            }
        }

        [Test]
        public void ExecuteDataSet_MultiResultSet_ByProcedure() {
            using(var cmd = NorthwindAdoRepository.GetProcedureCommand("OrderAndOrderDetails")) {
                using(var ds = NorthwindAdoRepository.ExecuteDataSet(cmd)) {
                    ds.Tables.Count.Should().Be(2);
                    ds.Tables.Cast<DataTable>().All(table => table.HasErrors == false).Should().Be.True();
                }
            }
        }

        #endregion

        #region << DataTable >>

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTable(int firstResult, int maxResults) {
            using(var cmd = NorthwindAdoRepository.GetCommand(GetOrderDetailsSql))
            using(var table = NorthwindAdoRepository.ExecuteDataTable(cmd, firstResult, maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount=[{0}]", table.Rows.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableByQuery(int firstResult, int maxResults) {
            using(var table = NorthwindAdoRepository.ExecuteDataTable(GetOrderDetailsSql, firstResult, maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount=[{0}]", table.Rows.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableAndCopy(int firstResult, int maxResults) {
            // DataTable을 반환받아 다른 DataSet에 저장할 수 있는지 파악한다.

            using(var dataset = new DataSet())
            using(var cmd = NorthwindAdoRepository.GetCommand(GetOrderDetailsSql))
            using(var table = NorthwindAdoRepository.ExecuteDataTable(cmd, firstResult, maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount=[{0}]", table.Rows.Count);

                dataset.Tables.Add(table);
                Assert.AreEqual(1, dataset.Tables.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableBySqlString(int firstResult, int maxResults) {
            using(var table = NorthwindAdoRepository.ExecuteDataTableBySqlString(GetOrderDetailsSql,
                                                                                 firstResult,
                                                                                 maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount=[{0}]", table.Rows.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableByProcedure(int firstResult, int maxResults) {
            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure(GetCustomerOrderHistorySql,
                                                                                 firstResult,
                                                                                 maxResults,
                                                                                 CustomerTestParameter)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount=[{0}]", table.Rows.Count);
            }
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        public void ExecuteDataTableToInstance(int firstResult, int maxResults) {
            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure(GetCustomerOrderHistorySql,
                                                                                 firstResult,
                                                                                 maxResults,
                                                                                 CustomerTestParameter)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 1);

                var orderHistories = AdoTool.Map<CustomerOrderHistory>(table);
                Assert.Greater(orderHistories.Count, 1);
            }
        }

        #endregion

        #region << DataTable AsList >>

        [Test]
        public void ExecuteDataTableAsList_BySqlString() {
            var sql = string.Concat(GetOrdersSql, ";", GetOrderDetailsSql);

            using(var cmd = NorthwindAdoRepository.GetSqlStringCommand(sql)) {
                var tables = NorthwindAdoRepository.ExecuteDataTableAsList(cmd);

                tables.Count.Should().Be(2);
                tables.All(table => table.HasErrors == false).Should().Be.True();
            }
        }

        [Test]
        public void ExecuteDataTableAsList_ByProcedure() {
            using(var cmd = NorthwindAdoRepository.GetProcedureCommand("OrderAndOrderDetails")) {
                var tables = NorthwindAdoRepository.ExecuteDataTableAsList(cmd).ToList();

                Assert.AreEqual(2, tables.Count);
                tables.All(table => table.HasErrors == false).Should().Be.True();
            }
        }

        #endregion

        #region << Execute Paging DataTable >>

        /// <summary>
        /// 단일 QUERY 문으로 PAGING을 수행합니다.
        /// </summary>
        [TestCase(0, 10, 0.0)]
        [TestCase(2, 10, 0.05)]
        [TestCase(3, 10, 0.05)]
        [TestCase(1, 10, 0.0)]
        [TestCase(3, 10, 0.05)]
        [TestCase(4, 10, 0.05)]
        public void ExecutePagingDataTableTest(int pageIndex, int pageSize, double discount) {
            var parameters = new[] { new AdoParameter("Discount", discount) };

            using(
                new OperationTimer(string.Format("ExecutePagingDataTable(pageIndex=[{0}], pageSize=[{1}])", pageIndex, pageSize), false)
                ) {
                using(var cmd = NorthwindAdoRepository.GetCommand(GetOrderDetailsSql + " WHERE Discount > @Discount"))
                using(var pagingTable = NorthwindAdoRepository.ExecutePagingDataTable(cmd, pageIndex, pageSize, parameters)) {
                    Assert.AreEqual(pageIndex, pagingTable.PageIndex);
                    Assert.AreEqual(pageSize, pagingTable.PageSize);

                    Assert.IsTrue(pagingTable.TotalPageCount > 0);
                    Assert.IsTrue(pagingTable.TotalItemCount > 0);

                    Assert.IsTrue(pagingTable.Table.Rows.Count > 0);
                    Assert.IsTrue(pagingTable.Table.Rows.Count <= pageSize);

                    var table = pagingTable.Table;
                    Assert.IsNotNull(table);
                    Assert.IsFalse(table.HasErrors);
                    Assert.IsTrue(table.Rows.Count > 0);
                }
            }
        }

        /// <summary>
        /// 단일 QUERY 문으로 PAGING을 수행합니다.
        /// </summary>
        [TestCase(0, 10, 0.0)]
        [TestCase(2, 10, 0.05)]
        [TestCase(3, 10, 0.05)]
        [TestCase(1, 10, 0.0)]
        [TestCase(3, 10, 0.05)]
        [TestCase(4, 10, 0.05)]
        public void ExecutePagingDataTableByQueryTest(int pageIndex, int pageSize, double discount) {
            var parameters = new[] { new AdoParameter("Discount", discount) };

            using(
                new OperationTimer(string.Format("ExecutePagingDataTableByQuery(pageIndex=[{0}], pageSize=[{1}])", pageIndex, pageSize))
                ) {
                using(
                    var pagingTable = NorthwindAdoRepository.ExecutePagingDataTable(GetOrderDetailsSql + " WHERE Discount > @Discount",
                                                                                    pageIndex,
                                                                                    pageSize,
                                                                                    parameters)) {
                    Assert.AreEqual(pageIndex, pagingTable.PageIndex);
                    Assert.AreEqual(pageSize, pagingTable.PageSize);

                    Assert.IsTrue(pagingTable.TotalPageCount > 0);
                    Assert.IsTrue(pagingTable.TotalItemCount > 0);

                    Assert.IsTrue(pagingTable.Table.Rows.Count > 0);
                    Assert.IsTrue(pagingTable.Table.Rows.Count <= pageSize);

                    var table = pagingTable.Table;
                    Assert.IsNotNull(table);
                    Assert.IsFalse(table.HasErrors);
                    Assert.IsTrue(table.Rows.Count > 0);
                }
            }
        }

        /// <summary>
        /// 단일 QUERY 문으로 PAGING을 수행합니다.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="discount"></param>
        [TestCase(0, 10, 0.0)]
        [TestCase(2, 10, 0.05)]
        [TestCase(3, 10, 0.05)]
        [TestCase(1, 10, 0.0)]
        [TestCase(3, 10, 0.05)]
        [TestCase(4, 10, 0.05)]
        public void ExecutePagingDataTableBySelectSqlTest(int pageIndex, int pageSize, double discount) {
            const string selectSql = GetOrderDetailsSql + @" WHERE Discount > @Discount";
            var parameters = new[] { new AdoParameter("Discount", discount) };

            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex={0}, pageSize={1})", pageIndex, pageSize))
                )
            using(var pagingTable = NorthwindAdoRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize, parameters)
                ) {
                Assert.AreEqual(pageIndex, pagingTable.PageIndex);
                Assert.AreEqual(pageSize, pagingTable.PageSize);

                Assert.IsTrue(pagingTable.TotalPageCount > 0);
                Assert.IsTrue(pagingTable.TotalItemCount > 0);

                Assert.IsTrue(pagingTable.Table.Rows.Count > 0);
                Assert.IsTrue(pagingTable.Table.Rows.Count <= pageSize);

                var table = pagingTable.Table;
                Assert.IsNotNull(table);
                Assert.IsFalse(table.HasErrors);
                Assert.IsTrue(table.Rows.Count > 0);
            }
        }

        [TestCase("SELECT OD.*, (SELECT TOP 1 name FROM sysobjects) AS SYS_NAME FROM [Order Details] AS OD ORDER BY OrderID ", 0, 100)]
        [TestCase("SELECT OD.*, (SELECT TOP 1 name FROM sysobjects) AS SYS_NAME FROM [Order Details] AS OD ORDER BY OrderID ", 0, 100)]
        [TestCase("SELECT OD.*, (SELECT TOP 1 name FROM sysobjects) AS SYS_NAME FROM [Order Details] AS OD ORDER BY OrderID", 2, 100)]
        public void ExecutePagingDataTableBySelectSqlTest(string selectSql, int pageIndex, int pageSize) {
            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex=[{0}], pageSize=[{1}])", pageIndex,
                                                 pageSize)))
            using(var pagingTable = NorthwindAdoRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize)) {
                Assert.AreEqual(pageIndex, pagingTable.PageIndex);
                Assert.AreEqual(pageSize, pagingTable.PageSize);

                Assert.IsTrue(pagingTable.TotalPageCount > 0);
                Assert.IsTrue(pagingTable.TotalItemCount > 0);

                Assert.IsTrue(pagingTable.Table.Rows.Count > 0);
                Assert.IsTrue(pagingTable.Table.Rows.Count <= pageSize);

                var table = pagingTable.Table;
                Assert.IsNotNull(table);
                Assert.IsFalse(table.HasErrors);
                Assert.IsTrue(table.Rows.Count > 0);
            }
        }

        #endregion

        #region << Execute Scalar >>

        [Test]
        public void ExecuteScalar() {
            var count = NorthwindAdoRepository.ExecuteScalarBySqlString("SELECT COUNT(*) FROM dbo.Orders").AsInt();
            Assert.Greater(count, 0);

            var filterCount
                = NorthwindAdoRepository
                    .ExecuteScalarBySqlString("SELECT COUNT(*) FROM Orders where OrderDate < @OrderDate and Freight < @Freight",
                                              new AdoParameter("OrderDate", DateTime.Today),
                                              new AdoParameter("Freight", 2))
                    .AsInt();

            Assert.Greater(filterCount, 0);

            var orderId =
                NorthwindAdoRepository
                    .ExecuteScalarBySqlString("SELECT TOP 1 ISNULL(OrderID, 0) FROM Orders ORDER BY ShippedDate")
                    .AsInt(0);

            Assert.AreNotEqual(0, orderId);

            orderId =
                NorthwindAdoRepository
                    .ExecuteScalarBySqlString("SELECT TOP 1 ISNULL(OrderID, 0) FROM Orders Where ShipVia=0 ORDER BY ShippedDate")
                    .AsInt(0);

            Assert.AreEqual(0, orderId);
        }

        #endregion

        #region << Execute NonQuery >>

        [Test]
        public void ExecuteNonQuery() {
            var row = NorthwindAdoRepository
                .ExecuteNonQueryBySqlString("DELETE FROM Employees where LastName=@LastName and FirstName=@FirstName",
                                            new AdoParameter("LastName", "Bae"),
                                            new AdoParameter("FirstName", "Sunghyouk"));

            Console.WriteLine("Row affected=[{0}]", row);
        }

        #endregion

        #region << ExecuteReader >>

        [Test]
        public void ExecuteReaderByQuery() {
            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(GetOrderDetailsSql)) {
                Assert.IsTrue(reader.Read());

                if(IsDebugEnabled)
                    log.Debug(reader.ToString(true));
            }
        }

        [Test]
        public void ExecuteReaderByProcedure() {
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure(GetCustomerOrderHistorySql, CustomerTestParameter)) {
                Assert.IsTrue(reader.Read());

                if(IsDebugEnabled)
                    log.Debug(reader.ToString(true));
            }
        }

        #endregion

        #region << Count Of DataReader >>

        [Test]
        public void CountOfDataReader() {
            Assert.AreEqual(5, NorthwindAdoRepository.CountBySqlString("SELECT TOP 5 * FROM [Order Details]"));

            Assert.AreEqual(1, NorthwindAdoRepository.CountBySqlString("SELECT TOP 1 * FROM Customers"));

            Assert.AreEqual(0, NorthwindAdoRepository.CountBySqlString("SELECT TOP 0 * FROM Customers"));

            var count = NorthwindAdoRepository.CountByProcedure(GetCustomerOrderHistorySql, CustomerTestParameter);

            Assert.IsTrue(count > 0);
        }

        #endregion

        #region << Exists >>

        [Test]
        public void ExistsByDataReader() {
            Assert.IsTrue(NorthwindAdoRepository.ExistsBySqlString("SELECT TOP 1 * FROM Customers"));
            Assert.IsFalse(NorthwindAdoRepository.ExistsBySqlString("SELECT TOP 0 * FROM Customers"));

            Assert.IsFalse(
                NorthwindAdoRepository.ExistsBySqlString("SELECT * FROM Customers WHERE CustomerID=@CustomerID",
                                                         new AdoParameter("CustomerID", "DEBOP68")));

            Assert.IsTrue(NorthwindAdoRepository.ExistsBySqlString("SELECT TOP 1 * FROM [Order Details]"));
            Assert.IsTrue(NorthwindAdoRepository.ExistsByProcedure(GetCustomerOrderHistorySql, CustomerTestParameter));
        }

        #endregion

        #region << ExecuteReader{TPersisntent} >>

        [Test]
        public void ExecuteReaderToInstanceByNameMapping() {
            using(var cmd = NorthwindAdoRepository.GetCommand(GetCustomerOrderHistorySql)) {
                var orderHistories = NorthwindAdoRepository.ExecuteInstance<CustomerOrderHistory>(NameMappings, cmd,
                                                                                                  CustomerTestParameter);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());
            }
        }

        [TestCase(0, 10)]
        [TestCase(3, 20)]
        [TestCase(5, 10)]
        public void ExecuteReaderWithPagingByNameMapper(int pageIndex, int pageSize) {
            using(var cmd = NorthwindAdoRepository.GetCommand(SQL_ORDER_DETAIL_SELECT)) {
                var orderDetails = NorthwindAdoRepository.ExecuteInstance<OrderDetail>(TrimMapper, cmd, pageIndex, pageSize);

                Console.WriteLine(orderDetails);
            }
        }

        [TestCase(0, 10)]
        [TestCase(3, 20)]
        [TestCase(5, 10)]
        public void ExecuteReaderWithPagingByPersister(int pageIndex, int pageSize) {
            var persister = new TrimReaderPersister<OrderDetail>();

            using(var cmd = NorthwindAdoRepository.GetCommand(SQL_ORDER_DETAIL_SELECT)) {
                var orderDetails = NorthwindAdoRepository.ExecuteInstance<OrderDetail>(persister, cmd, pageIndex, pageSize);

                Console.WriteLine(orderDetails);
            }
        }

        internal static CustomerOrderHistory DataReaderToCustomerOrderHistory(IDataReader dr) {
            Assert.IsNotNull(dr);

            var reader = dr;

            return new CustomerOrderHistory
                   {
                       ProductName = reader.AsString("ProductName"),
                       Total = reader.AsInt32Nullable("Total")
                   };
        }

        /// <summary>
        /// converter 를 이용하여 IDataReader를 지정된 Class의 인스턴스로 만든다.
        /// </summary>
        [Test]
        public void ExecuteReaderToInstanceByConverter() {
            Func<IDataReader, CustomerOrderHistory> @mapFunc
                = delegate(IDataReader dr) {
                      var reader = dr;
                      return new CustomerOrderHistory
                             {
                                 ProductName = reader.AsString("ProductName"),
                                 Total = reader.AsInt32Nullable("Total")
                             };
                  };

            using(var cmd = NorthwindAdoRepository.GetCommand(GetCustomerOrderHistorySql)) {
                var orderHistories = NorthwindAdoRepository.ExecuteInstance(@mapFunc, cmd, CustomerTestParameter);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());
            }

            Func<IDataReader, CustomerOrderHistory> mapFunc2 = DataReaderToCustomerOrderHistory;

            using(var cmd = NorthwindAdoRepository.GetCommand(GetCustomerOrderHistorySql)) {
                var orderHistories = NorthwindAdoRepository.ExecuteInstance(mapFunc2, cmd, CustomerTestParameter);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());
            }
        }

        #endregion

        #region << Execute Command / Procedure >>

        /// <summary>
        /// Output Parameter, Return Value 얻기
        /// </summary>
        [Test]
        public void ExecuteProcedure() {
            var outputs = NorthwindAdoRepository.ExecuteProcedure(GetCustomerOrderHistorySql, CustomerTestParameter);

            Assert.IsNotNull(outputs);
            Assert.Greater(outputs.Count(), 0);
        }

        #endregion

        #region << Fluent >>

        /// <summary>
        /// Stored Procedure로부터 DataReader나 DataTable을 얻고, 그 정보로 Persistent Object를 빌드한다.
        /// </summary>
        [Test]
        public void DatabaseToPersistentObject() {
            // CustOrderHist2 는 컬럼명만 PROJECT_NAME, TOTAL 로 변경한 것이다.
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist2", CustomerTestParameter)) {
                var orderHistories = reader.Map<CustomerOrderHistory>(reader.Mapping(NameMappingUtil.CapitalizeMappingFunc('_', ' ')));

                Assert.IsTrue(orderHistories.Count > 0);
                Console.WriteLine("Order History: " + orderHistories.CollectionToString());
            }
        }

        [Test]
        public void PersistentObjectToDatabase() {
            try {
                var category = new Category { CategoryName = "Test", Description = "FluentUtil" };

                // delete exist category
                NorthwindAdoRepository.ExecuteNonQueryBySqlString("DELETE FROM Categories where CategoryName = @CategoryName",
                                                                  new AdoParameter("CategoryName", category.CategoryName));

                // insert
                var result = NorthwindAdoRepository.ExecuteEntity("SaveOrUpdateCategory", category, CapitalizeMapper);

                category.CategoryId = result.AsInt(-1);
                Assert.AreNotEqual(-1, category.CategoryId);

                // update
                result = NorthwindAdoRepository.ExecuteEntity("SaveOrUpdateCategory", category, CapitalizeMapper);
                Assert.IsTrue(result.AsInt() > 0);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.Error(ex);

                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region << Fluent By INameMapper >>

        [Test]
        public void FluentByNameMapper_Load() {
            INameMapper nameMapper = new CapitalizeNameMapper();

            using(var cmd = AdoRepository.GetCommand("CustOrderHist2")) {
                var orderHistories = AdoRepository.ExecuteInstance<CustomerOrderHistory>(nameMapper, cmd, CustomerTestParameter);

                orderHistories.Count.Should().Be.GreaterThan(0);
                CollectionAssert.AllItemsAreNotNull((ICollection)orderHistories);
                CollectionAssert.AllItemsAreInstancesOfType((ICollection)orderHistories, typeof(CustomerOrderHistory));
            }
        }

        [Test]
        public void FluentByNameMapper_Save() {
            INameMapper nameMapper = new CapitalizeNameMapper();

            var category = new Category
                           {
                               CategoryName = "Test",
                               Description = "FluentUtil"
                           };

            // delete exist category
            NorthwindAdoRepository.ExecuteNonQueryBySqlString("DELETE FROM Categories where CategoryName = @CategoryName",
                                                              new AdoParameter("CategoryName", category.CategoryName));

            // insert
            var result = NorthwindAdoRepository.ExecuteEntity("SaveOrUpdateCategory", category, nameMapper);

            category.CategoryId = result.AsInt(-1);
            Assert.AreNotEqual(-1, category.CategoryId);

            // update
            result = NorthwindAdoRepository.ExecuteEntity("SaveOrUpdateCategory", category, nameMapper);
            Assert.IsTrue(result.AsInt() > 0);
        }

        #endregion

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(15,
                              ExecuteDataSetInTransactionScope,
                              ExecuteDataSetByProcedure,
                              ExecuteDataSet_MultiResultSet_BySqlString,
                              ExecuteDataSet_MultiResultSet_ByProcedure,
                              ExecuteDataTableAsList_BySqlString,
                              ExecuteDataTableAsList_ByProcedure,
                              ExecuteScalar,
                              ExecuteNonQuery,
                              ExecuteReaderByQuery,
                              ExecuteReaderByProcedure,
                              CountOfDataReader,
                              ExistsByDataReader,
                              ExecuteReaderToInstanceByNameMapping,
                              ExecuteReaderToInstanceByConverter,
                              ExecuteProcedure,
                              DatabaseToPersistentObject,
                              PersistentObjectToDatabase,
                              FluentByNameMapper_Load,
                              FluentByNameMapper_Save);
            ;
        }
    }
}