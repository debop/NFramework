using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Threading;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.PostgreSql.Ado {
    [TestFixture]
    public class NpgsqlRepositoryImplFixture : NpgsqlAdoFixtureBase {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Repository
        /// </summary>
        public IPostgreSqlRepository PostgreSqlRepository {
            get { return NorthwindPostgreSqlRepository; }
        }

        [Test]
        public void AResolveUniRepositoryTest() {
            PostgreSqlRepository.Should().Not.Be.Null();
        }

        #region << GetNamedQueryCommand >>

        [TestCase("Order, GetAll")]
        [TestCase("Employees, GetAll")]
        [TestCase("Invoices, GetAll")]
        public void ExecuteDataTableByCommandFrom_GetNamedQueryCommand(string queryKey) {
            PostgreSqlRepository.QueryProvider.Should().Not.Be.Null();

            if(IsDebugEnabled)
                log.Debug("실행할 SQL 문장=");

            using(var cmd = PostgreSqlRepository.GetNamedQueryCommand(queryKey))
            using(var dataTable = PostgreSqlRepository.ExecuteDataTable(cmd)) {
                VerifyDataTableHasData(dataTable);
            }
        }

        [TestCase("Order, GetAll")]
        [TestCase("Employees, GetAll")]
        [TestCase("Invoices, GetAll")]
        public void ExecuteDataTableByQueryTest(string queryKey) {
            PostgreSqlRepository.QueryProvider.Should().Not.Be.Null();

            // 내부에서 GetNamedQueryCommand를 생성합니다.
            using(var dataTable = PostgreSqlRepository.ExecuteDataTable(queryKey)) {
                VerifyDataTableHasData(dataTable);
            }
        }

        [TestCase("Order", "GetAll")]
        [TestCase("Employees", "GetAll")]
        [TestCase("Invoices", "GetAll")]
        public void ExecuteDataTableByCommandFrom_GetNamedQueryCommand_Section_QueryName(string section, string queryName) {
            PostgreSqlRepository.QueryProvider.Should().Not.Be.Null();

            using(var cmd = PostgreSqlRepository.GetNamedQueryCommand(section, queryName))
            using(var dataTable = PostgreSqlRepository.ExecuteDataTable(cmd)) {
                VerifyDataTableHasData(dataTable);
            }
        }

        [TestCase("Order", "GetAll")]
        [TestCase("Employees", "GetAll")]
        [TestCase("Invoices", "GetAll")]
        public void ExecuteDataTableByQuery_Section_QueryName(string section, string queryName) {
            PostgreSqlRepository.QueryProvider.Should().Not.Be.Null();

            // 내부에서 GetNamedQueryCommand를 생성합니다.
            using(var dataTable = PostgreSqlRepository.ExecuteDataTable(section + "," + queryName)) {
                VerifyDataTableHasData(dataTable);
            }
        }

        #endregion

        #region << DataSet >>

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSetTest(int firstResult, int maxResults) {
            using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT))
            using(var ds = PostgreSqlRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                ds.Tables.Count.Should().Be(1);
                var dataTable = ds.Tables[0];
                VerifyDataTableHasData(dataTable);
            }
        }

        [TestCase(0, 0)]
        [TestCase(2, 10)]
        [TestCase(2, 0)]
        public void ExecuteDataSetBySqlStringTest(int firstResult, int maxResults) {
            using(var ds = PostgreSqlRepository.ExecuteDataSetBySqlString(SQL_ORDER_BY_ORDERDATE_AND_FREIGHT_AND_CUSTOMER_SELECT,
                                                                          firstResult,
                                                                          maxResults,
                                                                          new AdoParameter("OrderDate", DateTime.Today),
                                                                          new AdoParameter("Freight", 50),
                                                                          CustomerTestParameter)) {
                ds.Tables.Count.Should().Be(1);
                var dataTable = ds.Tables[0];
                VerifyDataTableHasData(dataTable);
            }
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSetByQueryTest(int firstResult, int maxResults) {
            DataSet ds = null;

            using(ds = PostgreSqlRepository.ExecuteDataSet(SQL_ORDER_DETAIL_SELECT, firstResult, maxResults)) {
                ds.Tables.Count.Should().Be(1);
                var dataTable = ds.Tables[0];
                VerifyDataTableHasData(dataTable);
            }
        }

        [Test]
        public void ExecuteDataSetInTransactionScope() {
            Action<int, int> loadMethod = (firstResult, maxResults) => {
                                              using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_SELECT))
                                              using(var ds = PostgreSqlRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                                                  ds.Tables.Count.Should().Be(1);
                                                  var dataTable = ds.Tables[0];
                                                  VerifyDataTableHasData(dataTable);
                                              }
                                          };

            AdoWith.TransactionScope(delegate {
                                         loadMethod(5, 10);
                                         loadMethod(5, 5);
                                         loadMethod(2, 2);
                                     },
                                     delegate {
                                         loadMethod(5, 10);
                                         loadMethod(5, 5);
                                         loadMethod(1, 2);
                                     },
                                     delegate {
                                         loadMethod(5, 10);
                                         loadMethod(5, 0);
                                         loadMethod(1, 0);
                                     });
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSetWithPaging(int firstResult, int maxResults) {
            using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT))
            using(var ds = PostgreSqlRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                ds.Tables.Count.Should().Be(1);
                var dataTable = ds.Tables[0];
                VerifyDataTableHasData(dataTable);
            }
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSetBySqlStringWithPaging(int firstResult, int maxResults) {
            using(var ds = PostgreSqlRepository.ExecuteDataSetBySqlString(SQL_ORDER_DETAIL_SELECT, firstResult, maxResults)) {
                ds.Tables.Count.Should().Be(1);
                var dataTable = ds.Tables[0];
                VerifyDataTableHasData(dataTable);
            }
        }

        [Test]
        public virtual void ExecuteDataSetByProcedure() {
            using(var ds = PostgreSqlRepository.ExecuteDataSet(SP_CUSTOMER_ORDER_HISTORY, CustomerTestParameter)) {
                ds.Tables.Count.Should().Be(1);
                var dataTable = ds.Tables[0];
                VerifyDataTableHasData(dataTable);
            }
        }

        [TestCase(0, 0)]
        [TestCase(1, 10)]
        [TestCase(2, 0)]
        public virtual void ExecuteDataSetByProcedureWithPaging(int firstResult, int maxResults) {
            using(
                var ds = PostgreSqlRepository.ExecuteDataSet(SP_CUSTOMER_ORDER_HISTORY, firstResult, maxResults, CustomerTestParameter)) {
                ds.Tables.Count.Should().Be(1);
                var dataTable = ds.Tables[0];
                VerifyDataTableHasData(dataTable);
            }
        }

        #endregion

        #region << DataTable >>

        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataTable(int firstResult, int maxResults) {
            using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT))
            using(var table = PostgreSqlRepository.ExecuteDataTable(cmd, firstResult, maxResults)) {
                VerifyDataTableHasData(table);
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataTableByQuery(int firstResult, int maxResults) {
            using(var table = PostgreSqlRepository.ExecuteDataTable(SQL_ORDER_DETAIL_SELECT, firstResult, maxResults)) {
                VerifyDataTableHasData(table);
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataTableAndCopy(int firstResult, int maxResults) {
            // DataTable을 반환받아 다른 DataSet에 저장할 수 있는지 테스트한다.

            using(var dataset = new DataSet())
            using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT))
            using(var table = PostgreSqlRepository.ExecuteDataTable(cmd, firstResult, maxResults)) {
                VerifyDataTableHasData(table);

                dataset.Tables.Add(table);
                Assert.AreEqual(1, dataset.Tables.Count);
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataTableBySqlString(int firstResult, int maxResults) {
            using(var table = PostgreSqlRepository.ExecuteDataTableBySqlString(SQL_ORDER_SELECT, firstResult, maxResults)) {
                VerifyDataTableHasData(table);
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(1, 10)]
        [TestCase(2, 0)]
        public virtual void ExecuteDataTableByProcedureTest(int firstResult, int maxResults) {
            using(var table = PostgreSqlRepository.ExecuteDataTable(SP_CUSTOMER_ORDER_HISTORY,
                                                                    firstResult,
                                                                    maxResults,
                                                                    CustomerTestParameter)) {
                VerifyDataTableHasData(table);
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(1, 10)]
        [TestCase(2, 0)]
        public virtual void ExecuteDataTableByProcedureToInstance(int firstResult, int maxResults) {
            using(var table = PostgreSqlRepository.ExecuteDataTable(SP_CUSTOMER_ORDER_HISTORY,
                                                                    firstResult,
                                                                    maxResults,
                                                                    CustomerTestParameter)) {
                VerifyDataTableHasData(table);

                var orderHistories = table.Map<CustomerOrderHistory>(); //AdoTool.ConvertAll<CustomerOrderHistory>(table);
                orderHistories.Count.Should().Be.GreaterThan(0);

                orderHistories = table.MapAsParallel<CustomerOrderHistory>().ToList();
                    //AdoTool.ConvertAll<CustomerOrderHistory>(table);
                orderHistories.Count.Should().Be.GreaterThan(0);
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
                using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT + @" WHERE Discount > @Discount"))
                using(var pagingTable = PostgreSqlRepository.ExecutePagingDataTable(cmd, pageIndex, pageSize, parameters)) {
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
                    PostgreSqlRepository
                        .ExecutePagingDataTable(SQL_ORDER_DETAIL_SELECT + @" WHERE Discount > @Discount",
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
            const string selectSql = SQL_ORDER_DETAIL_SELECT + @" WHERE Discount > :Discount";
            var parameters = new AdoParameter[] { new AdoParameter("Discount", discount) };

            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex=[{0}], pageSize=[{1}])", pageIndex,
                                                 pageSize)))
            using(var pagingTable = PostgreSqlRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize, parameters)) {
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

        [TestCase("SELECT OD.*, 'A' AS SYS_NAME FROM Orders OD ORDER BY OrderID ", 0, 10)]
        [TestCase("SELECT OD.*, 'B' AS SYS_NAME FROM Orders OD ORDER BY OrderID ", 1, 10)]
        [TestCase("SELECT OD.*, 'C' AS SYS_NAME FROM Orders OD ORDER BY OrderID ", 2, 10)]
        public void ExecutePagingDataTableBySelectSqlTest2(string selectSql, int pageIndex, int pageSize) {
            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex=[{0}], pageSize=[{1}])", pageIndex,
                                                 pageSize)))
            using(var pagingTable = PostgreSqlRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize)) {
                Assert.AreEqual(pageIndex, pagingTable.PageIndex);
                Assert.AreEqual(pageSize, pagingTable.PageSize);

                Assert.IsTrue(pagingTable.TotalPageCount > 0);
                Assert.IsTrue(pagingTable.TotalItemCount > 0);

                Assert.IsTrue(pagingTable.Table.Rows.Count > 0);
                Assert.IsTrue(pagingTable.Table.Rows.Count <= pageSize);

                var table = pagingTable.Table;
                VerifyDataTableHasData(pagingTable.Table);
            }
        }

        #endregion

        #region << Execute Scalar >>

        [Test]
        public void ExecuteScalar() {
            PostgreSqlRepository
                .ExecuteScalarBySqlString(SQL_CUSTOMER_COUNT)
                .AsInt(0)
                .Should().Be.GreaterThan(0);

            PostgreSqlRepository
                .ExecuteScalarBySqlString("SELECT COUNT(*) FROM Orders where OrderDate < :OrderDate and Freight < :Freight",
                                          new AdoParameter("OrderDate", DateTime.Today),
                                          new AdoParameter("Freight", 50))
                .AsInt(0)
                .Should().Be.GreaterThan(0);

            //UniRepository
            //    .ExecuteScalarBySqlString("SELECT ISNULL(OrderID,0) FROM Orders ORDER BY ShippedDate")
            //    .AsInt(0)
            //    .Should().Not.Be(0);

            //UniRepository
            //    .ExecuteScalarBySqlString("SELECT TOP 1 ISNULL(OrderID,0) FROM Orders Where ShipVia=0 ORDER BY ShippedDate")
            //    .AsInt(0)
            //    .Should().Be(0);
        }

        #endregion

        #region << Execute NonQuery >>

        [Test]
        public void ExecuteNonQuery() {
            var row = PostgreSqlRepository
                .ExecuteNonQueryBySqlString("DELETE FROM Employees where LastName=:LastName and FirstName=:FirstName",
                                            new AdoParameter("LastName", "Bae"),
                                            new AdoParameter("FirstName", "Sunghyouk"));

            if(IsDebugEnabled)
                log.Debug("Row Affected=[{0}]", row);
        }

        #endregion

        #region << ExecuteReader >>

        [Test]
        public void ExecuteReaderByQuery() {
            using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT))
            using(var reader = PostgreSqlRepository.ExecuteReader(cmd)) {
                reader.Count().Should().Be.GreaterThan(0);
            }
        }

        [Test]
        public virtual void ExecuteReaderByProcedure() {
            using(var cmd = PostgreSqlRepository.GetCommand(SP_CUSTOMER_ORDER_HISTORY, CustomerTestParameter))
            using(var reader = PostgreSqlRepository.ExecuteReader(cmd)) {
                reader.Count().Should().Be.GreaterThan(0);
            }
        }

        #endregion

        #region << Count Of DataReader >>

        [Test]
        public void CountOfDataReader() {
            PostgreSqlRepository.CountBySqlString("SELECT * FROM OrderDetails").Should().Be.GreaterThan(0);
            PostgreSqlRepository.CountBySqlString("SELECT * FROM Customers").Should().Be.GreaterThan(0);
            PostgreSqlRepository.CountBySqlString("SELECT * FROM Customers WHERE CustomerID IS NULL").Should().Be(0);

            PostgreSqlRepository.Count(SP_CUSTOMER_ORDER_HISTORY, CustomerTestParameter).Should().Be.GreaterThan(0);
        }

        #endregion

        #region << Exists >>

        [Test]
        public void ExistsByDataReader() {
            PostgreSqlRepository.ExistsBySqlString("SELECT * FROM Customers").Should().Be.True();
            PostgreSqlRepository.ExistsBySqlString("SELECT * FROM Customers WHERE CustomerID IS NULL").Should().Be.False();

            PostgreSqlRepository.ExistsBySqlString("SELECT * FROM Customers WHERE CustomerID = :CustomerID",
                                                   new AdoParameter("CustomerID", "DEBOP68"))
                .Should().Be.False();

            PostgreSqlRepository.ExistsBySqlString("SELECT * FROM OrderDetails").Should().Be.True();

            PostgreSqlRepository.Exists(SP_CUSTOMER_ORDER_HISTORY, CustomerTestParameter).Should().Be.True();
        }

        #endregion

        #region << ExecuteReader{TPersisntent} >>

        [TestCase(0, 1000)]
        [TestCase(1, 30)]
        [TestCase(0, 500)]
        [TestCase(3, 10)]
        public void ExecuteReaderToInstanceByNameMapping(int pageIndex, int pageSize) {
            using(var cmd = PostgreSqlRepository.GetCommand(SQL_CUSTOMER_SELECT)) {
                var customers = PostgreSqlRepository.ExecuteInstance<Customer>(TrimMapper, cmd, pageIndex, pageSize);

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
            using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT)) {
                var orderDetails = PostgreSqlRepository.ExecuteInstance<OrderDetail>(TrimMapper, cmd, pageIndex, pageSize);

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

            using(var cmd = PostgreSqlRepository.GetCommand(SQL_ORDER_DETAIL_SELECT)) {
                var orderDetails = PostgreSqlRepository.ExecuteInstance<OrderDetail>(persister, cmd, pageIndex, pageSize);

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
        public virtual void ExecuteReaderToInstanceByConverter() {
            Func<IDataReader, CustomerOrderHistory> @mapFunc
                = delegate(IDataReader dr) {
                      var reader = dr;
                      return new CustomerOrderHistory
                             {
                                 ProductName = reader.AsString("ProductName"),
                                 Total = reader.AsInt32Nullable("Total")
                             };
                  };

            Func<IDataReader, CustomerOrderHistory> @mapFunc2 = DataReaderToCustomerOrderHistory;

            using(var cmd = PostgreSqlRepository.GetCommand(SP_CUSTOMER_ORDER_HISTORY)) {
                var orderHistories = PostgreSqlRepository.ExecuteInstance(@mapFunc, cmd, CustomerTestParameter);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());
            }
            using(var cmd = PostgreSqlRepository.GetCommand(SP_CUSTOMER_ORDER_HISTORY)) {
                var orderHistories = PostgreSqlRepository.ExecuteInstance(@mapFunc2, cmd, CustomerTestParameter);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());
            }
        }

        #endregion

        #region << Fluent By INameMapper >>

        [Test]
        public void FluentByNameMapper_Load() {

            INameMapper nameMapper = new CapitalizeNameMapper();

            using(var cmd = AdoRepository.GetCommand(SP_CUSTOMER_ORDER_HISTORY)) {
                var orderHistories = AdoRepository.ExecuteInstance<CustomerOrderHistory>(nameMapper, cmd, CustomerTestParameter);

                Assert.Greater(orderHistories.Count, 0);
                CollectionAssert.AllItemsAreNotNull((ICollection)orderHistories);
                CollectionAssert.AllItemsAreInstancesOfType((ICollection)orderHistories, typeof(CustomerOrderHistory));
            }
        }

        #endregion

        [TestCase(4, 5)]
        [TestCase(2, 10)]
        public void ThreadTest(int repeat, int concurrent) {
            for(int i = 0; i < repeat; i++) {
                TestTool.RunTasks(concurrent,
                                  () => ExecuteDataSetTest(0, 0),
                                  () => ExecuteDataSetByQueryTest(0, 0),
                                  ExecuteDataSetInTransactionScope,
                                  () => ExecuteDataSetBySqlStringWithPaging(0, 0),
                                  ExecuteDataSetByProcedure,
                                  () => ExecuteDataTable(0, 0),
                                  () => ExecuteDataTableByQuery(0, 0),
                                  () => ExecuteDataTableAndCopy(0, 0),
                                  () => ExecuteDataTableBySqlString(0, 0),
                                  () => ExecuteDataTableByProcedureTest(0, 0),
                                  () => ExecuteDataTableByProcedureToInstance(0, 0),
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
                                  FluentByNameMapper_Load
                    );

                Thread.Sleep(100);
            }
        }

        protected static void VerifyDataTableHasData(DataTable dataTable) {
            dataTable.Should().Not.Be.Null();
            dataTable.HasErrors.Should().Be.False();
            dataTable.Rows.Count.Should().Be.GreaterThan(0);
            dataTable.Columns.Count.Should().Be.GreaterThan(0);
        }
    }
}