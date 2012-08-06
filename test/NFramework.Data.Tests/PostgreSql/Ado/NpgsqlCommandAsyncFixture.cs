using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.PostgreSql.EnterpriseLibrary;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.PostgreSql.Ado {
    [TestFixture]
    public class NpgsqlCommandAsyncFixture : NpgsqlAdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public NpgsqlDatabase NorthwindDb {
            get { return NorthwindPostgreSqlRepository.Db; }
        }

        internal static void AssertTaskIsCompleted(Task task) {
            Assert.IsTrue(task.IsCompleted);
            Assert.IsFalse(task.IsFaulted, "Task is faulted");
            Assert.IsFalse(task.IsCanceled, "Task is canceled");
        }

        [Test]
        public void Raise_Exception_When_Invalid_QueryTest() {
            Assert.Throws<AggregateException>(() => {
                                                  using(var cmd = NorthwindDb.GetNpgsqlCommand("SELECT * FRO sysobjects")) {
                                                      var table = PostgreSqlCommandAsync.ExecuteDataTableAsync(NorthwindDb, cmd).Result;
                                                      Assert.IsNotNull(table);
                                                  }
                                              });
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(2, 5)]
        public void Can_ExecuteDataTableAsync(int firstResult, int maxResults) {
            using(var cmd = NorthwindDb.GetNpgsqlCommand(SQL_ORDER_DETAIL_SELECT))
            using(var asyncTask = NorthwindDb.ExecuteDataTableAsync(cmd, firstResult, maxResults)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
                if(maxResults > 0)
                    maxResults.Should().Be.GreaterThanOrEqualTo(dataTable.Rows.Count);
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(2, 5)]
        public void ExecuteDataTableAsync_With_ParametersTest(int firstResult, int maxResults) {
            using(var cmd = NorthwindDb.GetNpgsqlCommand(SQL_ORDER_DETAIL_BY_ORDER_ID))
            using(var asyncTask = NorthwindDb.ExecuteDataTableAsync(cmd, firstResult, maxResults, OrderTestParameter)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
                if(maxResults > 0)
                    maxResults.Should().Be.GreaterThanOrEqualTo(dataTable.Rows.Count);
            }
        }

        [Test]
        public void ExecuteNonQueryAsyncTest() {
            using(var cmd = NorthwindDb.GetNpgsqlCommand(SQL_REGION_DELETE))
            using(var asyncTask = NorthwindDb.ExecuteNonQueryAsync(cmd)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                // ExecuteNonQuery를 수행하면, 삭제한 레코드가 없기 때문에  결과 값은 0 입니다.
                //
                Assert.AreEqual(0, asyncTask.Result);
            }
        }

        [Test]
        public void ExecuteNonQueryAsyncByQueryTest() {
            using(var asyncTask = NorthwindDb.ExecuteNonQueryAsync(SQL_REGION_DELETE)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                // ExecuteNonQuery를 수행하면, 삭제한 레코드가 없기 때문에  결과 값은 0 입니다.
                //
                asyncTask.Result.Should().Be(0);
            }
        }

        [Test]
        public void ExecuteNonQueryAsyncBySqlString_WithParametersTest() {
            var region = new AdoParameter("RegionID", 1000);

            using(var asyncTask = NorthwindDb.ExecuteNonQueryAsync("DELETE FROM Region where RegionID >= :RegionID", region)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                // ExecuteNonQuery를 수행하면, 삭제한 레코드가 없기 때문에  결과 값은 0 입니다.
                //
                asyncTask.Result.Should().Be(0);
            }
        }

        [Test]
        public void ExecuteReaderAsyncTest() {
            using(var command = NorthwindDb.GetNpgsqlCommand(SQL_CUSTOMER_SELECT))
            using(var readerTask = NorthwindDb.ExecuteReaderAsync(command)) {
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
        public void ExecuteReaderBySqlStringAsyncTest() {
            using(var readerTask = NorthwindDb.ExecuteReaderBySqlStringAsync(SQL_CUSTOMER_SELECT)) {
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
        public void ExecuteScalarAsyncTest() {
            using(var command = NorthwindDb.GetNpgsqlCommand(SQL_CUSTOMER_COUNT))
            using(var scalarTask = NorthwindDb.ExecuteScalarAsync(command)) {
                scalarTask.Wait();
                AssertTaskIsCompleted(scalarTask);

                var customerCount = scalarTask.Result.AsInt();
                Assert.IsTrue(customerCount > 0);
            }
        }

        [Test]
        public void ExecuteScalarBySqlStringAsyncTest() {
            using(var scalarTask = NorthwindDb.ExecuteScalarAsync(SQL_CUSTOMER_COUNT)) {
                scalarTask.Wait();
                AssertTaskIsCompleted(scalarTask);

                var customerCount = scalarTask.Result.AsInt();
                Assert.IsTrue(customerCount > 0);
            }
        }

        [Test]
        public virtual void ExecuteMapObjectAsyncTest() {
            using(var cmd = NorthwindDb.GetNpgsqlCommand(SQL_INVOICE_SELECT)) {
                var mapTask = NorthwindDb.ExecuteMapObject(cmd, () => new Invoice(), TrimMapper, 0, 0, null);
                mapTask.Wait();

                var invoices = mapTask.Result;
                invoices.Count.Should().Be.GreaterThan(0);
                invoices.All(invoice => invoice.ProductID > 0).Should().Be.True();
                invoices.All(invoice => invoice.Quantity > 0).Should().Be.True();
            }
        }

        [TestCase(3, 10)]
        [TestCase(5, 5)]
        [TestCase(10, 3)]
        public virtual void ThreadTest(int repeat, int concurrent) {
            for(var n = 0; n < repeat; n++) {
                TestTool.RunTasks(concurrent,
                                  ExecuteNonQueryAsyncTest,
                                  ExecuteNonQueryAsyncByQueryTest,
                                  ExecuteNonQueryAsyncBySqlString_WithParametersTest,
                                  ExecuteReaderAsyncTest,
                                  ExecuteReaderBySqlStringAsyncTest,
                                  ExecuteScalarAsyncTest,
                                  ExecuteScalarBySqlStringAsyncTest,
                                  ExecuteMapObjectAsyncTest);

                Thread.Sleep(10);
            }
        }
    }
}