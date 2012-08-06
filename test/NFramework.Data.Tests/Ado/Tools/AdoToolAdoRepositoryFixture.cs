using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Reflections;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Tools {
    /// <summary>
    /// AdoRepository의 ExecuteXXXX() 메소드에 대한 비동기 실행을 수행하는 확장 메소드에 대한 테스트입니다.
    /// </summary>
    [TestFixture]
    public class AdoToolAdoRepositoryFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly string[] Sections = new[]
                                                    {
                                                        "Customer", "Order", "Order Details", "Products", "Employees", "Invoices",
                                                        "Orders Qry"
                                                    };

        private static readonly string[] Sections2 = Sections.Concat(Sections).Concat(Sections).ToArray();

        private const string QueryKey = "GetAll";

        [Test]
        public void _WarmUpTablesForAccuracy() {
            foreach(var section in Sections) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                using(var table = NorthwindAdoRepository.ExecuteDataTable(query)) {
                    Assert.IsFalse(table.HasErrors);
                }
            }
        }

        [Test]
        public void Can_Serial_ExecuteDataSet() {
            // 여러 테이블의 정보를 비동기적으로 거의 동시에 로드합니다.
            foreach(var section in Sections2) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var dataset = NorthwindAdoRepository.ExecuteDataSet(query);

                Assert.AreEqual(1, dataset.Tables.Count);
                Assert.IsFalse(dataset.Tables[0].HasErrors);

                if(IsDebugEnabled)
                    log.Debug("Table[{0}] has [{1}] rows", dataset.Tables[0].TableName, dataset.Tables[0].Rows.Count);

                dataset.Dispose();
            }
        }

        [Test]
        public void Can_ExecuteDataSetTask() {
            var dsTasks = new List<Task>();

            // 여러 테이블의 정보를 비동기적으로 거의 동시에 로드합니다.
            //
            foreach(var section in Sections2) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var task = NorthwindAdoRepository
                    .ExecuteDataSetAsync(query)
                    .ContinueWith(antecedent => {
                                      using(var dataset = antecedent.Result) {
                                          Assert.IsNotNull(dataset);
                                          Assert.AreEqual(1, dataset.Tables.Count);
                                          Assert.IsFalse(dataset.Tables[0].HasErrors);

                                          //if(IsDebugEnabled)
                                          //    log.Debug("Table[{0}] has [{1}] rows", dataset.Tables[0].TableName, dataset.Tables[0].Rows.Count);
                                      }
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
                dsTasks.Add(task);
            }

            Task.WaitAll(dsTasks.ToArray());
            dsTasks.All(t => t.IsCompleted).Should().Be.True();
        }

        [Test]
        public void Can_ChainTask_ExecuteDataSetTask() {
            var dsTasks = new List<Task<DataSet>>();

            // 테이블 정보를 로딩하는 작업 자체는 비동기로 이루어지지만, 테이블별로 순서대로 로드되도록 합니다.
            // Loading Task의 순차 실행이라 보시면 됩니다. (단 이런 작업이 설정된대로 자동으로 수행되므로, 최족 Task의 실행여부만 판단하면 됩니다.)
            //
            foreach(var section in Sections2) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var prevTask = dsTasks.LastOrDefault();

                // ExecuteDataSet() 실행을 순서대로 수행하도록 한다.
                if(prevTask != null) {
                    var task =
                        prevTask
                            .ContinueWith(_ => NorthwindAdoRepository.ExecuteDataSetAsync(query),
                                          TaskContinuationOptions.ExecuteSynchronously)
                            .Unwrap();
                    dsTasks.Add(task);
                }
                else {
                    var task = NorthwindAdoRepository.ExecuteDataSetAsync(query);
                    dsTasks.Add(task);
                }
            }

            foreach(var task in dsTasks) {
                var dataset = task.Result;

                dataset.Tables.Count.Should().Be(1);
                dataset.Tables[0].HasErrors.Should().Be.False();

                if(IsDebugEnabled)
                    log.Debug("Table[{0}] has [{1}] rows", dataset.Tables[0].TableName, dataset.Tables[0].Rows.Count);

                dataset.Dispose();
            }
        }

        [Test]
        public void Can_ExecuteDataTableTask() {
            var dsTasks = new List<Task>();

            // 여러 테이블의 정보를 비동기적으로 거의 동시에 로드합니다.
            //
            foreach(var section in Sections2) {
                var query = DefaultAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var task = DefaultAdoRepository
                    .ExecuteDataTableAsync(query)
                    .ContinueWith(antecedent => {
                                      using(var dataTable = antecedent.Result) {
                                          Assert.IsFalse(dataTable.HasErrors);

                                          if(IsDebugEnabled)
                                              log.Debug("Table[{0}] has [{1}] rows", dataTable.TableName, dataTable.Rows.Count);
                                      }
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
                dsTasks.Add(task);
            }

            Task.WaitAll(dsTasks.ToArray());
            dsTasks.All(t => t.IsCompleted).Should().Be.True();
        }

        [Test]
        public void Can_ExecutePagingDataTableTask() {
            var dsTasks = new List<Task>();

            // 여러 테이블의 정보를 비동기적으로 거의 동시에 로드합니다.
            //
            foreach(var section in Sections2) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var task = NorthwindAdoRepository
                    .ExecutePagingDataTableAsync(query, 1, 10)
                    .ContinueWith(antecedent => {
                                      using(var pagingDataTable = antecedent.Result) {
                                          Assert.IsFalse(pagingDataTable.Table.HasErrors);

                                          //if(IsDebugEnabled)
                                          //    log.Debug("PagingDataTable=", pagingDataTable);
                                      }
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
                dsTasks.Add(task);
            }

            Task.WaitAll(dsTasks.ToArray());
            dsTasks.All(t => t.IsCompleted).Should().Be.True();
        }

        [Test]
        public void Can_ExecuteNonQueryTask() {
            var executeScalarTasks = new List<Task<int>>();

            foreach(var section in Sections) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var countQuery = AdoTool.GetCountingSqlString(query);

                var task = NorthwindAdoRepository.ExecuteNonQueryAsync(countQuery);
                executeScalarTasks.Add(task);
            }

            Task.WaitAll(executeScalarTasks.ToArray());
            executeScalarTasks.All(t => t.IsCompleted).Should().Be.True();
        }

        [Test]
        public void Can_ExecuteDataReaderTask() {
            var dsTasks = new List<Task>();

            // 여러 테이블의 정보를 비동기적으로 거의 동시에 로드합니다.
            //
            foreach(var section in Sections2) {
                var query = DefaultAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var task = DefaultAdoRepository
                    .ExecuteReaderAsync(query)
                    .ContinueWith(antecedent => {
                                      using(var reader = antecedent.Result) {
                                          var count = 0;

                                          while(reader.Read()) {
                                              count++;
                                          }

                                          if(IsDebugEnabled)
                                              log.Debug("결과 셋의 갯수=[{0}]", count);

                                          reader.Dispose();
                                      }
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
                dsTasks.Add(task);
            }

            Task.WaitAll(dsTasks.ToArray());
            dsTasks.All(t => t.IsCompleted).Should().Be.True();
        }

        [Test]
        public void Can_ExecuteScalarTask() {
            var executeScalarTasks = new List<Task<object>>();

            foreach(var section in Sections) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var countQuery = AdoTool.GetCountingSqlString(query);

                var task = NorthwindAdoRepository.ExecuteScalarAsync(countQuery);
                executeScalarTasks.Add(task);
            }

            Task.WaitAll(executeScalarTasks.ToArray());
            executeScalarTasks.All(t => t.IsCompleted).Should().Be.True();

            foreach(var task in executeScalarTasks) {
                var scalar = (int)task.Result;
                Assert.Greater(scalar, 0);

                if(IsDebugEnabled)
                    log.Debug("Count = " + scalar);
            }
        }

        [Test]
        public void Can_CountTask() {
            var countTasks = new List<Task<int>>();

            foreach(var section in Sections) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);

                var task = NorthwindAdoRepository.CountAsync(query);
                countTasks.Add(task);
            }

            Task.WaitAll(countTasks.ToArray());
            countTasks.All(t => t.IsCompleted).Should().Be.True();

            foreach(var task in countTasks) {
                Assert.Greater(task.Result, 0);

                if(IsDebugEnabled)
                    log.Debug("Count = " + task.Result);
            }
        }

        [Test]
        public void Can_ExistsTask() {
            var countTasks = new List<Task<bool>>();

            foreach(var section in Sections) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryKey);
                Assert.IsNotEmpty(query);
                countTasks.Add(NorthwindAdoRepository.ExistsAsync(query));
            }

            Task.WaitAll(countTasks.ToArray());
            countTasks.All(t => t.IsCompleted).Should().Be.True();

            foreach(var task in countTasks) {
                Assert.IsTrue(task.Result);
                if(IsDebugEnabled)
                    log.Debug("Exists = " + task.Result);
            }
        }

        [Test]
        public void Can_ExecuteProcedureTask() {
            var countTasks = new List<Task<IAdoParameter[]>>();

            var spNames = new string[] { "CustOrderHist", "CustOrderHist", "CustOrderHist" };

            foreach(var spName in spNames) {
                var task = NorthwindAdoRepository.ExecuteProcedureAsync(spName, CustomerTestParameter);
                countTasks.Add(task);
            }

            Task.WaitAll(countTasks.ToArray());
            countTasks.All(t => t.IsCompleted).Should().Be.True();

            foreach(var task in countTasks) {
                if(IsDebugEnabled)
                    log.Debug("Results= " + task.Result.CollectionToString());
            }
        }

        [Test, Combinatorial]
        public void Can_ExecuteReaderMapTask([Values(0, 1, 2, 3, 4, 5)] int pageIndex, [Values(10, 20)] int pageSize) {
            var orderDetailsTask =
                NorthwindAdoRepository
                    .ExecuteInstanceAsync<OrderDetail>(TrimMapper,
                                                       SQL_ORDER_DETAIL_SELECT,
                                                       pageIndex,
                                                       pageSize);
            orderDetailsTask.Wait();

            orderDetailsTask.IsCompleted.Should().Be.True();
            orderDetailsTask.Result.Count.Should().Be.GreaterThan(0);
        }
    }
}