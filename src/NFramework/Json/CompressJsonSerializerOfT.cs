using System;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.Json {
    /// <summary>
    /// 압축 가능한 JsonSerializer 입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CompressJsonSerializer<T> : CompressSerializer<T> {
        /// <summary>
        /// <see cref="CompressJsonSerializer{T}"/> 의 Singleton 인스턴스
        /// </summary>
        public static CompressJsonSerializer<T> Instance {
            get { return SingletonTool<CompressJsonSerializer<T>>.Instance; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public CompressJsonSerializer() : base(new JsonByteSerializer<T>()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="serializer"></param>
        public CompressJsonSerializer(JsonByteSerializer<T> serializer) : base(serializer) {}
    }
}