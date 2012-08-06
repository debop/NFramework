using System;

namespace NSoft.NFramework.Data.RavenDB.NHCaches {
    /// <summary>
    /// String Resources
    /// </summary>
    internal static class SR {
        internal const string DefaultRegionName = @"nhibernate";
        internal const string DefaultConnectionString = @"server=localhost;database=nhibernate-cache";

        internal const string CacheKeyPrefix = @"NHibernate-Cache:";

        // 환경 설정을 위한 xml element 및 attribute name
        internal const string NodeRavenCache = @"ravenCache";
        internal const string NodeCache = @"cache";

        internal const string AttrRegion = @"region";
        internal const string AttrConnectionString = @"connectionString";
        internal const string AttrExpiration = @"expiration";
        internal const string AttrCompressThreshold = @"compressThreshold";

        /// <summary>
        /// 캐시 유효기간 : 5분
        /// </summary>
        internal static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

        internal const int DefaultCompressThreshold = 40960; // bytes
    }
}