using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace NSoft.NFramework.Data.NHibernateEx.NHCaches.SysCache {
    public sealed class SysCacheSectionHandler : IConfigurationSectionHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string CacheConfigSectionName = "cache";

        /// <summary>
        /// 구성 섹션 처리기를 만듭니다.
        /// </summary>
        /// <returns>
        /// 만들어진 섹션 처리기 개체입니다.
        /// </returns>
        /// <param name="parent">부모 개체입니다.</param>
        /// <param name="configContext">구성 컨텍스트 개체입니다.</param><param name="section">섹션 XML 노드입니다.</param>
        public object Create(object parent, object configContext, XmlNode section) {
            if(IsDebugEnabled)
                log.Debug("NSoft.NFramework.Data.NHCaches.SysCache를 위한 환경 설정 정보를 로드합니다...");

            var configs = new List<SysCacheConfig>();
            XmlNodeList nodes = section.SelectNodes(CacheConfigSectionName);

            foreach(XmlNode node in nodes) {
                // 캐시 Region, Expiration (초단위), 중요도
                //
                var region = node.Attributes["region"].AsText(string.Empty);
                var expiration = node.Attributes["expiration"].AsInt(120);
                var priority = node.Attributes["priority"].AsInt(4);

                var config = new SysCacheConfig(region, expiration, priority);
                configs.Add(config);

                if(IsDebugEnabled)
                    log.Debug("SysCache용 환경설정 정보를 읽었습니다. 환경설정 정보=[{0}]", config);
            }

            return configs.ToArray();
        }
    }
}