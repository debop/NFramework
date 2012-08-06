using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Serializations.SerializedObjects {
    /// <summary>
    /// 객체를 직렬화한 결과 정보 (Binary, JSON, XML, SOAP 등)의 기본 클래스 (원본 객체의 수형과 직렬화된 결과물을 가진다)
    /// </summary>
    [Serializable]
    public abstract class AbstractSerializedObject : ValueObjectBase, ISerializedObject {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        protected AbstractSerializedObject() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="graph"></param>
        protected AbstractSerializedObject(object graph) {
            graph.ShouldNotBeNull("graph");
            ObjectTypeName = graph.GetType().ToStringWithAssemblyName();
        }

        /// <summary>
        /// 직렬화 방법 (Binary|Xml|Soap|Json 등)
        /// </summary>
        public SerializationMethod Method { get; set; }

        /// <summary>
        /// 원본 객체 수형 명 (역직렬화 시에 필요)
        /// </summary>
        public string ObjectTypeName { get; set; }

        /// <summary>
        /// 원본 객체를 직렬화된 정보
        /// </summary>
        public byte[] SerializedValue { get; set; }

        /// <summary>
        /// 역직렬화를 통해, 객체를 반환한다.
        /// </summary>
        /// <returns></returns>
        public abstract object GetDeserializedObject();

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(ISerializedObject other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode() {
            return HashTool.Compute(Method, ObjectTypeName, SerializedValue);
        }

        public override string ToString() {
            return string.Format("[{0}]# ObjectTypeName=[{1}], SerializedValue=[{2}]", GetType().FullName, ObjectTypeName,
                                 SerializedValue);
        }
    }
}