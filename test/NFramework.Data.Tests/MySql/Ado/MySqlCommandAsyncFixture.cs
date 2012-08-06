using System;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NSoft.NFramework.Data.MySql.EnterpriseLibrary;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MySql.Ado {
    [TestFixture]
    public class MySqlCommandAsyncFixture : MySqlAdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public MySqlDatabase DefaultMySqlDb {
            get { return DefaultMySqlRepository.Db; }
        }

        [Test]
        public void InvalidQueryStringTest() {
            Assert.Throws<MySqlException>(
                () => MySqlCommandAsync
                          .ExecuteDataTableAsync(DefaultMySqlDb, "SEL * FRO Customers")
                          .Wait());
        }

        [Test]
        public void NotExistTableTest() {
            Assert.Throws<AggregateException>(
                () => MySqlCommandAsync
                          .ExecuteDataTableAsync(DefaultMySqlDb, "SELECT * FROM Northwind.XXXXXXX")
                          .Wait());
        }

        // Database 성능테스트를 위한 Warm Up 입니다.
        [Test]
        public void AWarmUpConnection() {
            Parallel.Invoke(() => {
                                using(var reader = DefaultMySqlRepository.ExecuteReaderBySqlString(SQL_CUSTOMER_SELECT)) {
                                    var count = 0;
                                    while(reader.Read())
                                        count++;
                                    count.Should().Be.GreaterThan(0);
                                }
                            },
                            () => {
                                using(var reader = DefaultMySqlRepository.ExecuteReaderBySqlString(SQL_ORDER_SELECT)) {
                                    var count = 0;
                                    while(reader.Read())
                                        count++;
                                    count.Should().Be.GreaterThan(0);
                                }
                            },
                            () => {
                                using(var reader = DefaultMySqlRepository.ExecuteReaderBySqlString(SQL_ORDER_DETAIL_SELECT)) {
                                    var count = 0;
                                    while(reader.Read())
                                        count++;
                                    count.Should().Be.GreaterThan(0);
                                }
                            });
        }

        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(2, 5)]
        public void ExecuteDataTableAsyncTest(int firstResult, int maxResults) {
            using(var asyncTask = DefaultMySqlDb.ExecuteDataTableAsync(SQL_ORDER_SELECT, firstResult, maxResults)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
            }

            Thread.Sleep(1);

            using(var asyncTask = DefaultMySqlDb.ExecuteDataTableAsync(SQL_INVOICE_SELECT, firstResult, maxResults)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
            }
        }

        [Test]
        public void ExecuteReaderAsyncTest() {
            var readerTask = DefaultMySqlDb.ExecuteReaderAsync(SQL_ORDER_SELECT);

            readerTask.Wait();
            AssertTaskIsCompleted(readerTask);

            using(var reader = readerTask.Result) {
                Assert.IsTrue(reader.Read());

                if(log.IsDebugEnabled)
                    log.Debug(reader.ToString(true));
            }

            Thread.Sleep(1);

            readerTask = DefaultMySqlDb.ExecuteReaderAsync(SQL_INVOICE_SELECT);

            readerTask.Wait();
            AssertTaskIsCompleted(readerTask);

            using(var reader = readerTask.Result) {
                Assert.IsTrue(reader.Read());

                if(log.IsDebugEnabled)
                    log.Debug(reader.ToString(true));
            }
        }

        [Test]
        public void ExecuteScalarAsyncTest() {
            using(var readerTask = DefaultMySqlDb.ExecuteScalarAsync(SQL_CUSTOMER_COUNT)) {
                readerTask.Wait();
                AssertTaskIsCompleted(readerTask);

                var customerCount = readerTask.Result.AsInt();
                Assert.IsTrue(customerCount > 0);
            }
        }

        [TestCase(4, 5)]
        [TestCase(2, 10)]
        public void ThreadTest(int repeat, int concurrent) {
            AWarmUpConnection();

            for(var i = 0; i < repeat; i++) {
                TestTool.RunTasks(concurrent,
                                  ExecuteReaderAsyncTest,
                                  () => ExecuteDataTableAsyncTest(0, 0),
                                  () => ExecuteDataTableAsyncTest(1, 5),
                                  () => ExecuteDataTableAsyncTest(2, 10),
                                  ExecuteReaderAsyncTest,
                                  ExecuteScalarAsyncTest,
                                  ExecuteReaderAsyncTest);

                Thread.Sleep(50);
            }
        }

        private static void AssertTaskIsCompleted(Task task) {
            task.Should().Not.Be.Null();
            task.IsCompleted.Should().Be.True();
            task.IsFaulted.Should().Be.False();
            task.IsCanceled.Should().Be.False();
        }
    }
}