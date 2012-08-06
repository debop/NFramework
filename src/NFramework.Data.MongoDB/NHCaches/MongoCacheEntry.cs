using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NSoft.NFramework.Data.MongoDB.NHCaches {
    /// <summary>
    /// NHibernate 2nd cache 정보를 MongoDB에 저장할 때, 사용되는 캐시 엔트리 class 입니다.
    /// </summary>
    [Serializable]
    internal class MongoCacheEntry : IEquatable<MongoCacheEntry> {
        public MongoCacheEntry() : this(Guid.NewGuid().ToString(), null) {}

        public MongoCacheEntry(string id, byte[] value) {
            Id = id;
            Value = value;
            LastUpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Cache Entry Key
        /// </summary>
        [BsonId]
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

        public bool Equals(MongoCacheEntry other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is MongoCacheEntry) && Equals((MongoCacheEntry)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Id, IsCompressed);
        }

        public override string ToString() {
            return string.Format("MongoCacheEntry# Id=[{0}], Value=[{1}], LastUpdateDate=[{2}], IsCompressed=[{3}]",
                                 Id, Value, LastUpdateDate, IsCompressed);
        }
    }
}