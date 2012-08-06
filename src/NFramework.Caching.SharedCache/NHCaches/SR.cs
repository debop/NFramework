using System;

namespace NSoft.NFramework.Caching.SharedCache.NHCaches {
    /// <summary>
    /// SharedCache 시스템에 사용되는 String Resource 를 표현합니다.
    /// </summary>
    internal static class SR {
        internal const string DefaultRegionName = @"nhibernate";

        internal const string CacheKeyPrefix = @"NHibernate-Cache:";

        // 환경 설정을 위한 xml element 및 attribute name

        internal const string NodeSharedCache = @"sharedCache";
        internal const string NodeCache = @"cache";

        internal const string AttrRegion = @"region";
        //internal const string AttrConnectionString = @"connectionString";
        internal const string AttrExpiration = @"expiration";
        internal const string AttrCompressThreshold = @"compressThreshold";

        /// <summary>
        /// 캐시 유효기간 : 2시간
        /// </summary>
        internal static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(2);

        internal const int DefaultCompressThreshold = 40960; // bytes
    }
}