using System.Data.Common;

namespace NSoft.NFramework.Data.AdoPoco {
    public class SqLiteProvider : AdoProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public SqLiteProvider(DbConnection connection) : base(connection) {}
        public SqLiteProvider(string connectionString, string providerName) : base(connectionString, providerName) {}
        public SqLiteProvider(string connectionString, DbProviderFactory providerFactory) : base(connectionString, providerFactory) {}
    }
}