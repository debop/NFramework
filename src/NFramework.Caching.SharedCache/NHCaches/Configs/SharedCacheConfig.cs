using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Caching.SharedCache.NHCaches {
    [Serializable]
    public class SharedCacheConfig {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private IDictionary<string, string> _properties;

        public SharedCacheConfig(string region, string expiration, string compressThreshold) {
            region.ShouldNotBeEmpty("region");

            if(IsDebugEnabled)
                log.Debug("SharedCacheConfig 를 생성했습니다. region=[{0}], expiration=[{1}], compressThreshold=[{2}]",
                          region, expiration, compressThreshold);

            Region = region;

            Properties = new Dictionary<string, string>
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
            protected set { _properties = value; }
        }
    }
}