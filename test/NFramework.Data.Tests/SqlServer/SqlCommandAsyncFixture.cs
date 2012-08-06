using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.SqlServer;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Ado.SqlServer {
    /// <summary>
    /// SqlCommand의 비동기 작업을 수행합니다.
    /// </summary>
    [TestFixture]
    public class SqlCommandAsyncFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public SqlDatabase NWindDatabase {
            get { return (SqlDatabase)NorthwindAdoRepository.Db; }
        }

        private static void AssertTaskIsCompleted(Task task) {
            Assert.IsTrue(task.IsCompleted);
            Assert.IsFalse(task.IsFaulted, "Task is faulted");
            Assert.IsFalse(task.IsCanceled, "Task is canceled");
        }

        [Test]
        public void Can_Raise_Exception_When_Invalid_Query() {
            Assert.Throws<AggregateException>(() =>
                                              With.TryActionAsync(() => {
                                                                      using(
                                                                          var cmd =
                                                                              NWindDatabase.GetSqlCommand("SELECT * FRO sysobjects")) {
                                                                          var table =
                                                                              SqlCommandAsync.ExecuteDataTableAsync(NWindDatabase, cmd).
                                                                                  Result;
                                                                          Assert.IsNotNull(table);
                                                                      }
                                                                  },
                                                                  age => { throw new AggregateException(age); }));
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(2, 5)]
        public void Can_ExecuteDataTableAsync(int firstResult, int maxResults) {
            using(var asyncTask = NWindDatabase.ExecuteDataTableAsync(SQL_ORDER_DETAIL_SELECT, firstResult, maxResults)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);

                if(maxResults > 0)
                    dataTable.Rows.Count.Should().Be.LessThanOrEqualTo(maxResults);
                else
                    dataTable.Rows.Count.Should().Be.GreaterThan(0);
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(2, 5)]
        public void Can_ExecuteDataTableAsync_With_Parameters(int firstResult, int maxResults) {
            using(
                var asyncTask = NWindDatabase.ExecuteDataTableAsync(SQL_ORDER_DETAILS_BY_ORDER_ID, firstResult, maxResults,
                                                                    OrderTestParameter)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);

                if(maxResults > 0)
                    dataTable.Rows.Count.Should().Be.LessThanOrEqualTo(maxResults);
                else
                    dataTable.Rows.Count.Should().Be.GreaterThan(0);
            }
        }

        [Test]
        public void Can_ExecuteNonQueryAsync() {
            using(var cmd = NWindDatabase.GetSqlCommand(SQL_REGION_DELETE))
            using(var asyncTask = SqlCommandAsync.ExecuteNonQueryAsync(NWindDatabase, cmd)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                // ExecuteNonQuery를 수행하면, 삭제한 레코드가 없기 때문에  결과 값은 0 입니다.
                //
                Assert.AreEqual(0, asyncTask.Result);
            }
        }

        [Test]
        public void Can_ExecuteNonQueryAsyncByQuery() {
            using(var asyncTask = NWindDatabase.ExecuteNonQueryAsync(SQL_REGION_DELETE)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                // ExecuteNonQuery를 수행하면, 삭제한 레코드가 없기 때문에  결과 값은 0 입니다.
                //
                asyncTask.Result.Should().Be(0);
            }
        }

        [Test]
        public void Can_ExecuteNonQueryAsyncBySqlString_WithParameters() {
            using(var asyncTask =
                NWindDatabase.ExecuteNonQueryBySqlStringAsync("DELETE FROM Region where RegionID >= @RegionID",
                                                              new AdoParameter("RegionID", 1000))) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                // ExecuteNonQuery를 수행하면, 삭제한 레코드가 없기 때문에  결과 값은 0 입니다.
                //
                asyncTask.Result.Should().Be(0);
            }
        }

        [Test]
        public void Can_ExecuteReaderAsync() {
            using(var command = NWindDatabase.GetSqlCommand(SQL_ORDER_DETAIL_SELECT)) {
                var readerTask = NWindDatabase.ExecuteReaderAsync(command);
                readerTask.Wait();
                AssertTaskIsCompleted(readerTask);

                using(var reader = readerTask.Result) {
                    Assert.IsTrue(reader.Read());

                    if(IsDebugEnabled)
                        log.Debug(reader.ToString(true));
                }
            }
        }

        [Test]
        public void Can_ExecuteReaderBySqlStringAsync() {
            var readerTask = NWindDatabase.ExecuteReaderBySqlStringAsync(SQL_ORDER_DETAIL_SELECT);

            readerTask.Wait();
            AssertTaskIsCompleted(readerTask);

            using(var reader = readerTask.Result) {
                Assert.IsTrue(reader.Read());

                if(IsDebugEnabled)
                    log.Debug(reader.ToString(true));
            }
        }

        [Test]
        public void Can_ExecuteScalarAsync() {
            using(var command = NWindDatabase.GetSqlCommand(SQL_REGION_COUNT))
            using(var readerTask = NWindDatabase.ExecuteScalarAsync(command)) {
                readerTask.Wait();
                AssertTaskIsCompleted(readerTask);

                readerTask.Result.AsInt().Should().Be.GreaterThan(0);
            }
        }

        [Test]
        public void Can_ExecuteScalarBySqlStringAsync() {
            using(var readerTask = NWindDatabase.ExecuteScalarBySqlStringAsync(SQL_REGION_COUNT)) {
                readerTask.Wait();
                AssertTaskIsCompleted(readerTask);

                readerTask.Result.AsInt().Should().Be.GreaterThan(0);
            }
        }

        [Test]
        public virtual void ExecuteMapObjectAsyncTest() {
            using(var cmd = NWindDatabase.GetSqlCommand(SQL_INVOICE_SELECT)) {
                var mapTask = NWindDatabase.ExecuteMapObject(cmd, () => new Invoice(), TrimMapper, 0, 0, null);
                mapTask.Wait();

                var invoices = mapTask.Result;
                invoices.Count.Should().Be.GreaterThan(0);
                invoices.All(invoice => invoice.ProductID > 0).Should().Be.True();
                invoices.All(invoice => invoice.Quantity > 0).Should().Be.True();
            }
        }

        [TestCase(3, 10)]
        [TestCase(5, 5)]
        public void ThreadTest(int repeat, int concurrent) {
            for(int i = 0; i < repeat; i++) {
                TestTool.RunTasks(concurrent,
                                  Can_ExecuteNonQueryAsync,
                                  Can_ExecuteNonQueryAsyncByQuery,
                                  Can_ExecuteNonQueryAsyncBySqlString_WithParameters,
                                  Can_ExecuteReaderAsync,
                                  Can_ExecuteReaderBySqlStringAsync,
                                  Can_ExecuteScalarAsync,
                                  Can_ExecuteScalarBySqlStringAsync,
                                  ExecuteMapObjectAsyncTest);

                Console.WriteLine("...............................................");
                Thread.Sleep(50);
                Console.WriteLine("...............................................");
            }
        }
    }
}