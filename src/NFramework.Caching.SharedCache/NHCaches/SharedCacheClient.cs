using System;
using System.Collections.Generic;
using NHibernate.Cache;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Serializations.Serializers;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace NSoft.NFramework.Caching.SharedCache.NHCaches {
    /// <summary>
    /// SharedCache 를 캐시 저장소로 사용하는 Cache Client입니다.
    /// 참고 : http://www.sharedcache.com/cms/
    /// </summary>
    public sealed class SharedCacheClient : ICache {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public SharedCacheClient() : this("nhibernate", null) {}

        public SharedCacheClient(string regionName) : this(regionName, null) {}

        public SharedCacheClient(string regionName, IDictionary<string, string> properties) {
            Region = regionName;
            Configure(properties);
        }

        #region << 환경 설정 정보 >>

        /// <summary>
        /// 캐시 영역
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// 기본 유효기간
        /// </summary>
        public TimeSpan Expiry { get; private set; }

        /// <summary>
        /// 압축 수행을 위한 최소 크기 (byte)
        /// </summary>
        public int CompressThreshold { get; private set; }

        private void Configure(IDictionary<string, string> props) {
            Expiry = SR.DefaultExpiry;
            CompressThreshold = SR.DefaultCompressThreshold;

            if(props == null) {
                if(log.IsWarnEnabled)
                    log.Warn("환경설정 값이 없어 기본값으로 설정합니다.");

                return;
            }

            Expiry = GetExpiration(props);
            CompressThreshold = GetCompressThreshold(props);
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
                    log.Debug("새로운 expiration=[{0}]", result);
            }
            else {
                result = SR.DefaultExpiry;

                if(IsDebugEnabled)
                    log.Debug("Expiration를 지정하지 않아서, 기본 값을 사용합니다. default expiration=[{0}]", result);
            }
            return result;
        }

        private static int GetCompressThreshold(IDictionary<string, string> props) {
            int result;
            string resultString;

            if(props.TryGetValue(SR.AttrCompressThreshold, out resultString)) {
                if(IsDebugEnabled)
                    log.Debug("new compressThreshold=[{0}]", resultString);

                result = resultString.AsInt(SR.DefaultCompressThreshold);
            }
            else {
                result = SR.DefaultCompressThreshold;

                if(IsDebugEnabled)
                    log.Debug("compressThreshold를 지정하지 않아서, 기본 값을 사용합니다. DefaultCompressThreshold=[{0}]", result);
            }
            return result;
        }

        #endregion

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

            SharedCacheEntry cacheEntry;
            if(IndexusDistributionCache.SharedCache.TryGet(cacheId, out cacheEntry)) {
                if(IsDebugEnabled)
                    log.Debug("저장된 캐시 엔트리 정보를 로드했습니다. cacheId=[{0}]", cacheId);

                return DeserializeValue(cacheEntry);
            }

            return null;
        }

        /// <summary/>
        /// <param name="key"/><param name="value"/>
        public void Put(object key, object value) {
            key.ShouldNotBeNull("key");
            value.ShouldNotBeNull("value");

            var cacheId = GetCacheId(key);

            if(IsDebugEnabled)
                log.Debug("지정된 키와 값으로 캐시에 추가 또는 갱신 작업을 시작합니다... cacheId=[{0}]", cacheId);

            SharedCacheEntry cacheEntry;

            if(IndexusDistributionCache.SharedCache.TryGet(cacheId, out cacheEntry) == false) {
                if(IsDebugEnabled)
                    log.Debug("캐시된 값이 없으므로 새로 추가합니다...");
            }
            else {
                if(IsDebugEnabled)
                    log.Debug("캐시된 값을 갱신합니다... cacheId=[{0}]", cacheId);
            }
            cacheEntry = SerializeValue(key, value);
            var saved = IndexusDistributionCache.SharedCache.TryAdd(cacheId, cacheEntry, DateTime.Now.Add(Expiry));


            if(IsDebugEnabled)
                log.Debug("지정된 키와 값으로 캐시에 추가 또는 갱신 작업을 수행했습니다. cacheId=[{0}], expiry=[{1}], 저장 결과=[{2}]", cacheId, Expiry, saved);
        }

        /// <summary>
        /// Remove an item from the Cache.
        /// </summary>
        /// <param name="key">The Id of the Item in the Cache to remove.</param><exception cref="T:NHibernate.Cache.CacheException"/>
        public void Remove(object key) {
            if(key == null)
                return;

            var cacheId = GetCacheId(key);

            if(IsDebugEnabled)
                log.Debug("캐시에서 요소를 제거합니다... cacheId=[{0}]", cacheId);

            IndexusDistributionCache.SharedCache.Remove(cacheId);
        }

        /// <summary>
        /// Clear the Cache
        /// </summary>
        /// <exception cref="T:NHibernate.Cache.CacheException"/>
        public void Clear() {
            if(IsDebugEnabled)
                log.Debug("캐시 정보를 모두 삭제합니다...");

            IndexusDistributionCache.SharedCache.Clear();
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
        /// <param name="key">The Key of the Item in the Cache to lock.</param><exception cref="T:NHibernate.Cache.CacheException"/>
        public void Lock(object key) {
            // Nothing to do.
        }

        /// <summary>
        /// If this is a clustered cache, unlock the item
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to unlock.</param><exception cref="T:NHibernate.Cache.CacheException"/>
        public void Unlock(object key) {
            // Nothing to do.
        }

        /// <summary>
        /// Generate a timestamp
        /// </summary>
        /// <returns/>
        public long NextTimestamp() {
            return Timestamper.Next();
        }

        /// <summary>
        /// Get a reasonable "lock timeout"
        /// </summary>
        public int Timeout {
            get { return Timestamper.OneMs * 60000; } // 60 seconds
        }

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
            return string.Concat(SR.CacheKeyPrefix, ":", Region, "@", key);
        }

        private static ICompressor Compressor {
            get { return SingletonTool<SharpGZipCompressor>.Instance; }
        }

        private static ISerializer Serializer {
            get { return SingletonTool<BinarySerializer>.Instance; }
        }

        /// <summary>
        /// 객체를 캐시에 저장하기 위해, <see cref="SharedCacheEntry"/> 인스턴스로 빌드합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private SharedCacheEntry SerializeValue(object key, object value) {
            var entry = new SharedCacheEntry(GetCacheId(key), null);

            var serialized = Serializer.Serialize(value);
            if(serialized.Length > CompressThreshold) {
                serialized = Compressor.Compress(serialized);
                entry.IsCompressed = true;
            }
            entry.Value = serialized;
            return entry;
        }

        /// <summary>
        /// 캐시에 저장된 <see cref="SharedCacheEntry"/>에서 원본 값을 추출합니다.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static object DeserializeValue(SharedCacheEntry entry) {
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