namespace NSoft.NFramework.Data.MongoDB {
    /// <summary>
    /// MongoDB 에 대한 Data 처리를 수행하는 Repository에 대한 Utility Class 입니다.
    /// </summary>
    public static partial class MongoRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 서버와의 접속이 제대로 되었는지 확인합니다.
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        public static void Ping(this IMongoRepository repository) {
            repository.Server.Ping();
        }
    }
}