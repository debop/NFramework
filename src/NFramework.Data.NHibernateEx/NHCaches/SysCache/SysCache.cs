using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using NHibernate.Cache;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.NHCaches.SysCache {
    /// <summary>
    /// .NET System의 메모리 Cache 시스템을 이용한 Cache 입니다.
    /// </summary>
    public sealed class SysCache : ICache {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string DefaultRegion = "realweb-cache";
        private static readonly object _syncLock = new object();

        private readonly Cache _cache;
        private string _region;
        private string _regionPrefix;
        private TimeSpan _expiration;
        private CacheItemPriority _priority;

        private readonly string _rootCacheKey;
        private bool _rootCacheKeyStored;

        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromSeconds(300);
        private static readonly string DefaultRegionPrefix = string.Empty;
        private const string CacheKeyPrefix = @"nhibernate-cache:";

        public SysCache() : this(DefaultRegion, null) {}
        public SysCache(string region) : this(region, null) {}

        public SysCache(string region, IDictionary<string, string> properties) {
            if(IsDebugEnabled)
                log.Debug("SysCache를 새로 생성합니다. region=[{0}], properties=[{1}]", region, properties.DictionaryToString());

            _region = region ?? DefaultRegion;
            _cache = HttpRuntime.Cache;

            Configure(properties);

            _rootCacheKey = GenerateRootCacheKey();
            StoreRootCacheKey();
        }

        public string Region {
            get { return _region ?? (_region = DefaultRegion); }
        }

        public string RegionPrefix {
            get { return _regionPrefix; }
        }

        public TimeSpan Expiration {
            get { return _expiration; }
        }

        public CacheItemPriority Priority {
            get { return _priority; }
        }

        private void Configure(IDictionary<string, string> props) {
            if(props == null) {
                if(IsDebugEnabled)
                    log.Debug("캐시의 환경 설정 값을 기본값으로 설정합니다...");

                _expiration = DefaultExpiration;
                _priority = CacheItemPriority.Default;
                _regionPrefix = DefaultRegionPrefix;
            }
            else {
                _priority = props.GetValueOrDefault("priority", "4").AsEnum(CacheItemPriority.Default);
                _expiration = TimeSpan.FromSeconds(Math.Max(60, props.GetValueOrDefault("expiration", "300").AsInt(300)));
                _regionPrefix = props.GetValueOrDefault("regionPrefix", string.Empty).AsText(DefaultRegionPrefix);
            }
        }

        private string GetCacheKey(object key) {
            return string.Format("{0}{1}{2}@{3}", CacheKeyPrefix, RegionPrefix, Region, key);
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

            var cacheKey = GetCacheKey(key);

            if(IsDebugEnabled)
                log.Debug("캐시에 저장된 정보를 로드합니다. key=[{0}], cacheKey=[{1}]", key, cacheKey);

            object item = _cache.Get(cacheKey);

            if(item != null) {
                return With.TryFunction<object>(() => {
                                                    var entry = (DictionaryEntry)item;
                                                    if(key.Equals(entry.Key))
                                                        return entry.Value;

                                                    return item;
                                                });
            }
            return item;
        }

        /// <summary/>
        /// <param name="key"/><param name="value"/>
        public void Put(object key, object value) {
            key.ShouldNotBeNull("key");
            value.ShouldNotBeNull("value");

            var cacheKey = GetCacheKey(key);


            if(_cache[cacheKey] != null) {
                if(IsDebugEnabled)
                    log.Debug("Remove cache item... key=[{0}], cacheKey=[{1}]", key, cacheKey);

                _cache.Remove(cacheKey);
            }
            else {
                if(IsDebugEnabled)
                    log.Debug("Add new cache item...  key=[{0}], cacheKey=[{1}]", key, cacheKey);
            }

            if(_rootCacheKeyStored == false)
                StoreRootCacheKey();

            _cache.Add(cacheKey,
                       new DictionaryEntry(key, value),
                       new CacheDependency(null, new[] { _rootCacheKey }),
                       Cache.NoAbsoluteExpiration,
                       _expiration,
                       _priority,
                       null);
        }

        /// <summary>
        /// Remove an item from the Cache.
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to remove.</param><exception cref="T:NHibernate.Cache.CacheException"/>
        public void Remove(object key) {
            key.ShouldNotBeNull("key");

            var cacheKey = GetCacheKey(key);

            if(IsDebugEnabled)
                log.Debug("Remove cache item... key=[{0}], cacheKey=[{1}]", key, cacheKey);

            _cache.Remove(cacheKey);
        }

        /// <summary>
        /// Clear the Cache
        /// </summary>
        /// <exception cref="T:NHibernate.Cache.CacheException"/>
        public void Clear() {
            lock(_syncLock) {
                RemoveRootCacheKey();
                StoreRootCacheKey();
            }
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
            // nothing to do.
        }

        /// <summary>
        /// If this is a clustered cache, unlock the item
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to unlock.</param><exception cref="T:NHibernate.Cache.CacheException"/>
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
            get { return _region; }
        }

        #endregion

        private string GenerateRootCacheKey() {
            return GetCacheKey(Guid.NewGuid());
        }

        private void RemoveRootCacheKey() {
            lock(_syncLock)
                _cache.Remove(_rootCacheKey);
        }

        private void RootCacheItemRemoved(string key, object value, CacheItemRemovedReason reason) {
            _rootCacheKeyStored = false;
        }

        private void StoreRootCacheKey() {
            _rootCacheKeyStored = true;

            _cache.Add(_rootCacheKey,
                       _rootCacheKey,
                       null,
                       Cache.NoAbsoluteExpiration,
                       Cache.NoSlidingExpiration,
                       CacheItemPriority.Normal,
                       RootCacheItemRemoved);
        }
    }
}