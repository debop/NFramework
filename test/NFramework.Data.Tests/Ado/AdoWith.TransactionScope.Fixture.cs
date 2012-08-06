using System;
using System.Threading;
using System.Transactions;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data {
    [TestFixture]
    public class AdoWithTransactionScopeFixture : AdoFixtureBase {
        [TearDown]
        public void CleanUp() {
            NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_DELETE);
        }

        public static void SelectRegions() {
            using(var table = NorthwindAdoRepository.ExecuteDataTableBySqlString(SQL_REGION_SELECT)) {
                Assert.Greater(table.Rows.Count, 0);
            }
        }

        public static int TotalRows() {
            var result = NorthwindAdoRepository.ExecuteScalarBySqlString(SQL_REGION_COUNT);
            return Convert.ToInt32(result);
        }

        [Test]
        [Repeat(5)]
        public void TransactionScope_ShouldNotPromoteToDTC() {
            int totalRows = TotalRows();
            int dtcCount = 0;

            TransactionManager.DistributedTransactionStarted += delegate { Interlocked.Increment(ref dtcCount); };

            using(var ts = AdoTool.CreateTransactionScope()) {
                int rows = NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_INSERT);
                Assert.AreEqual(1, rows);
                rows = NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_INSERT2);
                Assert.AreEqual(1, rows);

                ts.Complete();
            }
            Assert.AreEqual(totalRows + 2, TotalRows());
            Assert.AreEqual(0, dtcCount);
        }

        [Test]
        public void TransactionScope_ShouldNotPromoteToDTC2() {
            TestTool.RunTasks(10,
                              () => {
                                  using(var txScope = AdoTool.CreateTransactionScope()) {
                                      var count = TotalCount();

                                      var ds = NorthwindAdoRepository.ExecuteDataSetBySqlString(SQL_REGION_SELECT, 1, 10);
                                      var dt = NorthwindAdoRepository.ExecuteDataTableBySqlString(SQL_REGION_SELECT, 1, 10);

                                      Assert.IsFalse(ds.Tables[0].HasErrors);
                                      Assert.IsFalse(dt.HasErrors);

                                      int rows = NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_INSERT);
                                      Assert.AreEqual(1, rows);
                                      rows = NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_INSERT2);
                                      Assert.AreEqual(1, rows);

                                      NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_DELETE);

                                      ds = NorthwindAdoRepository.ExecuteDataSetBySqlString(SQL_REGION_SELECT);
                                      dt = NorthwindAdoRepository.ExecuteDataTableBySqlString(SQL_REGION_SELECT);

                                      txScope.Complete();
                                  }
                              });
        }

        /// <summary>
        /// 하나의 Transaction Scope 안에서 Method들을 실행한다. 
        /// </summary>
        /// <remarks>
        /// NOTE : SQL_REGION_SELECT 에 WITH (NOLOCK)이 없으면 멀티 Thread에서 DeadLock 이 걸린다
        /// </remarks>
        [Test]
        public void TransactionScope_ShouldNotPromoteToDTC3() {
            TestTool.RunTasks(10,
                              () => AdoWith.TransactionScope(TransactionScopeOption.Required,
                                                             System.Transactions.IsolationLevel.ReadCommitted,
                                                             delegate { var count = TotalCount(); },
                                                             delegate {
                                                                 NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_INSERT);
                                                                 NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_INSERT2);
                                                                 NorthwindAdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_DELETE);
                                                             },
                                                             delegate {
                                                                 var ds =
                                                                     NorthwindAdoRepository.ExecuteDataSetBySqlString(
                                                                         SQL_REGION_SELECT, 1, 10);
                                                             },
                                                             delegate {
                                                                 var dt =
                                                                     NorthwindAdoRepository.ExecuteDataTableBySqlString(
                                                                         SQL_REGION_SELECT, 1, 10);
                                                             }
                                        ));
        }
    }
}