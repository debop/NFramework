using System;

namespace NSoft.NFramework.Caching.SharedCache.NHCaches {
    /// <summary>
    /// SharedCache 시스템에 저장될 항목을 표현합니다.
    /// </summary>
    [Serializable]
    public class SharedCacheEntry : ValueObjectBase {
        public SharedCacheEntry() : this(Guid.NewGuid().ToString(), null) {}

        public SharedCacheEntry(string id, byte[] value) {
            Id = id;
            Value = value;
            LastUpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Cache Entry Key
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Cache Update Date
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// 압축 여부
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// 캐시 값
        /// </summary>
        public byte[] Value { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Id, IsCompressed);
        }

        public override string ToString() {
            return string.Format("SharedCacheEntry# Id=[{0}], Value=[{1}], LastUpdateDate=[{2}], IsCompressed=[{3}]",
                                 Id, Value, LastUpdateDate, IsCompressed);
        }
    }
}