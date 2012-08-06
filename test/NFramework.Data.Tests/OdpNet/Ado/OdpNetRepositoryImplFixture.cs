using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using Oracle.DataAccess.Client;
using SharpTestsEx;

namespace NSoft.NFramework.Data.OdpNet.Ado {
    [TestFixture]
    public class OdpNetRepositoryImplFixture : OdpNetFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static IOdpNetRepository OdpRepository {
            get { return DefaultOracleRepository; }
        }

        //! HINT: REF CURSOR 를 사용하여 Procedure에서 ResultSet을 반환합니다.
        //! HINT: http://www.oradev.com/ref_cursor.jsp

        #region << Create >>

        [Test]
        public void CreateOracleClientTest() {
            var conn = new OracleConnection();
        }

        [Test]
        public void CreateTest() {
            var repository = new OdpNetRepositoryImpl();
            repository.Should().Not.Be.Null();
            repository.Db.Should().Not.Be.Null();

            repository = new OdpNetRepositoryImpl(AdoTool.DefaultDatabaseName);
            repository.Should().Not.Be.Null();
            repository.Db.Should().Not.Be.Null();
        }

        [Test]
        public void ConnectionTest() {
            var repository = new OdpNetRepositoryImpl();
            repository.Should().Not.Be.Null();
            repository.Db.Should().Not.Be.Null();

            var tableCount = repository.ExecuteScalar("SELECT COUNT(*) FROM TAB").AsInt();
            tableCount.Should().Be.GreaterThan(0);
        }

        #endregion

        #region << GetNamedQueryCommand >>

        [TestCase("Employee, GetAll")]
        [TestCase("State, GetAll")]
        [TestCase("Customer, GetAll")]
        public void Can_Create_Command_By_NamedQueryCommand_With_QueryKey(string queryKey) {
            if(OdpRepository.QueryProvider != null) {
                using(var cmd = OdpRepository.GetNamedQueryCommand(queryKey))
                using(var dataTable = OdpRepository.ExecuteDataTable(cmd)) {
                    Assert.IsNotNull(dataTable);
                    Assert.IsFalse(dataTable.HasErrors);
                    Assert.IsTrue(dataTable.Rows.Count > 0);
                }

                Thread.Sleep(1);

                using(var dataTable = OdpRepository.ExecuteDataTable(queryKey)) {
                    Assert.IsNotNull(dataTable);
                    Assert.IsFalse(dataTable.HasErrors);
                    Assert.IsTrue(dataTable.Rows.Count > 0);
                }
            }
        }

        [TestCase("Employee", "GetAll")]
        [TestCase("State", "GetAll")]
        [TestCase("Customer", "GetAll")]
        public void Can_Create_Command_By_NamedQueryCommand_With_Section_And_QueryName(string section, string queryName) {
            if(OdpRepository.QueryProvider != null) {
                using(var cmd = OdpRepository.GetNamedQueryCommand(section, queryName))
                using(var dataTable = OdpRepository.ExecuteDataTable(cmd)) {
                    Assert.IsNotNull(dataTable);
                    Assert.IsFalse(dataTable.HasErrors);
                    Assert.IsTrue(dataTable.Rows.Count > 0);
                }

                Thread.Sleep(1);

                var queryKey = string.Concat(section, ",", queryName);
                using(var dataTable = OdpRepository.ExecuteDataTable(queryKey)) {
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
            using(var cmd = OdpRepository.GetCommand(SelectEmp))
            using(var ds = OdpRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
            }

            Thread.Sleep(1);

            using(var cmd = OdpRepository.GetCommand(SelectDemoCustomers))
            using(var ds = OdpRepository.ExecuteDataSet(cmd, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
            }
        }

        [TestCase(0, 0)]
        [TestCase(5, 10)]
        [TestCase(5, 0)]
        public void ExecuteDataSetByQueryDirectly(int firstResult, int maxResults) {
            using(var ds = OdpRepository.ExecuteDataSet(SelectEmp, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
            }

            Thread.Sleep(1);

            using(var ds = OdpRepository.ExecuteDataSet(SelectDemoCustomers, firstResult, maxResults)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
            }
        }

        [Test]
        public void ExecuteDataSetInTransactionScope() {
            Action<int, int> @loadMethod =
                (firstResult, maxResults) => {
                    using(var ds = OdpRepository.ExecuteDataSet(SelectDemoCustomers,
                                                                firstResult, maxResults)) {
                        Assert.AreEqual(ds.Tables.Count, 1);
                        Assert.IsFalse(ds.Tables[0].HasErrors);
                        Assert.Greater(ds.Tables[0].Rows.Count, 0);
                    }
                };

            AdoWith.TransactionScope(delegate {
                                         @loadMethod(5, 10);
                                         @loadMethod(5, 5);
                                         @loadMethod(1, 1);
                                     },
                                     delegate {
                                         @loadMethod(5, 10);
                                         @loadMethod(5, 5);
                                         @loadMethod(1, 1);
                                     },
                                     delegate {
                                         @loadMethod(5, 10);
                                         @loadMethod(5, 5);
                                         @loadMethod(1, 1);
                                     });
        }

        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(1, 0)]
        [TestCase(1, 10)]
        public void ExecuteDataSetByProcedure(int firstResult, int maxResults) {
            using(var dataSet =
                OdpRepository
                    .ExecuteDataSetByProcedure(SP_GET_EMPLOYEES_BY_DEPT,
                                               firstResult,
                                               maxResults,
                                               new AdoParameter("P_DEPTNO", 30,
                                                                DbType.Decimal))) {
                Assert.IsNotNull(dataSet);
                Assert.AreEqual(1, dataSet.Tables.Count);
                var table = dataSet.Tables[0];
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);
            }
        }

        #endregion

        #region << DataTable >>

        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(1, 0)]
        [TestCase(1, 10)]
        public void ExecuteDataTableByProcedure(int firstResult, int maxResults) {
            using(var table = OdpRepository.ExecuteDataTableByProcedure(SP_GET_EMPLOYEES_BY_DEPT,
                                                                        firstResult,
                                                                        maxResults,
                                                                        new AdoParameter("P_DEPTNO", 30,
                                                                                         DbType.Decimal))) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 0);
                Console.WriteLine("RowCount: " + table.Rows.Count);
            }
        }

        #endregion

        #region << Execute Paging DataTable >>

        /// <summary>
        /// 단일 QUERY 문으로 PAGING을 수행합니다.
        /// </summary>
        [TestCase(0, 5, 100.0)]
        [TestCase(2, 2, 100.0)]
        [TestCase(3, 3, 150.0)]
        [TestCase(1, 5, 600.0)]
        [TestCase(3, 2, 300.0)]
        [TestCase(4, 2, 200.0)]
        public void ExecutePagingDataTableTest(int pageIndex, int pageSize, double orderTotal) {
            const string selectSql = SelectDemoOrders + @" WHERE ORDER_TOTAL > :ORDER_TOTAL";
            var parameters = new[] { new AdoParameter("ORDER_TOTAL", orderTotal) };

            var title = string.Format("ExecutePagingDataTable(pageIndex=[{0}], pageSize=[{1}])", pageIndex, pageSize);
            using(new OperationTimer(title, false)) {
                using(var cmd = OdpRepository.GetCommand(selectSql))
                using(var pagingTable = OdpRepository.ExecutePagingDataTable(cmd, pageIndex, pageSize, parameters)) {
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
        [TestCase(0, 5, 100.0)]
        [TestCase(2, 2, 100.0)]
        [TestCase(3, 3, 150.0)]
        [TestCase(1, 5, 600.0)]
        [TestCase(3, 2, 300.0)]
        [TestCase(4, 2, 200.0)]
        public void ExecutePagingDataTableByQueryTest(int pageIndex, int pageSize, double orderTotal) {
            const string selectSql = SelectDemoOrders + @" WHERE ORDER_TOTAL > :ORDER_TOTAL";
            var parameters = new[] { new AdoParameter("ORDER_TOTAL", orderTotal) };

            var title = string.Format("ExecutePagingDataTableByQuery(pageIndex={0}, pageSize={1})", pageIndex, pageSize);
            using(new OperationTimer(title)) {
                using(var pagingTable = OdpRepository.ExecutePagingDataTable(selectSql, pageIndex, pageSize, parameters)) {
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
        /// <param name="orderTotal"></param>
        [TestCase(0, 5, 100.0)]
        [TestCase(2, 2, 100.0)]
        [TestCase(3, 3, 150.0)]
        [TestCase(1, 5, 600.0)]
        [TestCase(3, 2, 300.0)]
        [TestCase(4, 2, 200.0)]
        public void ExecutePagingDataTableBySelectSqlTest(int pageIndex, int pageSize, double orderTotal) {
            const string selectSql = SelectDemoOrders + @" WHERE ORDER_TOTAL > :ORDER_TOTAL";
            var parameters = new[] { new AdoParameter("ORDER_TOTAL", orderTotal) };

            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex={0}, pageSize={1})", pageIndex, pageSize))
                )
            using(var pagingTable = OdpRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize, parameters)) {
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

        [TestCase(SelectDemoCustomers, 0, 10)]
        [TestCase(SelectDemoStates, 1, 10)]
        public void ExecutePagingDataTableBySelectSqlTest(string selectSql, int pageIndex, int pageSize) {
            using(
                new OperationTimer(string.Format("ExecutePagingDataTableBySqlString(pageIndex={0}, pageSize={1})", pageIndex, pageSize))
                )
            using(var pagingTable = OdpRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize)) {
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
            OdpRepository
                .ExecuteScalarBySqlString(SelectDemoCustomers)
                .AsInt()
                .Should().Be.GreaterThan(0);

            OdpRepository
                .ExecuteScalarBySqlString(CountEmp)
                .AsInt()
                .Should().Be.GreaterThan(0);
        }

        #endregion

        #region << ExecuteReader >>

        [Test]
        public void ExecuteReaderByQuery() {
            using(var reader = OdpRepository.ExecuteReaderBySqlString(SelectEmp)) {
                reader.Read().Should().Be.True();

                if(IsDebugEnabled)
                    log.Debug(reader.ToString(true));
            }
        }

        [Test]
        public void ExecuteReaderByProcedure() {
            using(var reader = OdpRepository.ExecuteReader(SelectDemoCustomers)) {
                reader.Read().Should().Be.True();

                if(IsDebugEnabled)
                    log.Debug(reader.ToString(true));
            }
        }

        #endregion

        #region << Count Of DataReader >>

        [Test]
        public void CountOfDataReader() {
            OdpRepository
                .CountBySqlString(SelectEmp)
                .AsInt()
                .Should().Be.GreaterThan(0);
        }

        #endregion

        #region << Exists >>

        [Test]
        public void ExistsByDataReader() {
            OdpRepository
                .ExistsBySqlString(SelectEmp)
                .Should().Be.True();
        }

        #endregion

        #region << ExecuteReader{TPersisntent} >>

        [Test]
        public void ExecuteReaderToInstanceByNameMapping() {
            using(var cmd = OdpRepository.GetCommand(SelectDemoStates)) {
                var jobs = OdpRepository.ExecuteInstance<State>(CapitalizeMapper, cmd);

                jobs.All(job => job.St.IsNotWhiteSpace()).Should().Be.True();
                jobs.All(job => job.StateName.IsNotWhiteSpace()).Should().Be.True();
            }
        }

        [TestCase(0, 10)]
        [TestCase(3, 20)]
        [TestCase(5, 10)]
        public void ExecuteReaderWithPagingByNameMapper(int pageIndex, int pageSize) {
            using(var cmd = OdpRepository.GetCommand(SelectDemoStates)) {
                var jobs = OdpRepository.ExecuteInstance<State>(CapitalizeMapper, cmd, pageIndex, pageSize);

                jobs.All(job => job.St.IsNotWhiteSpace()).Should().Be.True();
                jobs.All(job => job.StateName.IsNotWhiteSpace()).Should().Be.True();
            }
        }

        [TestCase(0, 10)]
        [TestCase(3, 20)]
        [TestCase(5, 10)]
        public void ExecuteReaderWithPagingByPersister(int pageIndex, int pageSize) {
            var persister = new CapitalizeReaderPersister<State>();

            var jobs =
                Task.Factory
                    .StartNew(() => {
                                  using(var cmd = OdpRepository.GetCommand(SelectDemoStates))
                                      return OdpRepository.ExecuteInstance<State>(persister, cmd, pageIndex, pageSize);
                              })
                    .Result;

            jobs.All(job => job.St.IsNotWhiteSpace()).Should().Be.True();
            jobs.All(job => job.StateName.IsNotWhiteSpace()).Should().Be.True();
        }

        #endregion
    }
}