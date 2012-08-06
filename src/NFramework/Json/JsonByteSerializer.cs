using System;
using System.Text;
using NSoft.NFramework.Serializations;
using NSoft.NFramework.Serializations.Serializers;
using NSoft.NFramework.Tools;
using Newtonsoft.Json;

namespace NSoft.NFramework.Json {
    /// <summary>
    /// JSON 포맷으로 객체를 직렬화/역직렬화를 수행합니다. 직렬화된 정보를 Text가 아닌 UTF-8 인코딩 방식으로 byte[] 로 변환합니다.
    /// </summary>
    public class JsonByteSerializer : AbstractSerializer, IJsonSerializer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="JsonByteSerializer"/>의 Singleton 인스턴스
        /// </summary>
        public static JsonByteSerializer Instance {
            get { return SingletonTool<JsonByteSerializer>.Instance; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public JsonByteSerializer() : this(JsonTool.DefaultJsonSerializerSettings) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="jsonSerializerSettings">직렬화 설정 방식</param>
        public JsonByteSerializer(JsonSerializerSettings jsonSerializerSettings) {
            SerializerSettings = jsonSerializerSettings ?? JsonTool.DefaultJsonSerializerSettings;
        }

        /// <summary>
        /// SerializeSettings
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; private set; }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <returns>직렬화된 객체, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public override byte[] Serialize(object graph) {
            if(IsDebugEnabled)
                log.Debug("객체를 JSON 형식으로 Serialize를 수행하고, UTF-8 형식의 byte[] 로 변환하여 반환합니다...");

            if(Equals(graph, null))
                return SerializerTool.EmptyBytes;

            var jsonText = JsonConvert.SerializeObject(graph, Formatting.None, SerializerSettings);

            return jsonText.ToBytes(Encoding.UTF8);
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">serialized data to be deserialized.</param>
        /// <returns>deserialized object</returns>
        public override object Deserialize(byte[] data) {
            if(IsDebugEnabled)
                log.Debug("JSON 포맷으로 직렬화된 정보를 역직렬화를 해서 객체로 빌드합니다...");

            if(data == null || data.Length == 0)
                return null;

            return JsonConvert.DeserializeObject(data.ToText(Encoding.UTF8), SerializerSettings);
        }

        /// <summary>
        /// 특정 수형으로 역직렬화를 수행합니다.
        /// </summary>
        /// <param name="data">직렬화된 정보</param>
        /// <param name="targetType">역직렬화할 대상 수형</param>
        /// <returns>역직렬화 결과</returns>
        public object Deserialize(byte[] data, Type targetType) {
            if(IsDebugEnabled)
                log.Debug("JSON 포맷으로 직렬화된 정보를 역직렬화를 해서 객체로 빌드합니다... targetType=[{0}]", targetType);

            if(data == null || data.Length == 0)
                return null;

            return JsonConvert.DeserializeObject(data.ToText(Encoding.UTF8), targetType, SerializerSettings);
        }
    }
}