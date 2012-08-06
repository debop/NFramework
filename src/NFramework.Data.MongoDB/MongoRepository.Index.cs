using MongoDB.Driver;

namespace NSoft.NFramework.Data.MongoDB {
    /// <summary>
    /// <see cref="IMongoRepository"/> 를 위한 확장 메소드를 제공합니다.
    /// </summary>
    public static partial class MongoRepository {
        /// <summary>
        /// 인덱스를 생성합니다.
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="keyNames">인덱스 명의 배열</param>
        public static void CreateIndex(this IMongoRepository repository, params string[] keyNames) {
            if(IsDebugEnabled)
                log.Debug("Collection[{0}]에 인덱스를 생성합니다. keyNames=[{1}]", repository.CollectionName, keyNames);

            repository.Collection.CreateIndex(keyNames);
        }

        /// <summary>
        /// 인덱스를 생성합니다.
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="keys">인덱스 키</param>
        public static void CreateIndex(this IMongoRepository repository, IMongoIndexKeys keys) {
            repository.Collection.CreateIndex(keys);
        }

        /// <summary>
        /// 인덱스를 생성합니다.
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="keys">인덱스 키</param>
        /// <param name="options">인덱스 옵션</param>
        public static void CreateIndex(this IMongoRepository repository, IMongoIndexKeys keys, IMongoIndexOptions options) {
            repository.Collection.CreateIndex(keys, options);
        }

        /// <summary>
        /// 인덱스 제거
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <returns>결과 정보 <see cref="CommandResult.Response"/>.ElementCount 가 삭제된 인덱스의 수입니다.</returns>
        public static CommandResult DropAllIndexes(this IMongoRepository repository) {
            return repository.Collection.DropAllIndexes();
        }

        /// <summary>
        /// 인덱스 제거
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="keys">인덱스 키</param>
        /// <returns></returns>
        public static CommandResult DropIndex(this IMongoRepository repository, IMongoIndexKeys keys) {
            return repository.Collection.DropIndex(keys);
        }

        /// <summary>
        /// 인덱스 제거
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="indexName">인덱스 명</param>
        /// <returns></returns>
        public static CommandResult DropIndexByName(this IMongoRepository repository, string indexName) {
            return repository.Collection.DropIndexByName(indexName);
        }

        /// <summary>
        /// 해당 인덱스가 존재하지 않으면 새로 생성합니다.
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="keyNames"></param>
        public static void EnsureIndex(this IMongoRepository repository, params string[] keyNames) {
            repository.Collection.EnsureIndex(keyNames);
        }

        /// <summary>
        /// 해당 인덱스가 존재하지 않으면 새로 생성합니다.
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="keys"></param>
        public static void EnsureIndex(this IMongoRepository repository, IMongoIndexKeys keys) {
            repository.Collection.EnsureIndex(keys);
        }

        /// <summary>
        /// 해당 인덱스가 존재하지 않으면 새로 생성합니다.
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="keys"></param>
        /// <param name="options"></param>
        public static void EnsureIndex(this IMongoRepository repository, IMongoIndexKeys keys, IMongoIndexOptions options) {
            repository.Collection.EnsureIndex(keys, options);
        }

        /// <summary>
        /// <paramref name="repository"/>의 현재 컬렉션의 모든 인덱스 정보를 가져옵니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static GetIndexesResult GetIndexes(this IMongoRepository repository) {
            return repository.Collection.GetIndexes();
        }

        /// <summary>
        /// 인덱스 존재 여부를 파악합니다.
        /// </summary>
        /// <param name="repository">MongRepository 인스턴스</param>
        /// <param name="indexName">인덱스 명</param>
        /// <returns></returns>
        public static bool IndexExistsByName(this IMongoRepository repository, string indexName) {
            return repository.Collection.IndexExistsByName(indexName);
        }
    }
}