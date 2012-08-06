using System.Data.Common;

namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// 
    /// </summary>
    public class SqlCeProvider : AdoProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public SqlCeProvider(DbConnection connection) : base(connection) {}
        public SqlCeProvider(string connectionString, string providerName) : base(connectionString, providerName) {}
        public SqlCeProvider(string connectionString, DbProviderFactory providerFactory) : base(connectionString, providerFactory) {}
    }
}