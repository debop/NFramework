using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Caching.Memcached {
    public static class MemcachedTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly ConcurrentDictionary<string, MemcachedClient> _clientCache =
            new ConcurrentDictionary<string, MemcachedClient>();

        private static readonly object _syncLock = new object();

        private static readonly MemcachedClient _client = new MemcachedClient();

        /// <summary>
        /// 기본 <see cref="MemcachedClient"/>
        /// </summary>
        public static MemcachedClient Client {
            get { return _client; }
        }

        /// <summary>
        /// 지정한 Bucket명에 해당하는 Bucket과 통신하는 <see cref="MemcachedClient"/> 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static MemcachedClient GetClient(string sectionName = "default") {
            if(sectionName.IsWhiteSpace())
                return Client;

            sectionName = sectionName.AsText("default");

            if(IsDebugEnabled)
                log.Debug("MemcachedClient 를 생성하거나 캐시에 보관중인 것을 반환합니다... sectionName=[{0}]", sectionName);

            MemcachedClient client;

            if(_clientCache.TryGetValue(sectionName, out client) == false)
                lock(_syncLock)
                    if(_clientCache.TryGetValue(sectionName, out client) == false) {
                        client = new MemcachedClient(sectionName);
                        Thread.MemoryBarrier();
                        _clientCache.TryAdd(sectionName, client);

                        if(IsDebugEnabled)
                            log.Debug("새로운 MemcachedClient를 생성하고, 캐시에 저장합니다. sectionName=[{0}]", sectionName);
                    }

            return client;
        }

        /// <summary>
        /// 기본 유효기간 (1 시간) 
        /// </summary>
        public static TimeSpan DefaultExpiry {
            get { return DurationUtil.Hour; }
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <returns>저장된 캐시 값</returns>
        public static object GetValue(this MemcachedClient client, string key) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시 정보를 로드합니다. key=[{0}]", key);

            return client.Get(key);
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="newExpiresAt">새로운 유효기간</param>
        /// <returns>저장된 캐시 값</returns>
        public static object GetValue(this MemcachedClient client, string key, DateTime newExpiresAt) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시 정보를 로드합니다. key=[{0}], newExpiresAt=[{1}]", key, newExpiresAt);

            object value;

            if(client.TryGet(key, out value))
                client.SetValue(key, value, newExpiresAt);

            return value;
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <returns>저장된 캐시 값</returns>
        public static T GetValue<T>(this MemcachedClient client, string key) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시 정보를 로드합니다. key=[{0}]", key);

            return client.Get<T>(key);
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="newExpiresAt">새로운 유효기간</param>
        /// <returns>저장된 캐시 값</returns>
        public static T GetValue<T>(this MemcachedClient client, string key, DateTime newExpiresAt) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시 정보를 로드합니다. key=[{0}], newExpiresAt=[{1}]", key, newExpiresAt);

            T value;

            if(client.TryGetValue(key, out value))
                client.SetValue(key, value, newExpiresAt);

            return value;
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="keys">캐시 키</param>
        /// <returns>저장된 캐시 키-값의 Dictionary</returns>
        public static IDictionary<string, object> GetValue(this MemcachedClient client, IEnumerable<string> keys) {
            keys.ShouldNotBeNull("keys");

            if(IsDebugEnabled)
                log.Debug("캐시 정보를 로드합니다. keys=[{0}]", keys.CollectionToString());

            return client.Get(keys);
        }

        /// <summary>
        /// 지정된 정보를 캐시에 저장합니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="value">저장할 값</param>
        public static bool SetValue(this MemcachedClient client, string key, object value) {
            key.ShouldNotBeWhiteSpace("key");
            if(IsDebugEnabled)
                log.Debug("캐시 정보를 저장합니다. key=[{0}], value=[{1}]", key, value);

            return client.Store(StoreMode.Set, key, value);
        }

        /// <summary>
        /// 지정된 정보를 캐시에 저장합니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="value">저장할 값</param>
        /// <param name="expiresAt">삭제 시각</param>
        public static bool SetValue(this MemcachedClient client, string key, object value, DateTime expiresAt) {
            key.ShouldNotBeWhiteSpace("key");
            if(IsDebugEnabled)
                log.Debug("캐시 정보를 저장합니다. key=[{0}], value=[{1}], expiresAt=[{2}]", key, value, expiresAt);

            return client.Store(StoreMode.Set, key, value, expiresAt);
        }

        /// <summary>
        /// 지정된 정보를 캐시에 저장합니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="value">저장할 값</param>
        /// <param name="validFor">유효기간</param>
        public static bool SetValue(this MemcachedClient client, string key, object value, TimeSpan validFor) {
            key.ShouldNotBeWhiteSpace("key");
            if(IsDebugEnabled)
                log.Debug("캐시 정보를 저장합니다. key=[{0}], value=[{1}], validFor=[{2}]", key, value, validFor);

            return client.Store(StoreMode.Set, key, value, validFor);
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="value">저장된 값</param>
        /// <returns>조회 결과</returns>
        public static bool TryGetValue(this MemcachedClient client, string key, out object value) {
            return TryGetValue(client, key, null, out value);
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="newExpiresAt">새로운 유효기간</param>
        /// <param name="value">저장된 값</param>
        /// <returns>조회 결과</returns>
        public static bool TryGetValue(this MemcachedClient client, string key, DateTime? newExpiresAt, out object value) {
            key.ShouldNotBeWhiteSpace("key");
            if(IsDebugEnabled)
                log.Debug("캐시 정보를 로드합니다. key=[{0}]", key);

            var exists = client.TryGet(key, out value);

            if(exists)
                client.TrySetValue(key, value, newExpiresAt ?? DateTime.Now.Add(DefaultExpiry));

            return exists;
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">저장된 값의 수형</typeparam>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="value">저장된 값</param>
        /// <returns>조회 결과</returns>
        public static bool TryGetValue<T>(this MemcachedClient client, string key, out T value) {
            key.ShouldNotBeWhiteSpace("key");

            object storedValue;
            var result = TryGetValue(client, key, out storedValue);
            value = (result) ? (T)storedValue : default(T);
            return result;
        }

        /// <summary>
        /// 캐시에 저장된 정보를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">저장된 값의 수형</typeparam>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="newExpiresAt">새로운 유효기간</param>
        /// <param name="value">저장된 값</param>
        /// <returns>조회 결과</returns>
        public static bool TryGetValue<T>(this MemcachedClient client, string key, DateTime? newExpiresAt, out T value) {
            key.ShouldNotBeWhiteSpace("key");

            object storedValue;
            var result = TryGetValue(client, key, newExpiresAt, out storedValue);
            value = (result) ? (T)storedValue : default(T);
            return result;
        }

        /// <summary>
        /// 지정된 정보를 캐시에 저장합니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="value">저장할 값</param>
        public static bool TrySetValue(this MemcachedClient client, string key, object value) {
            key.ShouldNotBeWhiteSpace("key");

            try {
                return SetValue(client, key, value);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("정보를 Couchbase에 저장하는데, 실패했습니다.");
                    log.Error(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 지정된 정보를 캐시에 저장합니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="value">저장할 값</param>
        /// <param name="expiresAt">삭제 시각</param>
        public static bool TrySetValue(this MemcachedClient client, string key, object value, DateTime expiresAt) {
            key.ShouldNotBeWhiteSpace("key");

            try {
                return SetValue(client, key, value, expiresAt);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("정보를 Couchbase에 저장하는데, 실패했습니다.");
                    log.Error(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 지정된 정보를 캐시에 저장합니다.
        /// </summary>
        /// <param name="client">MemcachedClient 인스턴스</param>
        /// <param name="key">캐시 키</param>
        /// <param name="value">저장할 값</param>
        /// <param name="validFor">유효기간</param>
        public static bool TrySetValue(this MemcachedClient client, string key, object value, TimeSpan validFor) {
            key.ShouldNotBeWhiteSpace("key");

            try {
                return SetValue(client, key, value, validFor);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("정보를 Couchbase에 저장하는데, 실패했습니다.");
                    log.Error(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// <paramref name="client"/>에 해당하는 버킷의 모든 엔티티 정보를 삭제합니다.
        /// </summary>
        /// <param name="client"></param>
        public static void FlushAllItems(this MemcachedClient client) {
            if(IsDebugEnabled)
                log.Debug("모든 엔티티를 삭제합니다.");

            Client.FlushAll();
        }
    }
}