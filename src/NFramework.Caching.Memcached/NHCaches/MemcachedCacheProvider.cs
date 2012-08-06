using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NHibernate.Cache;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Caching.Memcached.NHCaches {
    /// <summary>
    /// Memcached 를 NHibernate 2nd 캐시 저장소로 사용하도록 하는 Cache Provider입니다.
    /// </summary>
    public class MemcachedCacheProvider : NHibernate.Cache.ICacheProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();
        private static readonly IDictionary<string, ICache> _caches = new ConcurrentDictionary<string, ICache>();

        static MemcachedCacheProvider() {
            var configs = ConfigurationManager.GetSection(SR.NodeMembaseCache) as IList<MemcachedCacheConfig>;

            if(configs == null)
                return;

            lock(_syncLock) {
                if(log.IsInfoEnabled)
                    log.Info("MembaseCacheProvider에서 환경 설정 정보를 읽어, 각 Region 별로 MembaseCacheClient를 생성합니다...");

                foreach(var config in configs) {
                    _caches.Add(config.Region, new MemcachedCacheClient(config.Region, config.Properties));
                }
            }
        }

        /// <summary>
        /// Configure the cache
        /// </summary>
        /// <param name="regionName">the name of the cache region</param><param name="properties">configuration settings</param>
        /// <returns/>
        public ICache BuildCache(string regionName, IDictionary<string, string> properties) {
            if(regionName == null)
                regionName = string.Empty;

            ICache result;
            if(_caches.TryGetValue(regionName, out result))
                return result;

            var props = properties ?? new Dictionary<string, string>(1);

            if(IsDebugEnabled) {
                var sb = new StringBuilder();
                props.RunEach(de => sb.AppendFormat("name={0}&value={1};", de.Key, de.Value));
                log.Debug("build cache with region=[{0}], properties=[{1}]", regionName, sb.ToString());
            }
            return new MemcachedCacheClient(regionName, props);
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