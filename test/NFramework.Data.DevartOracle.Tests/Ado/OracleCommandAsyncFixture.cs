using System;
using System.Threading;
using System.Threading.Tasks;
using Devart.Data.Oracle;
using NSoft.NFramework.Data.DevartOracle.EnterpriseLibrary;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.DevartOracle.Ado {
    [TestFixture]
    public class OracleCommandAsyncFixture : OracleFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public OracleDatabase DefaultOracleDb {
            get { return DefaultOracleRepository.Db; }
        }

        private static void AssertTaskIsCompleted(Task task) {
            Assert.IsTrue(task.IsCompleted);
            Assert.IsFalse(task.IsFaulted, "Task is faulted!!!");
            Assert.IsFalse(task.IsCanceled, "Task is canceled!!!");
        }

        [Test]
        public void Can_Raise_Exception_When_Invalid_Query() {
            if(IsDebugEnabled)
                log.Debug("[FROM] 키워드를 잘못 썼기 때문에 예외가 발생해야 합니다.");

            Assert.Throws<AggregateException>(() => {
                                                  using(var cmd = DefaultOracleDb.GetOracleCommand("SELECT * FRO TAB")) {
                                                      var table = OracleCommandAsync.ExecuteDataTableAsync(DefaultOracleDb, cmd).Result;
                                                      Assert.IsNotNull(table);
                                                  }
                                              });
        }

        [Test]
        public void CreateOracleConnectionAsynchronous() {
            var connStr = AdoTool.DefaultConnectionStringSettings.ConnectionString;

            if(IsDebugEnabled)
                log.Debug("ConnectionString=" + connStr);


            bool newConnection = false;
            using(var oraConn = OracleTool.CreateOracleConnection(DefaultOracleDb, ref newConnection)) {
                using(var oraCommand = new OracleCommand(SelectDept, oraConn))
                using(var reader = oraCommand.ExecuteReader()) {
                    Assert.IsTrue(reader.Read());
                }
                if(newConnection)
                    oraConn.Close();
            }
        }

        [Test]
        public void AWarmUpConnection() {
            Parallel.Invoke(() => {
                                using(var reader = DefaultOracleRepository.ExecuteReaderBySqlString(SelectDept)) {
                                    var count = 0;
                                    while(reader.Read())
                                        count++;
                                    count.Should().Be.GreaterThan(0);
                                }
                            },
                            () => {
                                using(var reader = DefaultOracleRepository.ExecuteReaderBySqlString(SelectEmp)) {
                                    var count = 0;
                                    while(reader.Read())
                                        count++;
                                    count.Should().Be.GreaterThan(0);
                                }
                            },
                            () => {
                                using(var reader = DefaultOracleRepository.ExecuteReaderBySqlString(SelectDemoCustomers)) {
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
            using(var asyncTask = DefaultOracleDb.ExecuteDataTableAsync(SelectEmp, firstResult, maxResults)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
            }

            Thread.Sleep(1);

            using(var asyncTask = DefaultOracleDb.ExecuteDataTableAsync(SelectDemoCustomers, firstResult, maxResults)) {
                asyncTask.Wait();
                AssertTaskIsCompleted(asyncTask);

                var dataTable = asyncTask.Result;

                Assert.IsNotNull(dataTable);
                Assert.IsFalse(dataTable.HasErrors);
            }
        }

        [Test]
        public void ExecuteReaderAsyncTest() {
            var readerTask = DefaultOracleDb.ExecuteReaderAsync(SelectDept);

            readerTask.Wait();
            AssertTaskIsCompleted(readerTask);

            using(var reader = readerTask.Result) {
                Assert.IsTrue(reader.Read());
                Console.WriteLine(reader.ToString(true));
            }

            Thread.Sleep(1);

            readerTask = DefaultOracleDb.ExecuteReaderAsync(SelectEmp);

            readerTask.Wait();
            AssertTaskIsCompleted(readerTask);

            using(var reader = readerTask.Result) {
                Assert.IsTrue(reader.Read());
                Console.WriteLine(reader.ToString(true));
            }
        }

        [Test]
        public void ExecuteScalarAsyncTest() {
            using(var readerTask = DefaultOracleDb.ExecuteScalarAsync(CountEmp)) {
                readerTask.Wait();
                AssertTaskIsCompleted(readerTask);

                var customerCount = readerTask.Result.AsInt();
                Assert.IsTrue(customerCount > 0);
            }
        }

        [TestCase(3, 10)]
        [TestCase(5, 5)]
        public void ThreadTest(int repeat, int concurrent) {
            AWarmUpConnection();

            for(var i = 0; i < repeat; i++) {
                TestTool.RunTasks(concurrent,
                                  CreateOracleConnectionAsynchronous,
                                  () => ExecuteDataTableAsyncTest(0, 0),
                                  () => ExecuteDataTableAsyncTest(0, 5),
                                  () => ExecuteDataTableAsyncTest(1, 0),
                                  ExecuteReaderAsyncTest,
                                  ExecuteScalarAsyncTest);

                Thread.Sleep(50);
            }
        }
    }
}