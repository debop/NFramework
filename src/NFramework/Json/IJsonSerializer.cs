using System;

namespace NSoft.NFramework.Json {
    /// <summary>
    /// JSON 방식의 Serializer의 인터페이스입니다.
    /// </summary>
    public interface IJsonSerializer : ISerializer {
        /// <summary>
        /// 특정 수형으로 역직렬화를 수행합니다.
        /// </summary>
        /// <param name="data">직렬화된 정보</param>
        /// <param name="targetType">역직렬화할 대상 수형</param>
        /// <returns>역직렬화 결과</returns>
        object Deserialize(byte[] data, Type targetType);
    }
}