using System;
using System.Collections.Generic;
using NHibernate.Cache;

namespace NSoft.NFramework.Caching.Memcached.NHCaches {
    /// <summary>
    /// Memcached를 캐시 저장소로 사용하는 NHibernate 2nd Cache의 Client 클래스입니다.
    /// </summary>
    public class MemcachedCacheClient : NHibernate.Cache.ICache {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        internal const string CacheKeyPrefix = @"NHibernate-Cache:";

        public MemcachedCacheClient() : this("nhibernate", null) {}
        public MemcachedCacheClient(string regionName) : this(regionName, null) {}

        public MemcachedCacheClient(string regionName, IDictionary<string, string> properties) {
            Region = regionName.AsText(SR.DefaultRegionName);
            Configure(properties);
        }

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
                    log.Debug("새로운 expiration value=[{0}]", result);
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

        private ICacheRepository _repository;

        /// <summary>
        /// Memcached CacheRepository
        /// </summary>
        public ICacheRepository Repository {
            get { return _repository ?? (_repository = new MemcachedRepository()); }
            protected set { _repository = value; }
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

            return Repository.Get(GetCacheId(key));
        }

        /// <summary>
        /// 항목을 캐시에 저장
        /// </summary>
        /// <param name="key"/><param name="value"/>
        public void Put(object key, object value) {
            key.ShouldNotBeNull("key");
            value.ShouldNotBeNull("value");

            Repository.Set(GetCacheId(key), value, Expiry);
        }

        /// <summary>
        /// Remove an item from the Cache.
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to remove.</param><exception cref="T:NHibernate.Cache.CacheException"/>
        public void Remove(object key) {
            if(key == null)
                return;

            Repository.Remove(GetCacheId(key));
        }

        /// <summary>
        /// Clear the Cache
        /// </summary>
        /// <exception cref="T:NHibernate.Cache.CacheException"/>
        public void Clear() {
            Repository.Clear();
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
            get { return (int)Expiry.TotalMilliseconds; }
        }

        /// <summary>
        /// Gets the name of the cache region
        /// </summary>
        public string RegionName {
            get { return Region; }
        }

        #endregion

        private string GetCacheId(object key) {
            return string.Concat(CacheKeyPrefix, Region, ":", key);
        }
    }
}