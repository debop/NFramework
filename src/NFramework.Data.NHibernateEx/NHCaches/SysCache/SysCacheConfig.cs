using System;
using System.Collections.Generic;
using System.Web.Caching;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx.NHCaches.SysCache {
    /// <summary>
    /// 환경 설정 속성 정보
    /// </summary>
    public sealed class SysCacheConfig : IEquatable<SysCacheConfig> {
        private readonly IDictionary<string, string> _properties;

        public SysCacheConfig() : this("NSoft.NFramework", null, null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="region">캐시 영역명</param>
        /// <param name="expiration">유효기간 (초단위)</param>
        /// <param name="priority">중요도 (1~7)</param>
        public SysCacheConfig(string region, int? expiration, int? priority) {
            Region = region ?? string.Empty;

            _properties = new Dictionary<string, string>
                          {
                              { "expiration", expiration.GetValueOrDefault(300).ToString() },
                              { "priority", priority.GetValueOrDefault(CacheItemPriority.Default.GetHashCode()).ToString() }
                          };
        }

        public string Region { get; private set; }

        public IDictionary<string, string> Properties {
            get { return _properties; }
        }

        public bool Equals(SysCacheConfig other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is SysCacheConfig) && Equals((SysCacheConfig)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Region);
        }

        public override string ToString() {
            return string.Format("SysCacheConfig# Region=[{0}], Properties=[{1}]", Region, Properties.DictionaryToString());
        }
    }
}