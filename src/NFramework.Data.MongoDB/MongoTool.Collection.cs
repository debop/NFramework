using System;

namespace NSoft.NFramework.Data.MongoDB {
    public static partial class MongoTool {
        /// <summary>
        /// 지정된 수형의 Collection Name을 유추합니다.
        /// </summary>
        /// <returns></returns>
        public static string GetCollectionName(Type type) {
            return type.Name;
        }
    }
}