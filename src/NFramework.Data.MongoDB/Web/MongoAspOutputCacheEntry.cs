using System;
using MongoDB.Bson.Serialization.Attributes;
using NSoft.NFramework.Serializations.SerializedObjects;

namespace NSoft.NFramework.Data.MongoDB.Web {
    /// <summary>
    /// MongDB에 저장할 OutputCache 정보
    /// </summary>
    [Serializable]
    public class MongoAspOutputCacheEntry {
        public MongoAspOutputCacheEntry(string id, object entry) : this(id, entry, DateTime.Now.AddDays(7).ToUniversalTime()) {}

        public MongoAspOutputCacheEntry(string id, object entry, DateTime utcExpiry) {
            id.ShouldNotBeWhiteSpace("id");

            Id = id;
            SerializedObject = new BinarySerializedObject(entry);
            UtcExpiry = utcExpiry;
        }

        /// <summary>
        /// Id
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// OutputCache 정보
        /// </summary>
        public BinarySerializedObject SerializedObject { get; set; }

        /// <summary>
        /// Output Cache  엔트리의 만료 시각
        /// </summary>
        public DateTime UtcExpiry { get; set; }

        public object GetEntry() {
            return (SerializedObject != null) ? SerializedObject.GetDeserializedObject() : null;
        }
    }
}