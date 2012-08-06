using System;

namespace NSoft.NFramework.Serializations {
    /// <summary>
    /// 객체를 직렬화한 결과 정보 (Binary, JSON, XML, SOAP 등)의 기본 인터페이스 (원본 객체의 수형과 직렬화된 결과물을 가진다)
    /// </summary>
    public interface ISerializedObject : IEquatable<ISerializedObject> {
        /// <summary>
        /// 직렬화 방법 (Binary|Xml|Soap|Json 등)
        /// </summary>
        SerializationMethod Method { get; set; }

        /// <summary>
        /// 원본 객체 수형 명 (역직렬화 시에 필요)
        /// </summary>
        string ObjectTypeName { get; set; }

        /// <summary>
        /// 원본 객체를 직렬화된 정보
        /// </summary>
        byte[] SerializedValue { get; set; }

        /// <summary>
        /// 역직렬화를 통해, 객체를 반환한다.
        /// </summary>
        /// <returns></returns>
        object GetDeserializedObject();
    }
}