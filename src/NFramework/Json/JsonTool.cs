using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSoft.NFramework.Json {
    /// <summary>
    /// Json 관련 Utility Method를 제공합니다. 
    /// </summary>
    public static partial class JsonTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << SerializerSettings >>

        /// <summary>
        /// Json 직렬화의 기본 설정
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSerializerSettings =
            new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                // 이걸 해줘야 struct 등이 제대로 deserialize 됩니다.
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                TypeNameHandling = TypeNameHandling.None,
                Converters = new List<JsonConverter>
                             {
                                 new IsoDateTimeConverter(),
                                 new StringEnumConverter()
                             }
            };

        /// <summary>
        /// Bson 직렬화의 기본 설정
        /// </summary>
        public static JsonSerializerSettings DefaultBsonSerializerSettings =
            new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                // MissingMemberHandling = MissingMemberHandling.Ignore,
                // NullValueHandling = NullValueHandling.Ignore,
                // ObjectCreationHandling = ObjectCreationHandling.Replace,      
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                TypeNameHandling = TypeNameHandling.None,
                Converters = new List<JsonConverter>
                             {
                                 new IsoDateTimeConverter(),
                                 new StringEnumConverter()
                             }
            };

        #endregion

        /// <summary>
        /// JSON 형식에서 DataTime 을 내부적으로 double이 아닌 long을 변경해서 저장하므로, .NET DateTime과 오차가 생길 수 있다.
        /// 직렬화된 정보 중 DateTime에 대한 비교는 꼭 ToJsonDateTime() 이용해서 DateTime을 변경한 후 비교해야 합니다.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToJsonDateTime(this DateTime dateTime) {
            return dateTime.AddTicks(-(dateTime.Ticks % 10000));
        }

        /// <summary>
        /// 지정한 객체를 JSON 포맷으로 직렬화를 수행하고, 문자열로 반환합니다.
        /// </summary>
        public static string SerializeAsText(object graph, Formatting? formatting = null,
                                             JsonSerializerSettings jsonSerializerSettings = null) {
            graph.ShouldNotBeNull("graph");
            return JsonConvert.SerializeObject(graph, formatting ?? Formatting.None,
                                               jsonSerializerSettings ?? DefaultJsonSerializerSettings);
        }

        /// <summary>
        /// 지정한 객체를 JSON 포맷으로 직렬화를 수행하고, 문자열로 반환합니다.
        /// </summary>
        public static string SerializeAsText<T>(T graph, Formatting? formatting = null,
                                                JsonSerializerSettings jsonSerializerSettings = null) {
            graph.ShouldNotBeNull("graph");
            return JsonConvert.SerializeObject(graph, formatting ?? Formatting.None,
                                               jsonSerializerSettings ?? DefaultJsonSerializerSettings);
        }

        /// <summary>
        /// 지정한 객체를 JSON 포맷으로 직렬화를 수행하고, byte array로 반환합니다.
        /// </summary>
        public static byte[] SerializeAsBytes(object graph, JsonSerializerSettings jsonSerializerSettings = null) {
            graph.ShouldNotBeNull("graph");
            // return SerializeAsText(graph, Formatting.None, jsonSerializerSettings).ToBytes(Encoding.UTF8);
            return BsonSerializer.Instance.Serialize(graph);
        }

        /// <summary>
        /// 지정한 객체를 JSON 포맷으로 직렬화를 수행하고, byte array로 반환합니다.
        /// </summary>
        public static byte[] SerializeAsBytes<T>(T graph, JsonSerializerSettings jsonSerializerSettings = null) {
            graph.ShouldNotBeNull("graph");
            return BsonSerializer<T>.Instance.Serialize(graph);
        }

        /// <summary>
        /// 지정한 객체를 JSON 포맷으로 직렬화를 수행하고, 압축하여 byte array로 반환합니다.
        /// </summary>
        public static byte[] SerializeWithCompress(object graph, JsonSerializerSettings jsonSerializerSettings = null) {
            graph.ShouldNotBeNull("graph");
            return Compressor.Compress(SerializeAsBytes(graph, jsonSerializerSettings));
        }

        /// <summary>
        /// 지정한 객체를 JSON 포맷으로 직렬화를 수행하고, 압축하여 byte array로 반환합니다.
        /// </summary>
        public static byte[] SerializeWithCompress<T>(T graph, JsonSerializerSettings jsonSerializerSettings = null) {
            graph.ShouldNotBeNull("graph");
            return Compressor.Compress(SerializeAsBytes<T>(graph, jsonSerializerSettings));
        }

        /// <summary>
        /// <paramref name="jsonText"/> 로부터 <paramref name="targetType"/> 수형으로 역직렬화를 수행합니다.
        /// </summary>
        /// <param name="jsonText">Json 직렬화 문자열</param>
        /// <param name="targetType">역직렬화 대상 수형</param>
        /// <param name="jsonSerializerSettings">Json 직렬화 관련 설정</param>
        /// <returns>역직렬화 결과</returns>
        public static object DeserializeFromText(string jsonText, Type targetType, JsonSerializerSettings jsonSerializerSettings = null) {
            if(jsonText.IsWhiteSpace())
                return null;

            return JsonConvert.DeserializeObject(jsonText, targetType, jsonSerializerSettings ?? DefaultJsonSerializerSettings);
        }

        /// <summary>
        /// <paramref name="jsonText"/> 로부터 {T} 수형으로 역직렬화를 수행합니다.
        /// </summary>
        /// <typeparam name="T">역직렬화 대상 수형</typeparam>
        /// <param name="jsonText">Json 직렬화 문자열</param>
        /// <param name="jsonSerializerSettings">Json 직렬화 관련 설정</param>
        /// <returns>역직렬화 결과</returns>
        public static T DeserializeFromText<T>(string jsonText, JsonSerializerSettings jsonSerializerSettings = null) {
            if(jsonText.IsWhiteSpace())
                return default(T);

            return JsonConvert.DeserializeObject<T>(jsonText, jsonSerializerSettings ?? DefaultJsonSerializerSettings);
        }

        /// <summary>
        /// <paramref name="bsonBytes"/> 로부터 <paramref name="targetType"/> 수형으로 역직렬화를 수행합니다.
        /// </summary>
        /// <param name="bsonBytes">Bson 직렬화 바이트 배열</param>
        /// <param name="targetType">역직렬화 대상 수형</param>
        /// <param name="jsonSerializerSettings">Json 직렬화 관련 설정</param>
        /// <returns>역직렬화 결과</returns>
        public static object DeserializeFromBytes(byte[] bsonBytes, Type targetType,
                                                  JsonSerializerSettings jsonSerializerSettings = null) {
            if(bsonBytes == null || bsonBytes.Length == 0)
                return null;

            return BsonSerializer.Instance.Deserialize(bsonBytes, targetType);
        }

        /// <summary>
        /// <paramref name="bsonBytes"/> 로부터 {T} 수형으로 역직렬화를 수행합니다.
        /// </summary>
        /// <typeparam name="T">역직렬화 대상 수형</typeparam>
        /// <param name="bsonBytes">Json 직렬화 바이트 배열</param>
        /// <param name="jsonSerializerSettings">Json 직렬화 관련 설정</param>
        /// <returns>역직렬화 결과</returns>
        public static T DeserializeFromBytes<T>(byte[] bsonBytes, JsonSerializerSettings jsonSerializerSettings = null) {
            if(bsonBytes == null || bsonBytes.Length == 0)
                return default(T);

            return new BsonSerializer<T>().Deserialize(bsonBytes);
        }

        /// <summary>
        /// 압축된 <paramref name="bsonBytes"/> 을 압축해제한 후 {T} 수형으로 역직렬화를 수행합니다.
        /// </summary>
        /// <typeparam name="T">역직렬화 대상 수형</typeparam>
        /// <param name="bsonBytes">Json 직렬화 바이트 배열</param>
        /// <param name="jsonSerializerSettings">Json 직렬화 관련 설정</param>
        /// <returns>역직렬화 결과</returns>
        public static T DeserializeWithDecompress<T>(byte[] bsonBytes, JsonSerializerSettings jsonSerializerSettings = null) {
            if(bsonBytes == null || bsonBytes.Length == 0)
                return default(T);

            return DeserializeFromBytes<T>(Compressor.Decompress(bsonBytes), jsonSerializerSettings ?? DefaultJsonSerializerSettings);
        }
    }
}