using System;
using System.Collections.Concurrent;
using NSoft.NFramework.Json;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.Serializations {
    public static partial class SerializerTool {
        private static readonly ConcurrentDictionary<CacheKey, object> _serializers = new ConcurrentDictionary<CacheKey, object>();

        /// <summary>
        /// <see cref="SerializationOptions"/>에 맞는 Serializer를 생성합니다.
        /// </summary>
        /// <param name="serializationOp"></param>
        /// <returns></returns>
        public static ISerializer<T> CreateSerializer<T>(SerializationOptions serializationOp = null) {
            serializationOp = serializationOp ?? DefaultSerializationOp;
            return
                (ISerializer<T>)
                _serializers.GetOrAdd(new CacheKey(typeof(T), serializationOp), key => CreateSerializerInternal<T>(key.Options));
        }

        private static ISerializer<T> CreateSerializerInternal<T>(SerializationOptions serializationOp) {
            if(IsDebugEnabled)
                log.Debug("ISerializer를 생성합니다. serializationOp=[{0}]", serializationOp);

            ISerializer<T> serializer;

            switch(serializationOp.Method) {
#if !SILVERLIGHT
                case SerializationMethod.Binary:
                    serializer = new BinarySerializer<T>();
                    break;
#endif
                case SerializationMethod.Json:
                    serializer = new JsonByteSerializer<T>();
                    break;

                case SerializationMethod.Bson:
                    serializer = new BsonSerializer<T>();
                    break;

                default:
                    serializer = new BsonSerializer<T>();
                    break;
            }

            if(serializationOp.IsCompress)
                serializer = new CompressSerializer<T>(serializer);

            if(serializationOp.IsEncrypt)
                serializer = new EncryptSerializer<T>(serializer);

            return serializer;
        }

        [Serializable]
        private class CacheKey : ValueObjectBase {
            public CacheKey(Type objectType, SerializationOptions options) {
                ObjectType = objectType;
                Options = options;
            }

            public Type ObjectType { get; private set; }

            public SerializationOptions Options { get; private set; }

            public bool Equals(CacheKey other) {
                return other != null && GetHashCode().Equals(other.GetHashCode());
            }

            public override bool Equals(object obj) {
                return (obj != null) && (obj is CacheKey) && Equals((CacheKey)obj);
            }

            public override int GetHashCode() {
                return HashTool.Compute(ObjectType.FullName, Options);
            }
        }
    }
}