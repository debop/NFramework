using System;
using System.IO;
using NSoft.NFramework.Serializations;
using NSoft.NFramework.Serializations.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace NSoft.NFramework.Json {
    /// <summary>
    /// BSON 형식으로 직렬화, 역직렬화를 수행합니다.
    /// </summary>
    public class BsonSerializer : AbstractSerializer, IJsonSerializer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="BsonSerializer"/>의 Singleton 인스턴스
        /// </summary>
        public static BsonSerializer Instance {
            get { return SingletonTool<BsonSerializer>.Instance; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public BsonSerializer() : this(new JsonSerializer()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="serializer">Json Serializer</param>
        public BsonSerializer(JsonSerializer serializer) {
            Serializer = serializer ?? new JsonSerializer();
        }

        /// <summary>
        /// Json Serializer
        /// </summary>
        public JsonSerializer Serializer { get; private set; }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">직렬화할 객체</param>
        /// <returns>직렬화된 객체 정보, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public override byte[] Serialize(object graph) {
            if(IsDebugEnabled)
                log.Debug("객체를 BSON (Binary JSON) 방식으로 직렬화를 수행합니다... graph=[{0}]", graph);

            if(graph == null)
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
        /// <param name="data">객체를 직렬화한 정보</param>
        /// <returns>역직렬화한 객체</returns>
        public override object Deserialize(byte[] data) {
            if(IsDebugEnabled)
                log.Debug("지정된 데이터를 이용하여 BSON (Binary JSON) 방식의 역직렬화를 수행합니다... data=[{0}]", data);

            if(data == null || data.Length == 0)
                return null;

            using(var ms = new MemoryStream(data))
            using(var reader = new BsonReader(ms)) {
                return Serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">객체를 직렬화한 정보</param>
        /// <param name="targetType">역직렬화된 객체의 수형</param>
        /// <returns>역직렬화한 객체</returns>
        public object Deserialize(byte[] data, Type targetType) {
            if(IsDebugEnabled)
                log.Debug("지정된 데이터를 이용하여 BSON (Binary JSON) 방식의 역직렬화를 수행합니다... data=[{0}], targetType=[{1}]", data, targetType);

            if(data == null || data.Length == 0)
                return null;

            using(var ms = new MemoryStream(data))
            using(var reader = new BsonReader(ms)) {
                return Serializer.Deserialize(reader, targetType);
            }
        }
    }
}