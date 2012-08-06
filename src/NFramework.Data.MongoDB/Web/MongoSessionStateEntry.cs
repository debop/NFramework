using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NSoft.NFramework.Data.MongoDB.Web {
    /// <summary>
    /// MongoDB에 저장할 Session 상태 정보
    /// </summary>
    [Serializable]
    internal class MongoSessionStateEntry {
        public MongoSessionStateEntry(string id, byte[] state) : this(id, state, DateTime.Now.AddDays(1).ToUniversalTime()) {}

        public MongoSessionStateEntry(string id, byte[] state, DateTime utcExpiry) {
            id.ShouldNotBeNull("id");

            Id = id;
            State = state ?? new byte[0];
            UtcExpiry = utcExpiry;
        }

        /// <summary>
        /// Session Id
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Session State 정보
        /// </summary>
        public byte[] State { get; set; }

        /// <summary>
        /// 만료 시각
        /// </summary>
        public DateTime UtcExpiry { get; set; }
    }
}