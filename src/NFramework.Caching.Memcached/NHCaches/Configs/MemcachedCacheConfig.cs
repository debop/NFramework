using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Caching.Memcached.NHCaches {
    /// <summary>
    /// Memcached Cache 에 대한 환경 설정 값을 나타냅니다.
    /// </summary>
    [Serializable]
    public class MemcachedCacheConfig {
        private IDictionary<string, string> _properties;

        public MemcachedCacheConfig(string region, string expiration, string compressThreshold) {
            Region = region ?? SR.DefaultRegionName;

            _properties = new Dictionary<string, string>
                          {
                              { SR.AttrExpiration, expiration },
                              { SR.AttrCompressThreshold, compressThreshold }
                          };
        }

        /// <summary>
        /// 캐시 영역 명
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// Cache 관련 속성들 (ConnectionString, Expiration 등)
        /// </summary>
        public IDictionary<string, string> Properties {
            get { return _properties ?? (_properties = new Dictionary<string, string>()); }
        }
    }
}