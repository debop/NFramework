using System;
using MongoDB.Driver;

namespace NSoft.NFramework.Data.MongoDB {
    /// <summary>
    /// Mongo DB 관련 Tool 입니다.
    /// </summary>
    public static partial class MongoTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 서버 명 (localhost)
        /// </summary>
        public const string DefaultConnectionString = @"server=localhost;database=default;safe=true;";

        /// <summary>
        /// 기본 Database 명 (default)
        /// </summary>
        public const string DefaultDatabaseName = "default";

        /// <summary>
        /// 기본 Collection 명 (defaultColl)
        /// </summary>
        public const string DefaultCollectionName = "defaultColl";

        /// <summary>
        /// 시스템 인덱스를 보관하는 컬렉션 명 (system.indexes)
        /// </summary>
        public const string SystemIndexesCollectionName = "system.indexes";

        /// <summary>
        /// MongoDB ConnectionString Format (ex: "mongodb://localhost/?safe=true")
        /// </summary>
        public const string ConnectionStringFormat = @"server={0};database={1}";

        /// <summary>
        /// Document Id 명 ("_id")
        /// </summary>
        public const string IdString = "_id";

        public const string IdIndexName = "_id_";

        /// <summary>
        /// 비어있는 <see cref="CommandResult"/>입니다. 
        /// </summary>
        public static readonly CommandResult EmptyCommandResult = new CommandResult();

        private static readonly object _syncLock = new object();

        /// <summary>
        /// MongoDB 가 JSON 형식에서 DataTime 을 내부적으로 double이 아닌 long을 변경해서 저장하므로, .NET DateTime과 오차가 생길 수 있다.
        /// MongoDB에 저장된 정보 중 DateTime에 대한 비교는 꼭 ToMongoDateTime() 이용해서 DateTime을 변경한 후 비교해야 합니다.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToMongoDateTime(this DateTime dateTime) {
            return dateTime.AddTicks(-(dateTime.Ticks % 10000));
        }
    }
}