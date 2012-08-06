using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Serializations {
    public static partial class SerializerTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 길이가 0인 바이트 배열
        /// </summary>
        public static byte[] EmptyBytes = new byte[0];

        /// <summary>
        /// 기본 직렬화 옵션 (Json)
        /// </summary>
        public static SerializationOptions DefaultSerializationOp = SerializationOptions.Json;

        /// <summary>
        /// Serialize specified <paramref name="graph"/> with specified <paramref name="formatter"/>, then save to stream.
        /// </summary>
        /// <param name="graph">serializing target object.</param>
        /// <param name="stream">where put serialization data.</param>
        /// <param name="formatter">serialization format</param>
        public static void Serialize(object graph, Stream stream, IFormatter formatter) {
            if(graph == null)
                return;

            stream.ShouldNotBeNull("stream");
            formatter.ShouldNotBeNull("formatter");

            lock(stream) {
                formatter.Serialize(stream, graph);
            }
        }

        /// <summary>
        /// Deserialize from stream at current position.
        /// </summary>
        /// <remarks>
        /// Stream의 현재 위치부터 읽어서, 역직렬화를 수행하므로, 
        /// 이 함수 호출전에 꼭 Position을 원하는 위치에 이동시켜 놓아야 한다.
        /// </remarks>
        /// <typeparam name="T">역직렬화된 인스턴스의 형식</typeparam>
        /// <param name="stream">target stream to deserialize</param>
        /// <param name="formatter">Deserialization Formatter</param>
        /// <param name="binder">Serialization binder</param>
        /// <returns>
        /// deserialized object, 
        /// if stream is null or fail to deserialize, return default(T)
        /// </returns>
        public static T Deserialize<T>(Stream stream, IFormatter formatter, SerializationBinder binder = null) {
            if(IsDebugEnabled)
                log.Debug("Deserialize the stream with specified formatter({0}) and binder({1})", formatter, binder);

            var result = default(T);

            if(stream == null) {
                if(log.IsWarnEnabled)
                    log.Warn("Input stream to deserialize is null, therefore return default({0})", result.GetType().Name);

                return result;
            }

            formatter.ShouldNotBeNull("formatter");

            lock(stream) {
                stream.SetStreamPosition(0);

                if(binder != null)
                    formatter.Binder = binder;

                result = (T)formatter.Deserialize(stream);
            }

            return result;
        }

        /// <summary>
        /// 인스턴스를 Deep Copy 수행한다.
        /// </summary>
        /// <typeparam name="T">Type of object to deep copy</typeparam>
        /// <param name="source">source object</param>
        /// <param name="binder">Serialization binder</param>
        /// <returns>copied object, if source is default(T), return default(T).</returns>
        public static T DeepCopy<T>(this T source, SerializationBinder binder = null) {
            if(IsDebugEnabled)
                log.Debug("원본 객체를 DeepCopy합니다.. source=[{0}], binder=[{1}]", source, binder);

            if(Equals(source, default(T)))
                return default(T);

            using(var ms = new MemoryStream()) {
                var bf = new BinaryFormatter();
                Serialize(source, ms, bf);
                ms.SetStreamPosition();

                return Deserialize<T>(ms, bf, binder);
            }
        }

        /// <summary>
        /// Is specified object can serialize/deserialize safely ?
        /// </summary>
        /// <param name="target">target object</param>
        /// <returns>if target object can serialize/deserialized safely, return true, otherwise return false.</returns>
        public static bool IsSafelySerializable(this object target) {
            return (target != null) && IsSafelySerializable(target.GetType());
        }

        /// <summary>
        /// Is specified object can serialize/deserialize safely ?
        /// </summary>
        /// <param name="objectType">Type of object to serialze/deserialize</param>
        /// <returns>if target object can serialize/deserialized safely, return true, otherwise return false.</returns>
        public static bool IsSafelySerializable(this Type objectType) {
            return objectType != null && (objectType.IsSerializable || objectType.IsSimpleType());
        }

        /// <summary>
        /// 객체를 <paramref name="serializationOp"/> 방식으로 직렬화합니다.
        /// </summary>
        public static byte[] Serialize<T>(T graph, SerializationOptions serializationOp = null) {
            if(IsDebugEnabled)
                log.Debug("객체를 직렬화 합니다.... graph type=[{0}], serializationOp=[{1}]", typeof(T), serializationOp);

            return CreateSerializer<T>(serializationOp).Serialize(graph);
        }

        /// <summary>
        /// 직렬화 정보를 <paramref name="serializationOp"/>  방식으로 역직렬화하여 객체로 빌드합니다.
        /// </summary>
        public static T Deserialize<T>(byte[] data, SerializationOptions serializationOp = null) {
            if(IsDebugEnabled)
                log.Debug("객체를 역직렬화 합니다.... graph type=[{0}], serializationOp=[{1}]", typeof(T), serializationOp);

            return CreateSerializer<T>(serializationOp).Deserialize(data);
        }
    }
}