using System.Configuration;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using NSoft.NFramework.Data.OdpNet.EnterpriseLibrary.Configuration;

namespace NSoft.NFramework.Data.OdpNet.EnterpriseLibrary {
    /// <summary>
    /// <see cref="OdpNetDatabase"/> 인스턴스를 빌드하기 위한 Assembler입니다.
    /// </summary>
    public class OdpNetDatabaseAssembler : IDatabaseAssembler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Builds an instance of the concrete subtype of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.Database"/> the receiver knows how to build, based on
        /// the provided connection string and any configuration information that might be contained by the
        /// <paramref name="configurationSource"/>.
        /// </summary>
        /// <param name="name">The name for the new database instance.</param>
        /// <param name="connectionStringSettings">The connection string for the new database instance.</param>
        /// <param name="configurationSource">The source for any additional configuration information.</param>
        /// <returns>The new database instance.</returns>
        public Database Assemble(string name, ConnectionStringSettings connectionStringSettings,
                                 IConfigurationSource configurationSource) {
            if(log.IsInfoEnabled)
                log.Info("OdpNetDatabase 인스턴스를 생성하기 위해 환경설정 정보를 읽습니다..." +
                         "name=[{0}], connectionStringSettings=[{1}], configurationSource=[{2}]",
                         name, connectionStringSettings.ConnectionString, configurationSource);

            var connectionSettings = OracleConnectionSettings.GetSettings(configurationSource);

            if(connectionSettings != null) {
                OracleConnectionData oraConnData = connectionSettings.OracleConnectionsData.Get(name);

                if(oraConnData != null) {
                    var packages = oraConnData.Packages.Cast<IOraclePackage>().ToArray();
                    return new OdpNetDatabase(connectionStringSettings.ConnectionString, packages);
                }
            }

            return new OdpNetDatabase(connectionStringSettings.ConnectionString);
        }
    }
}