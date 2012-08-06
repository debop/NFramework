using System;

namespace NSoft.NFramework.Caching.SharedCache {
    /// <summary>
    /// 캐시로 저장되는 정보
    /// </summary>
    [Serializable]
    internal class CacheItem {
        public Type ItemType { get; set; }
        public byte[] ItemData { get; set; }

        public override string ToString() {
            return string.Format("CacheItem# ItemType=[{0}], ItemData=[{1}]", ItemType, ItemData);
        }
    }
}