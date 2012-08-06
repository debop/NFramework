using System;

namespace NSoft.NFramework.Data.MongoDB {
    public static partial class MongoTool {
        public static bool FlushDatabase() {
            return ForceSync(DefaultConnectionString, false);
        }

        public static bool FlushDatabase(string server) {
            return ForceSync(server, false);
        }

        public static bool FlushDatabase(string server, bool isAsync) {
            return ForceSync(server, isAsync);
        }

        /// <summary>
        /// 보류중인 모든 Database 쓰기 작업에 대해, 완료를 수행한다. (Flush와 같은 개념이다)
        /// </summary>
        /// <returns></returns>
        public static bool ForceSync() {
            return ForceSync(DefaultConnectionString, false);
        }

        /// <summary>
        /// 보류중인 모든 Database 쓰기 작업에 대해, 완료를 수행한다. (Flush와 같은 개념이다)
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool ForceSync(string server) {
            return ForceSync(server, false);
        }

        /// <summary>
        /// 보류중인 모든 Database 쓰기 작업에 대해, 완료를 수행한다. (Flush와 같은 개념이다)
        /// </summary>
        /// <param name="server"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        public static bool ForceSync(string server, bool isAsync) {
            throw new NotImplementedException("구현 중");
        }
    }
}