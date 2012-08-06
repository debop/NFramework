using System.IO;
using NSoft.NFramework.Serializations;
using NSoft.NFramework.Serializations.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace NSoft.NFramework.Json {
    /// <summary>
    /// BSON 형식으로 직렬화, 역직렬화를 수행합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BsonSerializer<T> : AbstractSerializer<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="BsonSerializer{T}"/> 의 Singlton 인스턴스
        /// </summary>
        public static BsonSerializer<T> Instance {
            get { return SingletonTool<BsonSerializer<T>>.Instance; }
        }

        public BsonSerializer() : this(new JsonSerializer()) {}

        public BsonSerializer(JsonSerializer serializer) {
            Serializer = serializer ?? new JsonSerializer();
        }

        /// <summary>
        /// 내부 <see cref="JsonSerializer"/>
        /// </summary>
        public JsonSerializer Serializer { get; private set; }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <returns>직렬화된 객체, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public override byte[] Serialize(T graph) {
            if(IsDebugEnabled)
                log.Debug("객체를 BSON (Binary JSON) 방식으로 직렬화를 수행합니다... graph=[{0}]", graph);

            if(ReferenceEquals(graph, null))
                return SerializerTool.EmptyBytes;

            using(var ms = new MemoryStream())
            using(var writer = new BsonWriter(ms)) {
                Serializer.Serialize(writer, graph);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">serialized data to be deserialized.</param>
        /// <returns>deserialized object</returns>
        public override T Deserialize(byte[] data) {
            if(IsDebugEnabled)
                log.Debug("지정된 데이터를 이용하여 BSON (Binary JSON) 방식의 역직렬화를 수행합니다...");

            if(data == null || data.Length == 0)
                return default(T);

            using(var ms = new MemoryStream(data))
            using(var reader = new BsonReader(ms)) {
                return Serializer.Deserialize<T>(reader);
            }
        }
    }
}