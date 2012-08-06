using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.RavenDB.NHCaches {
    /// <summary>
    /// Raven Cache 설정 정보
    /// </summary>
    [Serializable]
    public class RavenCacheConfig {
        private readonly IDictionary<string, string> _properties;

        public RavenCacheConfig(string region, string connectionString, string expiration, string compressThreshold) {
            region.ShouldNotBeEmpty("region");
            connectionString.ShouldNotBeEmpty("connectionString");

            Region = region;
            _properties = new Dictionary<string, string>
                          {
                              { SR.AttrConnectionString, connectionString },
                              { SR.AttrExpiration, expiration },
                              { SR.AttrCompressThreshold, compressThreshold }
                          };
        }

        /// <summary>
        /// MongoDB connection string (예: mongodb://localhost/NFramework_Cache?strict=false) 
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// 부가 속성들
        /// </summary>
        public IDictionary<string, string> Properties {
            get { return _properties; }
        }
    }
}