using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using NSoft.NFramework.Xml;

namespace NSoft.NFramework.Caching.SharedCache.NHCaches {
    /// <summary>
    /// SharedCacheClient 의 환경설정 정보를 읽어드립니다.
    /// </summary>
    /// <example>
    /// <code>
    ///		<configSections>
    /// 		<section name="sharedCache" type="NSoft.NFramework.Caching.SharedCache.NHCaches.SharedCacheSectionHandler, NSoft.NFramework.Caching.SharedCache" />
    ///		</configSections>
    /// 
    ///		<sharedCache>
    /// 		<!-- region 은 Unique 해야 합니다. expiration: TimeSpan, compressThreshold:byte단위-->
    /// 		<cache region="NSoft.NFramework" expiration="02:00:00" compressThreshold="40960"/>
    /// 		<cache region="NAccesss" expiration="02:00:00" compressThreshold="40960"/>
    ///		</sharedCache>
    /// </code>
    /// </example>
    public class SharedCacheSectionHandler : IConfigurationSectionHandler {
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
        /// <param name="parent">부모 개체입니다.</param>
        /// <param name="configContext">구성 컨텍스트 개체입니다.</param><param name="section">섹션 XML 노드입니다.</param>
        public object Create(object parent, object configContext, XmlNode section) {
            if(log.IsInfoEnabled)
                log.Info("SharedCache를 NHibernate 2nd 캐시로 사용하기 위해, 환경설정 값을 로드합니다...");

            var cacheConfigs = new List<SharedCacheConfig>();

            if(section == null)
                return cacheConfigs.ToArray();

            XmlNodeList nodes = section.SelectNodes(SR.NodeCache);

            if(nodes == null)
                return cacheConfigs.ToArray();

            foreach(XmlElement node in nodes) {
                var region = node.AttributeToString(SR.AttrRegion, SR.DefaultRegionName);
                var expiration = node.AttributeToString(SR.AttrExpiration, SR.DefaultExpiry.ToString());
                var compressThreshold = node.AttributeToString(SR.AttrCompressThreshold, "4096");

                if(IsDebugEnabled)
                    log.Debug("MongoDB를 이용한 NHibernate.Caches의 환경설정 정보. " +
                              @"region=[{0}], expiration=[{1}], compressThreshold=[{2}] bytes",
                              region, expiration, compressThreshold);

                cacheConfigs.Add(new SharedCacheConfig(region, expiration, compressThreshold));
            }

            if(IsDebugEnabled) {
                log.Debug("SharedCache를 NHibernate 2nd 캐시로 사용하기 위해, 환경설정 값을 모두 로드했습니다. ");
                cacheConfigs.ForEach(cfg => log.Debug(cfg));
            }

            return cacheConfigs.ToArray();
        }
    }
}