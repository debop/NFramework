using System.Data.Common;

namespace NSoft.NFramework.Data.AdoPoco {
    public class OracleProvider : AdoProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public OracleProvider(DbConnection connection) : base(connection) {}
        public OracleProvider(string connectionString, string providerName) : base(connectionString, providerName) {}
        public OracleProvider(string connectionString, DbProviderFactory providerFactory) : base(connectionString, providerFactory) {}
    }
}