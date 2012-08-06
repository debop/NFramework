using System;

namespace NSoft.NFramework.Caching.Memcached.NHCaches {
    /// <summary>
    /// String Resources
    /// </summary>
    internal static class SR {
        internal const string DefaultRegionName = @"nhibernate";

        internal const string CacheKeyPrefix = @"NHibernate-Cache:";

        // 환경 설정을 위한 xml element 및 attribute name

        internal const string NodeMembaseCache = @"membaseCache";
        internal const string NodeCache = @"cache";

        internal const string AttrRegion = @"region";
        internal const string AttrExpiration = @"expiration";
        internal const string AttrCompressThreshold = @"compressThreshold";

        /// <summary>
        /// 캐시 유효기간 : 2분
        /// </summary>
        internal static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(2);

        internal const int DefaultCompressThreshold = 4096; // bytes
    }
}