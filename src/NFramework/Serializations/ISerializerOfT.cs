namespace NSoft.NFramework {
    /// <summary>
    /// 인스턴스를 Serialize/Deserialize를 수행하는 Class의 기본 인터페이스
    /// </summary>
    /// <typeparam name="T">Serialize를 수행할 형식</typeparam>
    public interface ISerializer<T> {
        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <returns>직렬화된 객체, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        byte[] Serialize(T graph);

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">serialized data to be deserialized.</param>
        /// <returns>deserialized object</returns>
        T Deserialize(byte[] data);
    }
}