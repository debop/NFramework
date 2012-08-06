using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NHibernate.Cache;

namespace NSoft.NFramework.Data.NHibernateEx.NHCaches.SysCache {
    /// <summary>
    /// System.Web.Caching 의 캐시 클래스들을 이용한 Cache provider입니다.
    /// </summary>
    public sealed class SysCacheProvider : ICacheProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string SysCacheSectionName = "syscache";

        private static readonly IDictionary<string, ICache> _caches;

        static SysCacheProvider() {
            _caches = new Dictionary<string, ICache>();

            var configs = ConfigurationManager.GetSection(SysCacheSectionName) as SysCacheConfig[];

            if(configs != null) {
                foreach(SysCacheConfig config in configs)
                    _caches.Add(config.Region, new SysCache(config.Region, config.Properties));
            }
        }

        /// <summary>
        /// Configure the cache
        /// </summary>
        /// <param name="regionName">the name of the cache region</param><param name="properties">configuration settings</param>
        /// <returns/>
        public ICache BuildCache(string regionName, IDictionary<string, string> properties) {
            if(regionName == null) {
                regionName = string.Empty;
            }

            ICache result;
            if(_caches.TryGetValue(regionName, out result)) {
                return result;
            }

            // create cache
            if(properties == null) {
                properties = new Dictionary<string, string>(1);
            }

            if(IsDebugEnabled) {
                var sb = new StringBuilder();
                sb.Append("building cache with region: ").Append(regionName).Append(", properties: ");

                foreach(var de in properties) {
                    sb.Append("name=").Append(de.Key);
                    sb.Append("&value=").Append(de.Value);
                    sb.Append(";");
                }
                log.Debug(sb.ToString());
            }
            return new SysCache(regionName, properties);
        }

        /// <summary>
        /// generate a timestamp
        /// </summary>
        /// <returns/>
        public long NextTimestamp() {
            return Timestamper.Next();
        }

        /// <summary>
        /// Callback to perform any necessary initialization of the underlying cache implementation
        ///             during ISessionFactory construction.
        /// </summary>
        /// <param name="properties">current configuration settings</param>
        public void Start(IDictionary<string, string> properties) {
            // Nothing to do.
        }

        /// <summary>
        /// Callback to perform any necessary cleanup of the underlying cache implementation
        ///             during <see cref="M:NHibernate.ISessionFactory.Close"/>.
        /// </summary>
        public void Stop() {
            // Nothing to do.
        }
    }
}