using System;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Data.SQLite.EnterpriseLibrary;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.SQLite {
    [TestFixture]
    public class SQLiteCommandAsyncFixture : SQLiteAdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public SQLiteDatabase DefaultSQLiteDb {
            get { return DefaultSQLiteRepository.Db; }
        }

        [Test]
        public void InvalidQueryStringTest() {
            // DB 마다 다 다르네...
            Assert.Throws<NotSupportedException>(
                () => SQLiteCommandAsync
                          .ExecuteDataTableAsync(DefaultSQLiteDb, "SEL * FRO Customers")
                          .Wait());
        }

        [Test]
        public void NotExistTableTest() {
            Assert.Throws<AggregateException>(
                () => SQLiteCommandAsync
                          .ExecuteDataTableAsync(DefaultSQLiteDb, "SELECT * FROM XXXXXXX")
                          .Wait());
        }

        // Database 성능테스트를 위한 Warm Up 입니다.
        [Test]
        public void AWarmUpConnection() {
            Parallel.Invoke(() => {
                                using(var reader = DefaultSQLiteRepository.ExecuteReaderBySqlString(SelectOrder)) {
                                    var count = 0;
                                    while(reader.Read())
                                        count++;
                                    count.Should().Be.GreaterThan(0);
                                }
                            },
                            () => {
                                using(var reader = DefaultSQLiteRepository.ExecuteReaderBySqlString(SelectOrderDetails)) {
                                    var count = 0;
                                    while(reader.Read())
                                        count++;
                                    count.Should().Be.GreaterThan(0);
                                }
                            },
                            () => {
                                using(var reader = DefaultSQLiteRepository.ExecuteReaderBySqlString(SelectCustomer)) {
                                    var count = 0;
                                    while(reader.Read())
                                        count++;
                                    count.Should().Be.GreaterThan(0);
                                }
                            });
        }

        [Test]
        public void ExecuteDataTableAsyncTest([Range(1, 5)] int firstResult, [Values(3, 5)] int maxResults) {
            using(var asyncTask = DefaultSQLiteDb.ExecuteDataTableAsync(SelectOrder, firstResult, maxResults)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
            }

            Thread.Sleep(1);

            using(var asyncTask = DefaultSQLiteDb.ExecuteDataTableAsync(SelectOrderDetails, firstResult, maxResults)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
            }
        }

        [Test]
        public void ExecuteReaderAsyncTest() {
            var readerTask = DefaultSQLiteDb.ExecuteReaderAsync(SelectCustomer);

            readerTask.Wait();
            AssertTaskIsCompleted(readerTask);

            using(var reader = readerTask.Result) {
                Assert.IsTrue(reader.Read());

                if(log.IsDebugEnabled)
                    log.Debug(reader.ToString(true));
            }

            Thread.Sleep(1);

            readerTask = DefaultSQLiteDb.ExecuteReaderAsync(SelectOrderDetails);

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
            using(var command = DefaultSQLiteDb.GetSQLiteCommand(CountOrder))
            using(var scalarTask = DefaultSQLiteDb.ExecuteScalarAsync(command)) {
                scalarTask.Wait();
                AssertTaskIsCompleted(scalarTask);

                var customerCount = scalarTask.Result.AsInt();
                Assert.IsTrue(customerCount > 0);
            }
        }

        [TestCase(2, 10)]
        [TestCase(2, 4)]
        public void ThreadTest(int repeat, int concurrent) {
            for(var i = 0; i < repeat; i++) {
                AWarmUpConnection();

                TestTool.RunTasks(concurrent,
                                  () => ExecuteDataTableAsyncTest(0, 0),
                                  () => ExecuteDataTableAsyncTest(1, 100),
                                  () => ExecuteDataTableAsyncTest(2, 100),
                                  ExecuteReaderAsyncTest,
                                  ExecuteScalarAsyncTest);
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