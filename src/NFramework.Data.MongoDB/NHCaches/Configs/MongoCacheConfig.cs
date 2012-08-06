using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.MongoDB.NHCaches {
    /// <summary>
    /// Config properties
    /// </summary>
    [Serializable]
    public class MongoCacheConfig {
        private readonly IDictionary<string, string> _properties;

        public MongoCacheConfig(string region, string connectionString, string expiration, string compressThreshold) {
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
        /// MongoDB connection string (예: mongodb://localhost/NFramework_NH_Cache?strict=false) 
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