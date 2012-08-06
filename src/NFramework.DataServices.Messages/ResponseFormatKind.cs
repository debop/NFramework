namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// Data 실행 결과의 형식 ( Non, Scalar, DataTable 등)
    /// </summary>
    public enum ResponseFormatKind {
        /// <summary>
        /// 미지정
        /// </summary>
        Unknown,

        /// <summary>
        /// 결과값 필요없음 (NonExecuteQuery 실행)
        /// </summary>
        None,

        /// <summary>
        /// Scalar 값 형태의 결과
        /// </summary>
        Scalar,

        /// <summary>
        /// Data 정보를 Json 포맷으로 전달 (<see cref="ResultSet"/> 으로 변환된 후 JSON 직렬화, 역직렬화가 수행된다)
        /// </summary>
        ResultSet,
    }
}