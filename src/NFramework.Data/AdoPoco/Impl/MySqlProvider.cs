using System.Data.Common;

namespace NSoft.NFramework.Data.AdoPoco.Impl {
    public class MySqlProvider : AdoProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public MySqlProvider(DbConnection connection) : base(connection) {}

        public MySqlProvider(string connectionString, string providerName = "System.Data.SqlClient")
            : base(connectionString, providerName) {}

        public MySqlProvider(string connectionString, DbProviderFactory providerFactory) : base(connectionString, providerFactory) {}
    }
}