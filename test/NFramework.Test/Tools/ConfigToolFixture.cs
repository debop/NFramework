using System.Configuration;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class ConfigToolFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void Can_GetAppSettings() {
            Assert.IsTrue(ConfigTool.HasAppSettings("key1"));
            Assert.IsTrue(ConfigTool.HasAppSettings("key2"));
            Assert.IsNotEmpty(ConfigTool.GetAppSettings("key1", string.Empty));
            Assert.IsNotEmpty(ConfigTool.GetAppSettings("key2", string.Empty));

            if(IsDebugEnabled)
                log.Debug(ConfigTool.GetAppSettings("key1", string.Empty));

            // 키가 없으므로 string.Empty 를 반환할 것이다
            Assert.IsFalse(ConfigTool.HasAppSettings("unknown"));
            Assert.IsEmpty(ConfigTool.GetAppSettings("unknown", string.Empty));
        }

        [Test]
        public void Display_ConnectionStrings() {
            foreach(ConnectionStringSettings settings in ConfigurationManager.ConnectionStrings)
                if(IsDebugEnabled)
                    log.Debug("ConnectionString Name={0}, Value={1}, provider={2}", settings.Name, settings.ConnectionString,
                              settings.ProviderName);
        }

        [Test]
        public void Can_GetConnectionString() {
            foreach(var dbName in ConfigTool.GetDatabaseNames()) {
                if(dbName.StartsWith("Local"))
                    continue;

                Assert.IsTrue(ConfigTool.HasConnectionString(dbName));
                Assert.IsNotEmpty(ConfigTool.GetConnectionString(dbName));
            }
        }

        [Test]
        public void Can_GetConnectionStringSettings() {
            foreach(var connSettings in ConfigTool.GetConnectionStringSettings()) {
                Assert.IsNotNull(connSettings);
                Assert.IsNotEmpty(connSettings.Name);


                if(connSettings.Name.StartsWith("Local") == false)
                    Assert.IsNotEmpty(connSettings.ConnectionString);
            }
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(4,
                              Can_GetAppSettings,
                              Display_ConnectionStrings,
                              Can_GetConnectionString,
                              Can_GetConnectionStringSettings);
        }
    }
}