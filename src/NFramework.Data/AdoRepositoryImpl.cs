namespace NSoft.NFramework.Data {
    /// <summary>
    /// <see cref="IAdoRepository"/>의 기본 Implementor
    /// </summary>
    public class AdoRepositoryImpl : AdoRepositoryImplBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        ///  Initialize AdoRepositoryImplBase with default database name that defined in App/Web.config DAAB configuration section
        /// </summary>
        public AdoRepositoryImpl() : base() {
            if(log.IsInfoEnabled)
                log.Info("DAAB를 이용한 AdoRepository를 생성합니다. dbName=[{0}]", AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// Initialize AdoRepositoryImplBase with database name.
        /// </summary>
        /// <param name="dbName">database name</param>
        public AdoRepositoryImpl(string dbName) : base(dbName) {
            if(log.IsInfoEnabled)
                log.Info("DAAB를 이용한 AdoRepository를 생성합니다. dbName=[{0}]", dbName);
        }
    }
}