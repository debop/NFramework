namespace NSoft.NFramework.Data.RavenDB {
    public class RavenRepositoryImpl : IRavenRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public RavenRepositoryImpl() : this(null) {}

        public RavenRepositoryImpl(string connectionString) {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; protected set; }
    }
}