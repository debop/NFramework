using System.Data.Common;

namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerProvider : AdoProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public SqlServerProvider(DbConnection connection) : base(connection) {}
        public SqlServerProvider(string connectionString, string providerName) : base(connectionString, providerName) {}
        public SqlServerProvider(string connectionString, DbProviderFactory providerFactory) : base(connectionString, providerFactory) {}
    }
}