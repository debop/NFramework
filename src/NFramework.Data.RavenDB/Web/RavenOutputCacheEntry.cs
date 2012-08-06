using System;
using NSoft.NFramework.Serializations.SerializedObjects;

namespace NSoft.NFramework.Data.RavenDB.Web {
    /// <summary>
    /// ASP.NET Output Cache 정보를 Raven DB에 저장하기 위한 클래스를 표현합니다.
    /// </summary>
    [Serializable]
    public class RavenOutputCacheEntry {
        public RavenOutputCacheEntry(string id, object entry) : this(id, entry, DateTime.Now.AddDays(7).ToUniversalTime()) {}

        public RavenOutputCacheEntry(string id, object entry, DateTime utcExpiry) {
            id.ShouldNotBeWhiteSpace("id");

            Id = id;
            SerializedObject = new BinarySerializedObject(entry);
            UtcExpiry = utcExpiry;
        }

        /// <summary>
        /// Id
        /// </summary>
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