namespace NSoft.NFramework.Serializations {
    /// <summary>
    /// 객체를 직렬화/역직렬화 하는 방식
    /// </summary>
    public enum SerializationMethod {
        /// <summary>
        /// 직렬화 없음
        /// </summary>
        None,

#if !SILVERLIGHT

        /// <summary>
        /// 이진 방식
        /// </summary>
        Binary,

        /// <summary>
        /// Xml
        /// </summary>
        Xml,

        /// <summary>
        /// Soap
        /// </summary>
        Soap,

#endif

        /// <summary>
        /// JSON
        /// </summary>
        Json,

        /// <summary>
        /// Binary JSON
        /// </summary>
        Bson
    }
}