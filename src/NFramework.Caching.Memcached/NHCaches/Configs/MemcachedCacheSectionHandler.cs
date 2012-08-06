using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using NSoft.NFramework.Xml;

namespace NSoft.NFramework.Caching.Memcached.NHCaches {
    /// <summary>
    /// <see cref="MemcachedCacheClient"/>를 위한 환경설정 정보를 Configuration 파일에서 읽어서 반환합니다.
    /// </summary>
    /// <example>
    /// <code>
    ///		<configSections>
    /// 		<section name="memcachedCache" type="NSoft.NFramework.Caching.Memcached.NHCaches.MemcachedCacheSectionHandler, NSoft.NFramework.Caching.Memcached" />
    ///		</configSections>
    /// 
    ///		<membaseCache>
    /// 		<!-- region 은 Unique 해야 합니다. expiration: Timespan , compressThreshold:byte단위-->
    /// 		<cache region="NSoft.NFramework" expiration="01:00:00" compressThreshold="40960"/>
    /// 		<cache region="NAccounts" expiration="01:00:00" compressThreshold="40960"/>
    ///		</membaseCache>
    /// </code>
    /// </example>
    public class MemcachedCacheSectionHandler : IConfigurationSectionHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 구성 섹션 처리기를 만듭니다.
        /// </summary>
        /// <returns>
        /// 만들어진 섹션 처리기 개체입니다.
        /// </returns>
        /// <param name="parent">부모 개체입니다.</param><param name="configContext">구성 컨텍스트 개체입니다.</param>
        /// <param name="section">섹션 XML 노드입니다.</param><filterpriority>2</filterpriority>
        public object Create(object parent, object configContext, XmlNode section) {
            if(log.IsInfoEnabled)
                log.Info("Couchbase를 NHibernate 2nd 캐시로 사용하기 위해, 환경설정 값을 로드합니다...");

            var cacheConfigs = new List<MemcachedCacheConfig>();

            if(section == null)
                return cacheConfigs.ToArray();

            var nodes = section.SelectNodes(SR.NodeCache);

            if(nodes == null)
                return cacheConfigs.ToArray();

            foreach(XmlElement node in nodes) {
                var region = node.AttributeToString(SR.AttrRegion, SR.DefaultRegionName);
                var expiration = node.AttributeToString(SR.AttrExpiration, SR.DefaultExpiry.ToString());
                var compressThreshold = node.AttributeToString(SR.AttrCompressThreshold, SR.DefaultCompressThreshold.ToString());

                if(IsDebugEnabled)
                    log.Debug("Couchbase를 이용한 NHibernate.Caches의 환경설정 정보. " +
                              @"region=[{0}], expiration=[{1}] seconds, compressThreshold=[{2}] bytes",
                              region, expiration, compressThreshold);

                cacheConfigs.Add(new MemcachedCacheConfig(region, expiration, compressThreshold));
            }

            if(IsDebugEnabled) {
                log.Debug("Couchbase를 NHibernate 2nd 캐시로 사용하기 위해, 환경설정 값을 모두 로드했습니다. ");
                cacheConfigs.ForEach(cfg => log.Debug(cfg));
            }

            return cacheConfigs.ToArray();
        }
    }
}