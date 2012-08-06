using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using SharedCache.WinServiceCommon;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace NSoft.NFramework.Caching.SharedCache {
    /// <summary>
    /// SharedCache를 위한 Extension Methods
    /// </summary>
    public static class SharedCacheTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 캐시된 요소의 갯수
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static long ItemCount(this IndexusProviderBase sharedCache, string serverName = null) {
            return serverName.IsWhiteSpace()
                       ? sharedCache.GetStats().ServiceAmountOfObjects
                       : sharedCache.GetStats(serverName).ServiceAmountOfObjects;
        }

        /// <summary>
        /// 캐시 서버의 요소가 차지하는 총 크기 (byte 단위)
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static long TotalSize(this IndexusProviderBase sharedCache, string serverName = null) {
            return serverName.IsWhiteSpace()
                       ? sharedCache.GetStats().ServiceTotalSize
                       : sharedCache.GetStats(serverName).ServiceTotalSize;
        }

        /// <summary>
        /// SharedCache에 해당 키에 해당하는 값이 존재하는지 확인합니다.
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ExistsKey(this IndexusProviderBase sharedCache, string key) {
            object item;
            return sharedCache.TryGet(key, out item);
        }

        /// <summary>
        /// 저장된 아이템의 Key들을 조회합니다.
        /// </summary>
        /// <param name="sharedCacke"></param>
        /// <param name="keyPredicate"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetKeys(this IndexusProviderBase sharedCacke, Func<string, bool> keyPredicate) {
            if(keyPredicate != null)
                return sharedCacke.GetAllKeys().Where(keyPredicate);
            return sharedCacke.GetAllKeys();
        }

        /// <summary>
        /// 저장된 아이템의 사용 횟수를 반환합니다. (key-사용횟수)
        /// </summary>
        /// <param name="sharedCacke"></param>
        /// <param name="keyPredicate">key filter</param>
        /// <param name="usagePredicate">usage filter (예: x=> x>0)</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, long>> ItemUsageList(this IndexusProviderBase sharedCacke,
                                                                            Func<string, bool> keyPredicate = null,
                                                                            Func<long, bool> usagePredicate = null) {
            var usageList = sharedCacke.GetStats().ServiceUsageList;

            if(usageList == null || usageList.Count == 0)
                return Enumerable.Empty<KeyValuePair<string, long>>();

            var query = usageList.AsEnumerable();

            if(keyPredicate != null)
                query = query.Where(x => keyPredicate(x.Key));

            if(usagePredicate != null)
                query = query.Where(x => usagePredicate(x.Value));

            return query;
        }

        /// <summary>
        /// SharedCache에 해당 키에 해당하는 값이 있으면 그 값을 반환하고, 없으면, <paramref name="valueFactory"/>로 값을 생성해서 값을 저장 후 그 값을 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sharedCache"></param>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static T GetOrAdd<T>(this IndexusProviderBase sharedCache, string key, Func<T> valueFactory) {
            sharedCache.ShouldNotBeNull("sharedCache");
            key.ShouldNotBeWhiteSpace("key");
            valueFactory.ShouldNotBeNull("valueFactory");

            if(IsDebugEnabled)
                log.Debug("SharedCache에서 Key[{0}]에 해당하는 값을 로드합니다.", key);

            T result;

            if(sharedCache.TryGet<T>(key, out result))
                return result;

            result = valueFactory();
            if(sharedCache.TryAdd(key, result))
                return result;

            throw new InvalidOperationException(string.Format("ShardCached에 Key[{0}]에 해당하는 값을 저장하는데 실패했습니다.", key));
        }

        /// <summary>
        /// 캐시 저장소에 특정 키의 값을 저장합니다.
        /// </summary>
        public static bool TryAdd(this IndexusProviderBase sharedCache, string key, object payload, DateTime? expires = null,
                                  IndexusMessage.CacheItemPriority? priority = null) {
            sharedCache.ShouldNotBeNull("sharedCache");
            key.ShouldNotBeWhiteSpace("key");

            return
                With.TryFunction(() => {
                                     if(IsDebugEnabled)
                                         log.Debug("SharedCache 시스템에 캐시 정보를 저장합니다... key=[{0}], payload=[{1}]", key, payload);

                                     sharedCache.Add(key,
                                                     payload,
                                                     expires.GetValueOrDefault(DateTime.Now.AddYears(1)),
                                                     priority.GetValueOrDefault(IndexusMessage.CacheItemPriority.Normal));

                                     if(IsDebugEnabled)
                                         log.Debug(
                                             "SharedCache 시스템에 캐시 정보를 저장했습니다!!! key=[{0}], payload=[{1}], expires=[{2}], priority=[{3}]",
                                             key, payload, expires, priority);

                                     return true;
                                 },
                                 () => false);
        }

        /// <summary>
        /// SharedCache에 특정 키의 값을 가져옵니다.
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGet(this IndexusProviderBase sharedCache, string key, out object value) {
            sharedCache.ShouldNotBeNull("sharedCache");
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("SharedCache 시스템에서 캐시 값을 로드합니다. key=[{0}]", key);

            try {
                value = sharedCache.Get(key);

                if(IsDebugEnabled)
                    log.Debug("SharedCache 시스템에서 캐시 값을 로드했습니다. key=[{0}], value=[{1}]", key, value);

                return true;
            }
            catch {
                value = null;
            }
            return false;
        }

        /// <summary>
        /// SharedCache에 특정 키의 값을 가져옵니다.
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGet<T>(this IndexusProviderBase sharedCache, string key, out T value) {
            sharedCache.ShouldNotBeNull("sharedCache");
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("SharedCache 시스템에서 캐시 값을 로드합니다... key=[{0}]", key);

            try {
                value = sharedCache.Get<T>(key);

                if(IsDebugEnabled)
                    log.Debug("SharedCache 시스템에서 캐시 값을 로드했습니다. key=[{0}], value=[{1}]", key, value);

                return true;
            }
            catch {
                value = default(T);
            }
            return false;
        }

        /// <summary>
        /// 지정된 키에 해당하는 Item을 삭제합니다.
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryRemove(this IndexusProviderBase sharedCache, string key) {
            sharedCache.ShouldNotBeNull("sharedCache");
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("SharedCache 시스템에서 해당 키의 값을 제거합니다... key=[{0}]", key);

            try {
                sharedCache.Remove(key);
                return true;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("해당 키의 캐시 값을 삭제하는데 실패했습니다. 무시합니다^^ key=[{0}]", key);
                    log.Warn(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 여러 개의 항목을 캐시에 추가합니다.
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool TryMultiAdd(this IndexusProviderBase sharedCache, IEnumerable<KeyValuePair<string, byte[]>> items) {
            if(items.IsEmptySequence())
                return false;

            try {
                if(IsDebugEnabled)
                    log.Debug("캐시 시스템에 다중 정보를 저장합니다... items key=[{0}]", items.Select(item => item.Key).CollectionToString());

                sharedCache.MultiAdd(items.ToDictionary(item => item.Key, item => item.Value));

                if(IsDebugEnabled)
                    log.Debug("캐시 시스템에 다중 정보를 저장했습니다!!!");

                return true;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("캐시에 여러 값을 저장하는데 실패했습니다. 예외는 무시합니다^^");
                    log.Warn(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 여러 개의 항목을 캐시에서 제거합니다.
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool TryMultiDelete(this IndexusProviderBase sharedCache, IEnumerable<string> keys) {
            if(keys.IsEmptySequence())
                return false;

            try {
                if(IsDebugEnabled)
                    log.Debug("캐시 시스템에 다중 정보를 삭제합니다... items key=[{0}]", keys.CollectionToString());

                sharedCache.MultiDelete(keys.ToList());

                if(IsDebugEnabled)
                    log.Debug("캐시 시스템에 다중 정보를 삭제했습니다!!!");

                return true;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("캐시에 여러 값을 삭제하는데 실패했습니다. 예외는 무시합니다^^");
                    log.Warn(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 여러 개의 항목을 캐시에서 가져옵니다.
        /// </summary>
        /// <param name="sharedCache"></param>
        /// <param name="keys"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool TryMultiGet(this IndexusProviderBase sharedCache, IEnumerable<string> keys,
                                       out IDictionary<string, byte[]> items) {
            items = null;
            if(keys.IsEmptySequence())
                return false;

            try {
                if(IsDebugEnabled)
                    log.Debug("캐시 시스템에서 다중 정보를 로드합니다... items key=[{0}]", keys.CollectionToString());

                items = sharedCache.MultiGet(keys.ToList());

                if(IsDebugEnabled)
                    log.Debug("캐시 시스템에 다중 정보를 로드했습니다!!!");

                return true;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("캐시에 여러 값을 로드하는데 실패했습니다. 예외는 무시합니다^^");
                    log.Warn(ex);
                }
            }
            return false;
        }
    }
}