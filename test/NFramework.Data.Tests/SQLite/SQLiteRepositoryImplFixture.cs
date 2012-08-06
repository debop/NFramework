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

namespace NSoft.NFramework.Data.SQLite {
    public class SQLiteRepositoryImplFixture : SQLiteAdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string GetCustomerOrderHistorySql =
            @"select P.ProductName AS ProductName, SUM( OD.UnitPrice * OD.Quantity * (1.0-OD.Discount)) AS Total
from Products P inner join OrderDetails OD on (P.ProductID = OD.ProductID)
	 inner join Orders O on (O.OrderID = OD.OrderID)
where O.CustomerID = @CustomerID 
group by P.ProductID";

        private const string GetCustomerSql = @"SELECT * FROM Customers";
        private const string GetOrdersSql = @"SELECT * FROM Orders";
        private const string GetOrderDetailsSql = @"SELECT * FROM OrderDetails ";

        private const string GetOrderByOrderDateAndFreightSql =
            @"SELECT * FROM Orders WHERE OrderDate < @OrderDate AND Freight < @Freight";

        private const string GetOrderByOrderDateAndFreightAndCustomerSql =
            @"SELECT * From Orders WHERE OrderDate < @OrderDate and Freight < @Freight and CustomerID = @CustomerID";

        public static ISQLiteRepository SQLiteRepository {
            get { return NorthwindRepository; }
        }

        [Test]
        public void Can_Load_Current_Repository() {
            SQLiteRepository.Should().Not.Be.Null();
            SQLiteRepository.DbName.Should().Not.Be.Empty();
        }

        #region << GetNamedQueryCommand >>

        [TestCase("Order, GetAll")]
        [TestCase("Employees, GetAll")]
        [TestCase("Customer, GetAll")]
        public void Can_Create_Command_By_NamedQueryCommand_With_QueryKey(string queryKey) {
            NorthwindRepository.QueryProvider.Should().Not.Be.Null();

            using(var cmd = NorthwindRepository.GetNamedQueryCommand(queryKey))
            using(var dataTable = NorthwindRepository.ExecuteDataTable(cmd)) {
                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
                Assert.IsTrue(dataTable.Rows.Count > 0);
            }

            using(var dataTable = NorthwindRepository.ExecuteDataTable(queryKey)) {
                dataTable.Should().Not.Be.Null();
                Assert.IsFalse(dataTable.HasErrors);
                Assert.IsTrue(dataTable.Rows.Count > 0);
            }
        }

        [TestCase("Order", "GetAll")]
        [TestCase("Employees", "GetAll")]
        [TestCase("Customer", "GetAll")]
        public void Can_Create_Command_By_NamedQueryCommand_With_Section_And_QueryName(string section, string queryName) {
            if(NorthwindRepository.QueryProvider != null) {
                using(var cmd = NorthwindRepository.GetNamedQueryCommand(section, queryName))
                using(var dataTable = NorthwindRepository.ExecuteDataTable(cmd)) {
                    Assert.IsNotNull(dataTable);
                    Assert.IsFalse(dataTable.HasErrors);
                    Assert.IsTrue(dataTable.Rows.Count > 0);
                }

                var queryKey = string.Concat(section, ",", queryName);
                using(var dataTable = NorthwindRepository.ExecuteDataTable(queryKey)) {
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

            using(var cmd = SQLiteRepository.GetCommand(GetOrderDetailsSql))
            using(ds = SQLiteRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }

            Thread.Sleep(1);

            using(ds =
                  SQLiteRepository
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

            using(ds = SQLiteRepository.ExecuteDataSet(GetOrderDetailsSql, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 1);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSetInTransactionScope() {
            Action<int, int> loadMethod = (firstResult, maxResults) => {
                                              using(var cmd = SQLiteRepository.GetCommand(GetOrderDetailsSql))
                                              using(var ds = SQLiteRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
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

            using(var cmd = SQLiteRepository.GetCommand(GetOrderDetailsSql))
            using(ds = SQLiteRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }

            Thread.Sleep(1);

            using(ds = SQLiteRepository.ExecuteDataSetBySqlString(GetOrderDetailsSql, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                //Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSetBySqlString() {
            using(var ds = SQLiteRepository.ExecuteDataSetBySqlString(GetCustomerOrderHistorySql, CustomerTestParameter)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
                // Console.WriteLine("RowCount: " + ds.Tables[0].Rows.Count);
            }
        }

        [Test]
        public void ExecuteDataSet_MultiResultSet_BySqlString() {
            var sql = string.Concat(GetOrdersSql, ";", GetOrderDetailsSql);

            using(var cmd = SQLiteRepository.GetSqlStringCommand(sql))
            using(var ds = SQLiteRepository.ExecuteDataSet(cmd)) {
                Assert.AreEqual(2, ds.Tables.Count);
                ds.Tables.Cast<DataTable>().All(table => table.HasErrors == false).Should().Be.True();
            }
        }

        #endregion

        #region << DataTable >>

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTable(int firstResult, int maxResults) {
            using(var cmd = SQLiteRepository.GetCommand(GetOrderDetailsSql))
            using(var table = SQLiteRepository.ExecuteDataTable(cmd, firstResult, maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableByQuery(int firstResult, int maxResults) {
            using(var table = SQLiteRepository.ExecuteDataTable(GetOrderDetailsSql, firstResult, maxResults)) {
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
            using(var cmd = SQLiteRepository.GetCommand(GetOrderDetailsSql))
            using(var table = SQLiteRepository.ExecuteDataTable(cmd, firstResult, maxResults)) {
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
            using(var table = SQLiteRepository.ExecuteDataTableBySqlString(GetOrderDetailsSql,
                                                                           firstResult,
                                                                           maxResults)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);
            }
        }

        [TestCase(5, 10)]
        [TestCase(0, 0)]
        public void ExecuteDataTableBySqlString2(int firstResult, int maxResults) {
            using(var table = SQLiteRepository.ExecuteDataTableBySqlString(GetCustomerOrderHistorySql,
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
            using(var table = SQLiteRepository.ExecuteDataTableBySqlString(GetCustomerOrderHistorySql,
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

            using(var cmd = SQLiteRepository.GetSqlStringCommand(sql)) {
                var tables = SQLiteRepository.ExecuteDataTableAsList(cmd);
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

            using(new OperationTimer(string.Format("ExecutePagingDataTable(pageIndex={0}, pageSize={1})", pageIndex, pageSize), false)) {
                using(var cmd = SQLiteRepository.GetCommand(GetOrderDetailsSql + @" WHERE Discount > @Discount"))
                using(var pagingTable = SQLiteRepository.ExecutePagingDataTable(cmd, pageIndex, pageSize, parameters)) {
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

            using(new OperationTimer(string.Format("ExecutePagingDataTableByQuery(pageIndex={0}, pageSize={1})", pageIndex, pageSize))) {
                using(var pagingTable =
                    SQLiteRepository
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
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex={0}, pageSize={1})", pageIndex, pageSize))
                )
            using(var pagingTable = SQLiteRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize, parameters)) {
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

        [TestCase("SELECT OD.*, (SELECT LastName FROM Employees limit 1) AS SYS_NAME FROM OrderDetails AS OD ORDER BY OrderID ", 0, 100)
        ]
        [TestCase("SELECT OD.*, (SELECT LastName FROM Employees limit 1) AS SYS_NAME FROM OrderDetails AS OD ORDER BY OrderID ", 0, 100)
        ]
        [TestCase("SELECT OD.*, (SELECT LastName FROM Employees limit 1) AS SYS_NAME FROM OrderDetails AS OD ORDER BY OrderID", 2, 100)]
        public void ExecutePagingDataTableBySelectSqlTest(string selectSql, int pageIndex, int pageSize) {
            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex={0}, pageSize={1})", pageIndex, pageSize))
                )
            using(var pagingTable = SQLiteRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize)) {
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
            SQLiteRepository.ExecuteScalarBySqlString("SELECT COUNT(*) FROM Orders").AsInt().Should().Be.GreaterThan(0);

            SQLiteRepository
                .ExecuteScalarBySqlString("SELECT COUNT(*) FROM Orders where OrderDate < @OrderDate and Freight < @Freight",
                                          new AdoParameter("OrderDate", DateTime.Today),
                                          new AdoParameter("Freight", 2))
                .AsInt()
                .Should().Be.GreaterThan(0);

            SQLiteRepository
                .ExecuteScalarBySqlString("SELECT OrderID FROM Orders ORDER BY ShippedDate LIMIT 1")
                .AsInt(0)
                .Should().Be.GreaterThan(0);


            // Console.WriteLine("OrderId = {0}, Type={1}", orderId, orderId.GetType());


            SQLiteRepository
                .ExecuteScalarBySqlString("SELECT OrderID FROM Orders Where Freight=0 ORDER BY ShippedDate limit 1 ")
                .AsInt(0)
                .Should().Be(0);

            //Console.WriteLine("OrderId = {0}, Type={1}", orderId, orderId.GetType());
        }

        #endregion

        #region << Execute NonQuery >>

        [Test]
        public void ExecuteNonQuery() {
            var row = SQLiteRepository
                .ExecuteNonQueryBySqlString("DELETE FROM Employees where LastName=@LastName and FirstName=@FirstName",
                                            new AdoParameter("LastName", "Bae", DbType.String, 255),
                                            new AdoParameter("FirstName", "Sunghyouk", DbType.String, 255));

            Console.WriteLine("Row affected: " + row);
        }

        #endregion

        #region << ExecuteReader >>

        [Test]
        public void ExecuteReaderByQuery() {
            using(var reader = SQLiteRepository.ExecuteReaderBySqlString(GetOrderDetailsSql)) {
                Assert.IsTrue(reader.Count() > 0);
            }
        }

        [Test]
        public void ExecuteReaderBySqlString() {
            using(var reader = SQLiteRepository.ExecuteReaderBySqlString(GetCustomerOrderHistorySql, CustomerTestParameter)) {
                Assert.IsTrue(reader.Count() > 0);
            }
        }

        #endregion

        #region << Count Of DataReader >>

        [Test]
        public void CountOfDataReader() {
            Assert.AreEqual(5, SQLiteRepository.CountBySqlString("SELECT * FROM OrderDetails limit 5"));

            Assert.AreEqual(1, SQLiteRepository.CountBySqlString("SELECT * FROM Customers limit 1"));

            Assert.AreEqual(0, SQLiteRepository.CountBySqlString("SELECT * FROM Customers limit 0"));
        }

        #endregion

        #region << Exists >>

        [Test]
        public void ExistsByDataReader() {
            Assert.IsTrue(SQLiteRepository.ExistsBySqlString("SELECT * FROM Customers limit 1"));
            Assert.IsFalse(SQLiteRepository.ExistsBySqlString("SELECT * FROM Customers limit 0"));

            Assert.IsFalse(
                SQLiteRepository.ExistsBySqlString("SELECT * FROM Customers WHERE CustomerID=@CustomerID",
                                                   new AdoParameter("CustomerID", "DEBOP68")));

            Assert.IsTrue(SQLiteRepository.ExistsBySqlString("SELECT * FROM OrderDetails limit 1"));
        }

        #endregion

        #region << ExecuteReader{TPersisntent} >>

        [TestCase(0, 1000)]
        [TestCase(1, 30)]
        [TestCase(0, 500)]
        [TestCase(3, 10)]
        public void ExecuteReaderToInstanceByNameMapping(int pageIndex, int pageSize) {
            using(var cmd = SQLiteRepository.GetCommand(GetCustomerSql)) {
                var customers = SQLiteRepository.ExecuteInstance<Customer>(TrimMapper, cmd, pageIndex, pageSize);

                Assert.Greater(customers.Count(), 0);
                Assert.IsTrue(customers.All(c => c.CustomerID.IsNotWhiteSpace()));
                Assert.IsTrue(customers.All(c => c.CompanyName.IsNotWhiteSpace()));
                Assert.IsTrue(customers.All(c => c.ContactName.IsNotWhiteSpace()));
            }
        }

        [TestCase(0, 10)]
        [TestCase(3, 20)]
        [TestCase(5, 10)]
        public void ExecuteReaderWithPaging(int pageIndex, int pageSize) {
            using(var cmd = SQLiteRepository.GetCommand(GetOrderDetailsSql)) {
                var orderDetails = SQLiteRepository.ExecuteInstance<OrderDetail>(TrimMapper, cmd, pageIndex, pageSize);

                Assert.IsTrue(orderDetails.All(od => od.UnitPrice.GetValueOrDefault(0) > 0));
                Assert.IsTrue(orderDetails.All(od => od.Quantity.GetValueOrDefault(-1) >= 0));
                Assert.IsTrue(orderDetails.All(od => od.Discount.GetValueOrDefault(-1) >= 0F));
            }
        }

        [TestCase(0, 10)]
        [TestCase(3, 20)]
        [TestCase(5, 10)]
        public void ExecuteReaderWithPagingByPersister(int pageIndex, int pageSize) {
            var persister = new TrimReaderPersister<OrderDetail>();

            using(var cmd = NorthwindRepository.GetCommand(GetOrderDetailsSql)) {
                var orderDetails = NorthwindRepository.ExecuteInstance<OrderDetail>(persister, cmd, pageIndex, pageSize);

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

            Func<IDataReader, CustomerOrderHistory> @mapFunc2 = DataReaderToCustomerOrderHistory;

            using(var cmd = SQLiteRepository.GetCommand(GetCustomerOrderHistorySql)) {
                var orderHistories = SQLiteRepository.ExecuteInstance(@mapFunc, cmd, CustomerTestParameter);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());
            }
            using(var cmd = SQLiteRepository.GetCommand(GetCustomerOrderHistorySql)) {
                var orderHistories = SQLiteRepository.ExecuteInstance(@mapFunc2, cmd, CustomerTestParameter);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());
            }
        }

        #endregion

        #region << Fluent >>

        /// <summary>
        /// Stored Procedure로부터 DataReader나 DataTable을 얻고, 그 정보로 Persistent Object를 빌드한다.
        /// </summary>
        [Test]
        public void DatabaseToPersistentObject() {
            // CustOrderHist2 는 컬럼명만 PROJECT_NAME, TOTAL 로 변경한 것이다.
            using(var reader = SQLiteRepository.ExecuteReaderBySqlString(GetCustomerOrderHistorySql, CustomerTestParameter)) {
                var orderHistories =
                    reader.Map<CustomerOrderHistory>(reader.Mapping(NameMappingUtil.CapitalizeMappingFunc('_', ' '))).ToList();

                Assert.IsTrue(orderHistories.Count > 0);
                Console.WriteLine("Order History: " + orderHistories.CollectionToString());
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void PersistentObjectToDatabase() {
            var category = new Category { CategoryName = "Test", Description = "FluentUtil" };

            // delete exist category
            SQLiteRepository.ExecuteNonQueryBySqlString("DELETE FROM Categories where CategoryName = @CategoryName",
                                                        new AdoParameter("CategoryName", category.CategoryName, DbType.String, 255));

            // insert
            var result = SQLiteRepository.ExecuteEntity("SaveOrUpdateCategory", category, CapitalizeMapper);

            category.CategoryId = result.AsInt(-1);
            Assert.AreNotEqual(-1, category.CategoryId);

            // update
            result = SQLiteRepository.ExecuteEntity("SaveOrUpdateCategory", category, CapitalizeMapper);
            Assert.AreNotEqual(0, result.AsInt());
        }

        #endregion

        #region << Fluent By INameMapper >>

        [Test]
        public void FluentByNameMapper_Load() {
            INameMapper nameMapper = new CapitalizeNameMapper();

            using(var cmd = SQLiteRepository.GetCommand(GetCustomerOrderHistorySql)) {
                var orderHistories = SQLiteRepository.ExecuteInstance<CustomerOrderHistory>(nameMapper, cmd, CustomerTestParameter);

                Assert.Greater(orderHistories.Count, 0);
                CollectionAssert.AllItemsAreNotNull((ICollection)orderHistories);
                CollectionAssert.AllItemsAreInstancesOfType((ICollection)orderHistories, typeof(CustomerOrderHistory));
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void FluentByNameMapper_Save() {
            INameMapper nameMapper = new CapitalizeNameMapper();

            var category = new Category
                           {
                               CategoryName = "Test",
                               Description = "FluentUtil"
                           };

            // delete exist category
            SQLiteRepository.ExecuteNonQueryBySqlString("DELETE FROM Categories where CategoryName = @CategoryName",
                                                        new AdoParameter("CategoryName", category.CategoryName, DbType.String, 255));

            // insert
            var result = SQLiteRepository.ExecuteEntity("SaveOrUpdateCategory", category, nameMapper);

            category.CategoryId = result.AsInt(-1); //ConvertTool.DefValue(result, -1);
            Assert.AreNotEqual(-1, category.CategoryId);

            // update
            result = SQLiteRepository.ExecuteEntity("SaveOrUpdateCategory", category, nameMapper);
            Assert.AreNotEqual(0, result.AsInt());
        }

        #endregion

        [TestCase(2, 10)]
        [TestCase(5, 4)]
        public void ThreadTest(int repeat, int concurrent) {
            for(var i = 0; i < repeat; i++) {
                TestTool.RunTasks(concurrent,
                                  () => ExecuteDataSet(0, 0),
                                  () => ExecuteDataSetByQuery(0, 0),
                                  ExecuteDataSetInTransactionScope,
                                  () => ExecuteDataSetByPaging(0, 0),
                                  ExecuteDataSet_MultiResultSet_BySqlString,
                                  () => ExecuteDataTable(0, 0),
                                  () => ExecuteDataTableByQuery(0, 0),
                                  () => ExecuteDataTableAndCopy(0, 0),
                                  () => ExecuteDataTableBySqlString(0, 0),
                                  () => ExecuteDataTableToInstance(0, 0),
                                  ExecuteDataTableAsList_BySqlString,
                                  () => ExecutePagingDataTableTest(0, 10, 0.0),
                                  () => ExecutePagingDataTableByQueryTest(0, 10, 0.0),
                                  () => ExecutePagingDataTableBySelectSqlTest(0, 10, 0.0),
                                  ExecuteScalar,
                                  ExecuteNonQuery,
                                  ExecuteReaderByQuery,
                                  CountOfDataReader,
                                  ExistsByDataReader,
                                  () => ExecuteReaderToInstanceByNameMapping(0, 1000),
                                  () => ExecuteReaderWithPaging(0, 10),
                                  () => ExecuteReaderWithPagingByPersister(0, 10),
                                  ExecuteReaderToInstanceByConverter,
                                  DatabaseToPersistentObject,
                                  FluentByNameMapper_Load
                    );

                Thread.Sleep(100);
            }
        }
    }
}