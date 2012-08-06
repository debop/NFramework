using System;
using Devart.Data.Oracle;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.DevartOracle.WebHost {
    public class DevartOracleHttpApplication : UnitOfWorkHttpApplication {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public override void Application_Start(object sender, EventArgs e) {
            base.Application_Start(sender, e);

            using(var conn = new OracleConnection(ConfigTool.GetConnectionString("LOCAL_XE"))) {
                log.Info("LOCAL_XE에 대한 OracleConnection을 빌드하고, 연결합니다...");

                conn.Open();
                conn.Ping();
            }
        }
    }
}