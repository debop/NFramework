using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Cache;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.Data.MongoDB.NHCaches {
    /// <summary>
    /// MongoDB 를 캐시 저장소로 이용하는 캐시입니다.
    /// </summary>
    public class MongoCacheClient : NHibernate.Cache.ICache {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        public MongoCacheClient() : this(SR.DefaultRegionName, null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="regionName"></param>
        public MongoCacheClient(string regionName) : this(regionName, null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="props"></param>
        public MongoCacheClient(string regionName, IDictionary<string, string> props) {
            Region = regionName;
            Configure(props);
        }

        #region << 환경 설정 정보 >>

        /// <summary>
        /// 캐시 영역
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// MongoDB ConnectionString (예: server=localhost;database=nhcache;safe=true; )
        /// </summary>
        /// <seealso cref="MongoTool.DefaultConnectionString"/>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// 기본 유효기간
        /// </summary>
        public TimeSpan Expiry { get; private set; }

        /// <summary>
        /// 압축 수행을 위한 최소 크기 (byte)
        /// </summary>
        public int CompressThreshold { get; private set; }

        private void Configure(IDictionary<string, string> props) {
            ConnectionString = SR.DefaultConnectionString;
            Expiry = SR.DefaultExpiry;
            CompressThreshold = SR.DefaultCompressThreshold;

            if(props == null) {
                if(log.IsWarnEnabled)
                    log.Warn("환경설정 값이 없어 기본값으로 설정합니다.");

                return;
            }

            ConnectionString = GetConnectionString(props);
            Expiry = GetExpiration(props);
            CompressThreshold = GetCompressThreshold(props);
        }

        private static string GetConnectionString(IDictionary<string, string> props) {
            string result;

            if(props.TryGetValue(SR.AttrConnectionString, out result)) {
                if(IsDebugEnabled)
                    log.Debug("새로운 connectionString=[{0}]", result);
            }
            else {
                result = SR.DefaultConnectionString;

                if(IsDebugEnabled)
                    log.Debug("connectionString이 제공되지 않아 기본 server을 사용합니다. 기본 connectionString=[{0}]", result);
            }
            return result;
        }

        private static TimeSpan GetExpiration(IDictionary<string, string> props) {
            TimeSpan result;
            string expirationString;

            if(props.TryGetValue(SR.AttrExpiration, out expirationString) == false) {
                props.TryGetValue(NHibernate.Cfg.Environment.CacheDefaultExpiration, out expirationString);
            }

            if(expirationString != null) {
                result = With.TryFunction(() => TimeSpan.Parse(expirationString), () => SR.DefaultExpiry);

                if(IsDebugEnabled)
                    log.Debug("새로운 expiration value=[{0}] (seconds)", result.TotalSeconds);
            }
            else {
                result = SR.DefaultExpiry;

                if(IsDebugEnabled)
                    log.Debug("Expiration를 지정하지 않아서, 기본 값을 사용합니다. default expiration=[{0}] (seconds)", result);
            }
            return result;
        }

        private static int GetCompressThreshold(IDictionary<string, string> props) {
            int result;
            string resultString;

            if(props.TryGetValue(SR.AttrCompressThreshold, out resultString)) {
                if(IsDebugEnabled)
                    log.Debug("new compressThreshold=[{0}] (bytes)", resultString);

                result = resultString.AsInt(SR.DefaultCompressThreshold);
            }
            else {
                result = SR.DefaultCompressThreshold;

                if(IsDebugEnabled)
                    log.Debug("compressThreshold를 지정하지 않아서, 기본 값을 사용합니다. DefaultCompressThreshold=[{0}] (bytes)", result);
            }
            return result;
        }

        #endregion

        private IMongoRepository _repository;

        public IMongoRepository Repository {
            get {
                if(_repository == null)
                    lock(_syncLock)
                        if(_repository == null) {
                            var repository = MongoTool.CreateRepository(ConnectionString);
                            Thread.MemoryBarrier();
                            _repository = repository;
                        }
                return _repository;
            }
        }

        #region << ICache >>

        /// <summary>
        /// Get the object from the Cache
        /// </summary>
        /// <param name="key"/>
        /// <returns/>
        public object Get(object key) {
            if(key == null)
                return null;

            var cacheId = GetCacheId(key);

            if(IsDebugEnabled)
                log.Debug("캐시에서 해당 키의 요소를 로드합니다... cacheId=[{0}]", cacheId);

            var cacheEntry = Repository.FindOneByIdAs<MongoCacheEntry>(cacheId);

            if(IsDebugEnabled)
                log.Debug("로드된 cacheEntry=[{0}]", cacheEntry);

            if(cacheEntry != null) {
                if(IsDebugEnabled)
                    log.Debug("LastUpdateDate=[{0}], Expiry=[{1}], ExpiredDate=[{2}], Now=[{3}]",
                              cacheEntry.LastUpdateDate, Expiry, cacheEntry.LastUpdateDate.Add(Expiry), DateTime.UtcNow);

                // 유효기간을 검사하여 삭제합니다.
                var isExpired = DateTime.UtcNow > cacheEntry.LastUpdateDate.Add(Expiry);
                if(isExpired) {
                    if(IsDebugEnabled)
                        log.Debug("해당 캐시 저장값의 유효기간이 만료되었습니다., 캐시에서 삭제하고 null을 반환합니다... key=[{0}]", key);

                    Remove(key);
                    return null;
                }

                if(IsDebugEnabled)
                    log.Debug("해당 캐시 저장값을 반환합니다!!! key=[{0}]", key);

                return DeserializeValue(cacheEntry);
            }

            return null;
        }

        /// <summary>
        /// 캐시에 값을 저장합니다.
        /// </summary>
        /// <param name="key">키</param>
        /// <param name="value">값</param>
        public void Put(object key, object value) {
            key.ShouldNotBeNull("key");
            value.ShouldNotBeNull("value");

            var cacheId = GetCacheId(key);

            if(IsDebugEnabled)
                log.Debug("지정된 키와 값으로 캐시에 추가 또는 갱신 작업을 시작합니다... cacheId=[{0}]", cacheId);

            var cacheEntry = Repository.FindOneByIdAs<MongoCacheEntry>(cacheId);

            if(cacheEntry == null) {
                if(IsDebugEnabled)
                    log.Debug("캐시된 값이 없으므로 새로 추가합니다...");
            }
            else {
                if(IsDebugEnabled)
                    log.Debug("캐시된 값을 갱신합니다... cacheId=[{0}]", cacheId);
            }
            cacheEntry = SerializeValue(key, value);

            Repository.Insert<MongoCacheEntry>(cacheEntry);
        }

        /// <summary>
        /// 캐시에서 해당 키의 캐시 항목을 삭제합니다.
        /// </summary>
        /// <param name="key">The Id of the Item in the Cache to remove.</param>
        /// <exception cref="T:NHibernate.Cache.CacheException"/>
        public void Remove(object key) {
            key.ShouldNotBeNull("key");

            var cacheId = GetCacheId(key);

            if(IsDebugEnabled)
                log.Debug("캐시에서 요소를 제거합니다... cacheKey=[{0}]", cacheId);

            Repository.RemoveByIdAs<CacheKey>(cacheId);
        }

        /// <summary>
        /// Clear the Cache
        /// </summary>
        /// <exception cref="T:NHibernate.Cache.CacheException"/>
        public void Clear() {
            Repository.RemoveAll();
        }

        /// <summary>
        /// Clean up.
        /// </summary>
        /// <exception cref="T:NHibernate.Cache.CacheException"/>
        public void Destroy() {
            Clear();
        }

        /// <summary>
        /// If this is a clustered cache, lock the item
        /// </summary>
        /// <param name="key">The Id of the Item in the Cache to lock.</param><exception cref="T:NHibernate.Cache.CacheException"/>
        public void Lock(object key) {
            // Nothing to do.
        }

        /// <summary>
        /// If this is a clustered cache, unlock the item
        /// </summary>
        /// <param name="key">The Id of the Item in the Cache to unlock.</param><exception cref="T:NHibernate.Cache.CacheException"/>
        public void Unlock(object key) {
            // nothing to do.
        }

        /// <summary>
        /// Generate a timestamp
        /// </summary>
        /// <returns/>
        public long NextTimestamp() {
            return Timestamper.Next();
        }

        /// <summary>
        /// Get a reasonable "lock timeout" (60 seconds)
        /// </summary>
        public int Timeout {
            get { return Timestamper.OneMs * 60000; }
        }

        /// <summary>
        /// Gets the name of the cache region
        /// </summary>
        public string RegionName {
            get { return Region; }
        }

        #endregion

        #region << Helper >>

        /// <summary>
        /// 캐시 저장 시의 고유 Id값을 생성합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetCacheId(object key) {
            return string.Format("{0}:{1}@{2}", SR.CacheKeyPrefix, Region, key);
        }

        private static ICompressor Compressor {
            get { return SingletonTool<SharpGZipCompressor>.Instance; }
        }

        private static ISerializer Serializer {
            get { return SingletonTool<BinarySerializer>.Instance; }
        }

        /// <summary>
        /// 객체를 캐시에 저장하기 위해, <see cref="MongoCacheEntry"/> 인스턴스로 빌드합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private MongoCacheEntry SerializeValue(object key, object value) {
            var entry = new MongoCacheEntry(GetCacheId(key), null);

            var serialized = Serializer.Serialize(value);
            if(serialized.Length > CompressThreshold) {
                serialized = Compressor.Compress(serialized);
                entry.IsCompressed = true;
            }
            entry.Value = serialized;
            return entry;
        }

        /// <summary>
        /// 캐시에 저장된 <see cref="MongoCacheEntry"/>에서 원본 값을 추출합니다.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static object DeserializeValue(MongoCacheEntry entry) {
            entry.ShouldNotBeNull("entry");

            byte[] value = entry.Value;

            if(value == null)
                return null;

            if(entry.IsCompressed)
                value = Compressor.Decompress(value);

            return Serializer.Deserialize(value);
        }

        #endregion
    }
}