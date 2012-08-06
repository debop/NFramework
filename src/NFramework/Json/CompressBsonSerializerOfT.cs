using System;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.Json {
    /// <summary>
    /// 압축 가능한 Bson Serializer 입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CompressBsonSerializer<T> : CompressSerializer<T> {
        /// <summary>
        /// <see cref="CompressBsonSerializer{T}"/> 의 Singleton 인스턴스
        /// </summary>
        public static CompressBsonSerializer<T> Instance {
            get { return SingletonTool<CompressBsonSerializer<T>>.Instance; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public CompressBsonSerializer() : base(new BsonSerializer<T>()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="serializer"></param>
        public CompressBsonSerializer(BsonSerializer<T> serializer) : base(serializer) {}
    }
}