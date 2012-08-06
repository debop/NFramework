using System;
using System.Data;
using System.Text;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Repositories {
    /// <summary>
    /// IAdoRepository로 Expose하기 전에 먼저 테스트를 수행하기 위해
    /// </summary>
    [TestFixture]
    public class AdoRepositoryImplFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly object _syncLock = new object();

        private IAdoRepository _impl;

        public IAdoRepository AdoImpl {
            get {
                if(_impl == null)
                    lock(_syncLock)
                        _impl = AdoRepositoryFactory.Instance.CreateRepository();

                return _impl;
            }
        }

        /// <summary>
        /// ADO.NET Transaction을 이용하여 작업을 수행한다. DTC가 필요없다.
        /// </summary>
        [Test]
        public void Transaction_Without_DTC() {
            AdoImpl.BeginTransaction(IsolationLevel.ReadCommitted);
            try {
                Assert.IsTrue(AdoImpl.IsActiveTransaction);

                var countCommand = AdoImpl.GetSqlStringCommand(SQL_REGION_COUNT);
                var insertCommand = AdoImpl.GetSqlStringCommand(SQL_REGION_INSERT);
                var deleteCommand = AdoImpl.GetSqlStringCommand(SQL_REGION_DELETE);
                var selectCommand = AdoImpl.GetSqlStringCommand(SQL_REGION_SELECT);

                var count = AdoImpl.ExecuteScalar(countCommand).AsInt();

                AdoImpl.ExecuteNonQuery(insertCommand);
                AdoImpl.ExecuteNonQuery(deleteCommand);

                // NOTE : 이 메소드가 Tx 안에서 수행되려면 Query 문에 WITH (NOLOCK) 이 있어야 한다.
                var resultSet = AdoImpl.ExecuteDataTable(selectCommand);

                AdoImpl.Commit();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.Error(ex);

                AdoImpl.Rollback();

                throw;
            }
            Assert.IsFalse(AdoImpl.IsActiveTransaction);
        }

        [Test]
        public void Rollback_Test() {
            {
                var originalCount = AdoImpl.CountBySqlString(SQL_REGION_SELECT_SHARED_LOCK);

                AdoImpl.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    Assert.IsTrue(AdoImpl.IsActiveTransaction);

                    var insertCommand = AdoImpl.GetSqlStringCommand(SQL_REGION_INSERT);
                    var selectCommand = AdoImpl.GetSqlStringCommand(SQL_REGION_SELECT);

                    AdoImpl.ExecuteNonQuery(insertCommand);

                    // NOTE : 이 메소드가 Tx 안에서 수행되려면 Query 문에 WITH (NOLOCK) 이 있어야 한다.
                    var resultSet = AdoImpl.ExecuteDataTable(selectCommand);

                    // Rollback Test
                    AdoImpl.ExecuteNonQueryBySqlString(ErrorQueryString);

                    AdoImpl.Commit();
                }
                catch(Exception ex) {
                    AdoImpl.Rollback();

                    if(log.IsWarnEnabled)
                        log.WarnException("예외가 발생하여 Rollback 하였습니다.", ex);
                }

                Assert.IsFalse(AdoImpl.IsActiveTransaction);

                var rowCount = AdoImpl.CountBySqlString(SQL_REGION_SELECT_SHARED_LOCK);

                Assert.AreEqual(originalCount, rowCount);
            }
        }

        /// <summary>
        /// 여러개의 Query문을 한꺼번에 호출하여 하나의 DataSet에 로드한다.
        /// </summary>
        [Test]
        public void LoadDataSet_With_Multiple_Query() {
            const string query = SQL_REGION_SELECT + ";" + SQL_REGION_COUNT;

            using(var ds = new DataSet())
            using(var cmd = AdoImpl.GetSqlStringCommand(query)) {
                AdoImpl.LoadDataSet(cmd, ds, new[] { "Region", "CountOfRegion" });
                Assert.AreEqual(2, ds.Tables.Count);
                Assert.AreEqual(ds.Tables[0].Rows.Count, ds.Tables[1].Rows[0][0]);
            }
        }

        /// <summary>
        /// SP 여러 개를 한꺼번에 호출하여 하나의 DataSet에 로드한다.
        /// </summary>
        [Test]
        public void LoadDataSet_With_Multiple_Procedure() {
            var queryBuilder = new StringBuilder();

            queryBuilder
                .AppendFormat("EXEC CustOrderHist @CustomerID").Append(";")
                .AppendFormat("EXEC CustOrderHist2 @CustomerID").Append(";")
                .AppendFormat("EXEC CustOrdersOrders @CustomerID").Append(";");

            var ds = new DataSet();

            using(var scope = AdoTool.CreateTransactionScope()) {
                using(var cmd = AdoImpl.GetSqlStringCommand(queryBuilder.ToString(), CustomerTestParameter)) {
                    AdoImpl.LoadDataSet(cmd, ds, new[] { "CustOrderHist", "CustOrderHist2", "CustOrdersOrders" });
                    Assert.AreEqual(3, ds.Tables.Count);

                    foreach(DataTable dataTable in ds.Tables)
                        Assert.IsFalse(dataTable.HasErrors);
                }
                scope.Complete();
            }
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(2,
                              Transaction_Without_DTC,
                              Rollback_Test,
                              LoadDataSet_With_Multiple_Query,
                              LoadDataSet_With_Multiple_Procedure);
        }
    }
}