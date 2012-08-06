using System.Collections.Generic;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.QueryProviders {
    [TestFixture]
    public class IniAdoQueryProviderFixture : IoCSetUpBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public IIniQueryProvider Provider {
            get { return IoC.Resolve<IIniQueryProvider>("IniAdoQueryProvider.Northwind.SqlServer"); }
        }

        [Test]
        public void GetQueries() {
            Assert.IsNotNull(Provider);
            var table = Provider.GetQueries();

            Assert.IsNotNull(table);
            Assert.Greater(table.Count, 0);

            foreach(KeyValuePair<string, string> entry in table) {
                Assert.IsNotEmpty(entry.Value, "key=[{0}]의 값이 없습니다.", entry.Key);
            }
        }

        [Test]
        public void GetQuery() {
            Assert.IsNotNull(Provider);
            var table = Provider.GetQueries();

            Assert.IsNotNull(table);

            foreach(string key in table.Keys) {
                var query = Provider.GetQuery(key);
                Assert.IsNotEmpty(query);
            }
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(10, () => {
                                      GetQueries();
                                      GetQuery();
                                  });
        }
    }
}