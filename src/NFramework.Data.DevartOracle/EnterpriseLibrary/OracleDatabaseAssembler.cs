using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration;

namespace NSoft.NFramework.Data.DevartOracle.EnterpriseLibrary {
    /// <summary>
    /// EntLib 4.1에서는 이 클래스를 정의해주어야 합니다.
    /// </summary>
    internal sealed class OracleDatabaseAssembler : IDatabaseAssembler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << IDatabaseAssembler >>

        /// <summary>
        /// Builds an instance of the concrete subtype of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.Database"/> 
        /// the receiver knows how to build, based on the provided connection string and any configuration information 
        /// that might be contained by the <paramref name="configurationSource"/>.
        /// </summary>
        /// <param name="name">The name for the new database instance.</param>
        /// <param name="connectionStringSettings">The connection string for the new database instance.</param>
        /// <param name="configurationSource">The source for any additional configuration information.</param>
        /// <returns>
        /// The new database instance.
        /// </returns>
        public Database Assemble(string name, ConnectionStringSettings connectionStringSettings,
                                 IConfigurationSource configurationSource) {
            if(IsDebugEnabled)
                log.Debug("EnterpriseLibrary용 OracleProvider 인스턴스를 빌드합니다... name=[{0}]", name);

            var settings = OracleConnectionSettings.GetSettings(configurationSource);

            if(settings != null) {
                var data = settings.OracleConnectionsData.Get(name);

                if(data != null) {
                    var packages = new IOraclePackage[data.Packages.Count];
                    var num = 0;

                    foreach(var package in data.Packages) {
                        packages[num++] = package;
                    }
                    return new OracleDatabase(connectionStringSettings.ConnectionString);
                }
            }

            if(IsDebugEnabled)
                log.Debug("OracleDatabase를 생성합니다. connectionString=[{0}]", connectionStringSettings.ConnectionString);

            return new OracleDatabase(connectionStringSettings.ConnectionString);
        }

        #endregion
    }
}