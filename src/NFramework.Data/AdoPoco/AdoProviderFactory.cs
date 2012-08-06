using System;
using System.Data.Common;
using NSoft.NFramework.Data.AdoPoco.Impl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// <see cref="IAdoProvider"/> 인스턴스를 생성하는 Factory 클래스입니다.
    /// </summary>
    public static class AdoProviderFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static IAdoProvider CreateByName(string dbName) {
            if(IsDebugEnabled)
                log.Debug("IAdoProvider 구현 인스턴스를 생성합니다. dbName=[{0}]", dbName);

            var connSettings = ConfigTool.GetConnectionSettings(dbName);

            return Create(connSettings.ConnectionString, connSettings.ProviderName);
        }

        public static IAdoProvider Create(string connectionString, string providerName) {
            if(IsDebugEnabled)
                log.Debug("IAdoProvider 인스턴스를 생성합니다. connectionString=[{0}], providerName=[{1}]", connectionString, providerName);

            var factory = DbProviderFactories.GetFactory(providerName);

            if(factory == null)
                throw new NotSupportedException("지원하지 않는 Database Provider입니다. providerName=" + providerName);

            var factoryName = factory.GetType().Name;

            switch(factoryName.ToLower()) {
                case "mysql":
                    return new MySqlProvider(connectionString, providerName);
                case "sqlce":
                    return new SqlCeProvider(connectionString, providerName);
                case "npgsql":
                    return new PostgreSqlProvider(connectionString, providerName);
                case "oracle":
                    return new OracleProvider(connectionString, providerName);
                case "sqlite":
                    return new SqLiteProvider(connectionString, providerName);
                default:
                    return new SqlServerProvider(connectionString, providerName);
            }
        }
    }
}