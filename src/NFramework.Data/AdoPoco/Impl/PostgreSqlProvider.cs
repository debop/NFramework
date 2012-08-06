using System.Data.Common;

namespace NSoft.NFramework.Data.AdoPoco.Impl {
    public class PostgreSqlProvider : AdoProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public PostgreSqlProvider(DbConnection connection) : base(connection) {}
        public PostgreSqlProvider(string connectionString, string providerName) : base(connectionString, providerName) {}
        public PostgreSqlProvider(string connectionString, DbProviderFactory providerFactory) : base(connectionString, providerFactory) {}
    }
}