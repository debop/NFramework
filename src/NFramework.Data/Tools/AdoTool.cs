using System;
using System.Data;
using NLog;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// AdoRepository의 ExtensionMethods 
    /// </summary>
    public static partial class AdoTool {
        #region << logger >>

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// DataTable의 기본 이름 ("Table")
        /// </summary>
        // NOTE: 기본 TableName을 변경하면 UpdateDataSet에서 예외가 발생한다. 이유는 모르겠고... 이름없는 Table은 기본 이름으로 설정해줘야 한다.
        public const string DefaultTableName = @"Table";

        /// <summary>
        /// Command Parameter의 DataSource별 접두사
        /// </summary>
        public static readonly char[] PARAMETER_PREFIX = new[] { '@', ':', '?' };

        /// <summary>
        /// Command 객체 생성시, Data Source로부터 Parameter 정보를 찾아낼 것인가. (기본=True)
        /// </summary>
        public const bool DEFAULT_DISCOVER_PARAMETER = true;

        /// <summary>
        /// Paging 시 기본 PageSize (10)
        /// </summary>
        public const int DefaultPageSize = 10;

        private static readonly object _syncLock = new object();

        /// <summary>
        /// <paramref name="repository"/>에 해당하는 Database에 연결이 제대뢰 되는지 확인합니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static bool Ping(this IAdoRepository repository) {
            repository.ShouldNotBeNull("repository");

            if(IsDebugEnabled)
                log.Debug("Database[{0}] 에 연결을 시도합니다...", repository.DbName);

            try {
                using(var conn = repository.Db.CreateConnection()) {
                    conn.Open();
                    return (conn.State == ConnectionState.Open);
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Database[{0}]에 접속할 수 없습니다.", repository.DbName);
                    log.Error(ex);
                }
            }
            return false;
        }
    }
}