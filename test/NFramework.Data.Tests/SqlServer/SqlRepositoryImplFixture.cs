using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Threading;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Data.SqlServer;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Ado.SqlServer {
    /// <summary>
    /// NOTE: Command를 중복해서 사용하면 안됩니다. (비동기 방식이라 다른 작업에서 Command의 Connection이 닫힐 수 있습니다)
    /// NOTE: 중복 사용을 하고 싶다면, 메소드 호출 전에 Command에 Connection을 미리 지정해 주고, 모든 비동기 작업이 끝난 후에 Connection을 닫아야 합니다.
    /// </summary>
    [TestFixture]
    public class SqlRepositoryImplFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string GetCustomerOrderHistorySql = @"CustOrderHist";
        private const string GetCustomerSql = @"SELECT * FROM dbo.Customers WITH (NOLOCK)";
        private const string GetOrdersSql = @"SELECT * FROM dbo.Orders WITH (NOLOCK)";
        private const string GetOrderDetailsSql = @"SELECT * FROM [Order Details] WITH (NOLOCK)";

        private const string GetOrderByOrderDateAndFreightSql =
            @"SELECT * FROM Orders WITH (NOLOCK) WHERE OrderDate < @OrderDate AND Freight < @Freight";

        private const string GetOrderByOrderDateAndFreightAndCustomerSql =
            @"SELECT * From Orders WITH (NOLOCK) where OrderDate < @OrderDate and Freight < @Freight and CustomerID = @CustomerID";

        static SqlRepositoryImplFixture() {
            SqlRepository = new SqlRepositoryImpl(AdoTool.DefaultDatabaseName);
        }

        public static ISqlRepository SqlRepository { get; private set; }

        [TestFixtureTearDown]
        public void ClassTearDown() {
            //Thread.Sleep(1);
            //Console.WriteLine("WaitAsync for Monitoring Connection Pooling...");
        }

        [Test]
        public void Can_Load_Current_Repository() {
            AdoRepository.Current.Should().Not.Be.Null();

            AdoRepository.Current.DbName
                .Should().Not.Be.Empty()
                .And.Be.EqualTo(AdoTool.DefaultDatabaseName);
        }

        #region << GetNamedQueryCommand >>

        [TestCase("Order, GetAll")]
        [TestCase("Employees, GetAll")]
        [TestCase("Invoices, GetAll")]
        public void Can_Create_Command_By_NamedQueryCommand_With_QueryKey(string queryKey) {
            NorthwindAdoRepository.QueryProvider.Should("NorthwindRepository.QueryProvider").Not.Be.Null();

            using(var cmd = NorthwindAdoRepository.GetNamedQueryCommand(queryKey))
            using(var dataTable = NorthwindAdoRepository.ExecuteDataTable(cmd)) {
                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
                Assert.IsTrue(dataTable.Rows.Count > 0);
            }

            using(var dataTable = NorthwindAdoRepository.ExecuteDataTable(queryKey)) {
                dataTable.Should().Not.Be.Null();
                Assert.IsFalse(dataTable.HasErrors);
                Assert.IsTrue(dataTable.Rows.Count > 0);
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

                var queryKey = string.Concat(section, ",", queryName);
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
            DataSet ds = null;

            using(var cmd = SqlRepository.GetCommand(GetOrderDetailsSql))
            using(ds = SqlRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }

            Thread.Sleep(1);

            using(ds =
                  SqlRepository
                      .ExecuteDataSetBySqlString(GetOrderByOrderDateAndFreightAndCustomerSql,
                                                 firstResult,
                                                 maxResults,
                                                 new AdoParameter("OrderDate", DateTime.Today, DbType.DateTime),
                                                 new AdoParameter("Freight", 2, DbType.Int32),
                                                 CustomerTestParameter)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                //Assert.Greater(ds.Tables[0].Rows.Count, 0);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSetByQuery(int firstResult, int maxResults) {
            DataSet ds = null;

            using(ds = SqlRepository.ExecuteDataSet(GetOrderDetailsSql, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 1);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSetInTransactionScope() {
            Action<int, int> loadMethod = (firstResult, maxResults) => {
                                              using(var cmd = SqlRepository.GetCommand(GetOrderDetailsSql))
                                              using(var ds = SqlRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                                                  Assert.AreEqual(ds.Tables.Count, 1);
                                                  Assert.IsFalse(ds.Tables[0].HasErrors);
                                                  Assert.Greater(ds.Tables[0].Rows.Count, 0);
                                                  //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
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

            using(var cmd = SqlRepository.GetCommand(GetOrderDetailsSql))
            using(ds = SqlRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }

            Thread.Sleep(1);

            using(ds = SqlRepository.ExecuteDataSetBySqlString(GetOrderDetailsSql, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSetByProcedure() {
            using(var ds = SqlRepository.ExecuteDataSetByProcedure(GetCustomerOrderHistorySql, CustomerTestParameter)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                // Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSet_MultiResultSet_BySqlString() {
            var sql = string.Concat(GetOrdersSql, ";", GetOrderDetailsSql);

            using(var cmd = SqlRepository.GetSqlStringCommand(sql))
            using(var ds = SqlRepository.ExecuteDataSet(cmd)) {
                Assert.AreEqual(2, ds.Tables.Count);
                ds.Tables.Cast<DataTable>().All(table => table.HasErrors == false).Should().Be.True();
            }
        }

        [Test]
        public void ExecuteDataSet_MultiResultSet_ByProcedure() {
            using(var cmd = SqlRepository.GetProcedureCommand("OrderAndOrderDetails")) {
                using(var ds = SqlRepository.ExecuteDataSet(cmd)) {
                    Assert.AreEqual(2, ds.Tables.Count);
                    ds.Tables.Cast<DataTable>().All(table => table.HasErrors == false).Should().Be.True();
                }
            }
        }

        #endregion

        #region << DataTable >>

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTable(int firstResult, int maxResults) {
            using(var cmd = SqlRepository.GetCommand(GetOrderDetailsSql))
            using(var table = SqlRepository.ExecuteDataTable(cmd, firstResult, maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableByQuery(int firstResult, int maxResults) {
            using(var table = SqlRepository.ExecuteDataTable(GetOrderDetailsSql, firstResult, maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableAndCopy(int firstResult, int maxResults) {
            // DataTable을 반환받아 다른 DataSet에 저장할 수 있는지 파악한다.

            using(var dataset = new DataSet())
            using(var cmd = SqlRepository.GetCommand(GetOrderDetailsSql))
            using(var table = SqlRepository.ExecuteDataTable(cmd, firstResult, maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);

                dataset.Tables.Add(table);
                Assert.AreEqual(1, dataset.Tables.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableBySqlString(int firstResult, int maxResults) {
            using(var table = SqlRepository.ExecuteDataTableBySqlString(GetOrderDetailsSql,
                                                                        firstResult,
                                                                        maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableByProcedure(int firstResult, int maxResults) {
            using(var table = SqlRepository.ExecuteDataTableByProcedure(GetCustomerOrderHistorySql,
                                                                        firstResult,
                                                                        maxResults,
                                                                        CustomerTestParameter)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);
            }
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        public void ExecuteDataTableToInstance(int firstResult, int maxResults) {
            using(var table = SqlRepository.ExecuteDataTableByProcedure(GetCustomerOrderHistorySql,
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

            using(var cmd = SqlRepository.GetSqlStringCommand(sql)) {
                var tables = SqlRepository.ExecuteDataTableAsList(cmd);
                Assert.AreEqual(2, tables.Count);
                tables.All(table => table.HasErrors == false).Should().Be.True();
            }
        }

        [Test]
        public void ExecuteDataTableAsList_ByProcedure() {
            using(var cmd = SqlRepository.GetProcedureCommand("OrderAndOrderDetails")) {
                var tables = SqlRepository.ExecuteDataTableAsList(cmd).ToList();

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
                using(var cmd = SqlRepository.GetCommand(GetOrderDetailsSql + @" WHERE Discount > @Discount"))
                using(var pagingTable = SqlRepository.ExecutePagingDataTable(cmd, pageIndex, pageSize, parameters)) {
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
                using(var pagingTable =
                    SqlRepository
                        .ExecutePagingDataTable(GetOrderDetailsSql + @" WHERE Discount > @Discount",
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
            var parameters = new AdoParameter[] { new AdoParameter("Discount", discount) };

            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex=[{0}], pageSize=[{1}])", pageIndex,
                                                 pageSize)))
            using(var pagingTable = SqlRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize, parameters)) {
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

            Console.WriteLine("Waiting for Verify Connection Pooling");
        }

        [TestCase("SELECT OD.*, (SELECT TOP 1 name FROM sysobjects) AS SYS_NAME FROM [Order Details] AS OD ORDER BY OrderID ", 0, 100)]
        [TestCase("SELECT OD.*, (SELECT TOP 1 name FROM sysobjects) AS SYS_NAME FROM [Order Details] AS OD ORDER BY OrderID ", 0, 100)]
        [TestCase("SELECT OD.*, (SELECT TOP 1 name FROM sysobjects) AS SYS_NAME FROM [Order Details] AS OD ORDER BY OrderID", 2, 100)]
        public void ExecutePagingDataTableBySelectSqlTest(string selectSql, int pageIndex, int pageSize) {
            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex=[{0}], pageSize=[{1}])", pageIndex,
                                                 pageSize)))
            using(var pagingTable = SqlRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize)) {
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

            Console.WriteLine("Waiting for Verify Connection Pooling");
        }

        #endregion

        #region << Execute Scalar >>

        [Test]
        public void ExecuteScalar() {
            SqlRepository
                .ExecuteScalarBySqlString("SELECT COUNT(*) FROM dbo.Orders")
                .AsInt(0)
                .Should().Be.GreaterThan(0);

            SqlRepository
                .ExecuteScalarBySqlString("SELECT COUNT(*) FROM Orders where OrderDate < @OrderDate and Freight < @Freight",
                                          new AdoParameter("OrderDate", DateTime.Today),
                                          new AdoParameter("Freight", 2))
                .AsInt(0)
                .Should().Be.GreaterThan(0);

            SqlRepository
                .ExecuteScalarBySqlString("SELECT TOP 1 ISNULL(OrderID,0) FROM Orders ORDER BY ShippedDate")
                .AsInt(0)
                .Should().Not.Be(0);

            SqlRepository
                .ExecuteScalarBySqlString("SELECT TOP 1 ISNULL(OrderID,0) FROM Orders Where ShipVia=0 ORDER BY ShippedDate")
                .AsInt(0)
                .Should().Be(0);
        }

        #endregion

        #region << Execute NonQuery >>

        [Test]
        public void ExecuteNonQuery() {
            var row = SqlRepository
                .ExecuteNonQueryBySqlString("DELETE FROM Employees where LastName=@LastName and FirstName=@FirstName",
                                            new AdoParameter("LastName", "Bae"),
                                            new AdoParameter("FirstName", "Sunghyouk"));

            Console.WriteLine("Row affected: " + row);
        }

        #endregion

        #region << ExecuteReader >>

        [Test]
        public void ExecuteReaderByQuery() {
            using(var reader = SqlRepository.ExecuteReaderBySqlString(GetOrderDetailsSql)) {
                Assert.IsTrue(reader.Count() > 0);
            }
        }

        [Test]
        public void ExecuteReaderByProcedure() {
            using(var reader = SqlRepository.ExecuteReaderByProcedure(GetCustomerOrderHistorySql, CustomerTestParameter)) {
                Assert.IsTrue(reader.Count() > 0);
            }
        }

        #endregion

        #region << Count Of DataReader >>

        [Test]
        public void CountOfDataReader() {
            Assert.AreEqual(5, SqlRepository.CountBySqlString("SELECT TOP 5 * FROM [Order Details]"));

            Assert.AreEqual(1, SqlRepository.CountBySqlString("SELECT TOP 1 * FROM Customers"));

            Assert.AreEqual(0, SqlRepository.CountBySqlString("SELECT TOP 0 * FROM Customers"));

            var count = SqlRepository.CountByProcedure(GetCustomerOrderHistorySql, CustomerTestParameter);

            Assert.IsTrue(count > 0);
        }

        #endregion

        #region << Exists >>

        [Test]
        public void ExistsByDataReader() {
            Assert.IsTrue(SqlRepository.ExistsBySqlString("SELECT TOP 1 * FROM Customers"));
            Assert.IsFalse(SqlRepository.ExistsBySqlString("SELECT TOP 0 * FROM Customers"));

            Assert.IsFalse(
                SqlRepository.ExistsBySqlString("SELECT * FROM Customers WHERE CustomerID=@CustomerID",
                                                new AdoParameter("CustomerID", "DEBOP68")));

            Assert.IsTrue(SqlRepository.ExistsBySqlString("SELECT TOP 1 * FROM [Order Details]"));
            Assert.IsTrue(SqlRepository.ExistsByProcedure(GetCustomerOrderHistorySql, CustomerTestParameter));
        }

        #endregion

        #region << ExecuteReader{TPersisntent} >>

        [TestCase(0, 1000)]
        [TestCase(1, 30)]
        [TestCase(0, 500)]
        [TestCase(3, 10)]
        public void ExecuteReaderToInstanceByNameMapping(int pageIndex, int pageSize) {
            using(var cmd = SqlRepository.GetCommand(GetCustomerSql)) {
                var customers = SqlRepository.ExecuteInstance<Customer>(TrimMapper, cmd, pageIndex, pageSize);

                customers.Count().Should().Be.GreaterThan(0);
                customers.All(c => c.CustomerID.IsNotWhiteSpace()).Should().Be.True();
                customers.All(c => c.CompanyName.IsNotWhiteSpace()).Should().Be.True();
                customers.All(c => c.ContactName.IsNotWhiteSpace()).Should().Be.True();
            }
        }

        [TestCase(0, 10)]
        [TestCase(3, 20)]
        [TestCase(5, 10)]
        public void ExecuteReaderWithPaging(int pageIndex, int pageSize) {
            using(var cmd = SqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT)) {
                var orderDetails = SqlRepository.ExecuteInstance<OrderDetail>(TrimMapper, cmd, pageIndex, pageSize);

                orderDetails.All(od => od.UnitPrice.GetValueOrDefault(0) > 0).Should().Be.True();
                orderDetails.All(od => od.Quantity.GetValueOrDefault(-1) >= 0).Should().Be.True();
                orderDetails.All(od => od.Discount.GetValueOrDefault(-1) >= 0F).Should().Be.True();
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


            using(var cmd = SqlRepository.GetCommand(GetCustomerOrderHistorySql)) {
                var orderHistories = SqlRepository.ExecuteInstance(@mapFunc, cmd, CustomerTestParameter);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());
            }

            Func<IDataReader, CustomerOrderHistory> @mapFunc2 = DataReaderToCustomerOrderHistory;
            using(var cmd = SqlRepository.GetCommand(GetCustomerOrderHistorySql)) {
                var orderHistories = SqlRepository.ExecuteInstance(@mapFunc2, cmd, CustomerTestParameter);

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
            var outputs = SqlRepository.ExecuteProcedure(GetCustomerOrderHistorySql, CustomerTestParameter);

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
            using(var reader = SqlRepository.ExecuteReaderByProcedure("CustOrderHist2", CustomerTestParameter)) {
                var orderHistories =
                    reader.Map<CustomerOrderHistory>(reader.Mapping(NameMappingUtil.CapitalizeMappingFunc('_', ' '))).ToList();

                Assert.IsTrue(orderHistories.Count > 0);
                Console.WriteLine("Order History: " + orderHistories.CollectionToString());
            }
        }

        [Test]
        public void PersistentObjectToDatabase() {
            try {
                var category = new Category { CategoryName = "Test", Description = "FluentUtil" };

                // delete exist category
                SqlRepository.ExecuteNonQueryBySqlString("DELETE FROM Categories where CategoryName = @CategoryName",
                                                         new AdoParameter("CategoryName", category.CategoryName));

                // insert
                var result = SqlRepository.ExecuteEntity("SaveOrUpdateCategory", category, CapitalizeMapper);

                category.CategoryId = result.AsInt(-1);
                Assert.AreNotEqual(-1, category.CategoryId);

                // update
                result = SqlRepository.ExecuteEntity("SaveOrUpdateCategory", category, CapitalizeMapper);
                Assert.AreNotEqual(0, result.AsInt());
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

                Assert.Greater(orderHistories.Count, 0);
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
            SqlRepository.ExecuteNonQueryBySqlString("DELETE FROM Categories where CategoryName = @CategoryName",
                                                     new AdoParameter("CategoryName", category.CategoryName));

            // insert
            var result = SqlRepository.ExecuteEntity("SaveOrUpdateCategory", category, nameMapper);

            category.CategoryId = result.AsInt(-1); //ConvertTool.DefValue(result, -1);
            Assert.AreNotEqual(-1, category.CategoryId);

            // update
            result = SqlRepository.ExecuteEntity("SaveOrUpdateCategory", category, nameMapper);
            Assert.AreNotEqual(0, result.AsInt());
        }

        #endregion

        [TestCase(4, 5)]
        [TestCase(2, 10)]
        public void ThreadTest(int repeat, int concurrent) {
            for(var i = 0; i < repeat; i++) {
                TestTool.RunTasks(concurrent,
                                  () => ExecuteDataSet(0, 0),
                                  () => ExecuteDataSetByQuery(0, 0),
                                  ExecuteDataSetInTransactionScope,
                                  () => ExecuteDataSetByPaging(0, 0),
                                  ExecuteDataSetByProcedure,
                                  ExecuteDataSet_MultiResultSet_BySqlString,
                                  ExecuteDataSet_MultiResultSet_ByProcedure,
                                  () => ExecuteDataTable(0, 0),
                                  () => ExecuteDataTableByQuery(0, 0),
                                  () => ExecuteDataTableAndCopy(0, 0),
                                  () => ExecuteDataTableBySqlString(0, 0),
                                  () => ExecuteDataTableByProcedure(0, 0),
                                  () => ExecuteDataTableToInstance(0, 0),
                                  ExecuteDataTableAsList_BySqlString,
                                  ExecuteDataTableAsList_ByProcedure,
                                  () => ExecutePagingDataTableTest(0, 10, 0.0),
                                  () => ExecutePagingDataTableByQueryTest(0, 10, 0.0),
                                  () => ExecutePagingDataTableBySelectSqlTest(0, 10, 0.0),
                                  ExecuteScalar,
                                  ExecuteNonQuery,
                                  ExecuteReaderByQuery,
                                  ExecuteReaderByProcedure,
                                  CountOfDataReader,
                                  ExistsByDataReader,
                                  () => ExecuteReaderToInstanceByNameMapping(0, 1000),
                                  () => ExecuteReaderWithPaging(0, 10),
                                  () => ExecuteReaderWithPagingByPersister(0, 10),
                                  ExecuteReaderToInstanceByConverter,
                                  ExecuteProcedure,
                                  DatabaseToPersistentObject,
                                  FluentByNameMapper_Load
                    );

                Thread.Sleep(100);
            }
        }
    }
}