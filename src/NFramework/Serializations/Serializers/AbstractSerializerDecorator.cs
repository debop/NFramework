using NSoft.NFramework.Json;

namespace NSoft.NFramework.Serializations.Serializers {
    /// <summary>
    /// 특정 인스턴스를 직렬화/역직렬화 시에 추가 변환 작업 (압축/암호화 등)을 수행하는 Decorator에 대한 기본 클래스입니다.
    /// </summary>
    public abstract class AbstractSerializerDecorator<T> : ISerializer<T> {
        protected AbstractSerializerDecorator() : this(new BsonSerializer<T>()) {}

        protected AbstractSerializerDecorator(ISerializer<T> serializer) {
            Serializer = serializer;
        }

        /// <summary>
        /// Wrapping 된 실제 Serializer
        /// </summary>
        public virtual ISerializer<T> Serializer { get; set; }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">직렬화할 객체</param>
        /// <returns>직렬화된 객체 정보, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public virtual byte[] Serialize(T graph) {
            return Serializer.Serialize(graph);
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">객체를 직렬화한 정보</param>
        /// <returns>역직렬화한 객체</returns>
        public virtual T Deserialize(byte[] data) {
            return Serializer.Deserialize(data);
        }
    }
}