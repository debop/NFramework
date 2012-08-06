using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Repositories {
    [TestFixture]
    public class AdoQueryStringProviderFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void Can_GetQueries_From_QueryStringProvider() {
            var table = NorthwindAdoRepository.QueryProvider.GetQueries();

            Assert.IsNotNull(table);
            Assert.Greater(table.Keys.Count, 0);

            table.Keys.AsEnumerable().All(key => table[key].IsNotWhiteSpace());

            foreach(var key in table.Keys)
                Assert.IsNotEmpty(table[key]);

            //foreach (string key in table.Keys)
            //    if (IsDebugEnabled)
            //        log.Debug("{0} = {1}", key, table[key]);
        }

        [Test]
        public void ExecuteDataSet() {
            var query = NorthwindAdoRepository.QueryProvider.GetQuery("Order Details, GetAll");
            using(var ds = NorthwindAdoRepository.ExecuteDataSetBySqlString(query)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
            }
        }

        [Test]
        public void ExecuteDataSetWithParameter() {
            var query = NorthwindAdoRepository.QueryProvider.GetQuery("Order Details, GetByOrder");
            using(var ds = NorthwindAdoRepository.ExecuteDataSetBySqlString(query, base.OrderTestParameter)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
            }
        }

        [Test]
        public void ExecuteDataSetByProcedure() {
            var spName = NorthwindAdoRepository.QueryProvider.GetQuery("Order, CustomerOrderHistory");
            using(var cmd = NorthwindAdoRepository.GetProcedureCommand(spName)) {
                using(var ds = NorthwindAdoRepository.ExecuteDataSet(cmd, CustomerTestParameter)) {
                    Assert.AreEqual(ds.Tables.Count, 1);
                    Assert.IsFalse(ds.Tables[0].HasErrors);
                    Assert.Greater(ds.Tables[0].Rows.Count, 0);
                }
            }

            spName = NorthwindAdoRepository.QueryProvider.GetQuery("CustomerOrdersDetail");
            using(var ds = NorthwindAdoRepository.ExecuteDataSetByProcedure(spName, OrderTestParameter)) {
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.IsFalse(ds.Tables[0].HasErrors);
                Assert.Greater(ds.Tables[0].Rows.Count, 0);
            }
        }

        [Test]
        public void UseTransactionScope() {
            AdoWith.TransactionScope(ExecuteDataSet,
                                     ExecuteDataSetWithParameter,
                                     ExecuteDataSetByProcedure);

            var query = NorthwindAdoRepository.QueryProvider.GetQuery("Order Details, GetAll");
            AdoWith.TransactionScope(
                () => { using(NorthwindAdoRepository.ExecuteDataSetBySqlString(query)) {} },
                () => { }
                );
        }

        [Test]
        public void ExecuteInParallel() {
            Parallel.Invoke(ExecuteDataSet,
                            ExecuteDataSetWithParameter,
                            ExecuteDataSetByProcedure);
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(5,
                              Can_GetQueries_From_QueryStringProvider,
                              ExecuteDataSet,
                              ExecuteDataSetWithParameter,
                              ExecuteDataSetByProcedure,
                              UseTransactionScope,
                              ExecuteInParallel);
        }
    }
}