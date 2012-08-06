namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 요청 메소드 종류 (SqlString, Procedure, Method 등)
    /// </summary>
    public enum RequestMethodKind {
        /// <summary>
        /// 미지정
        /// </summary>
        Unknown,

        /// <summary>
        /// 서버상에 정의된 Method를 호출한다. (ini 파일로 저장되어 있음)
        /// </summary>
        Method,

        /// <summary>
        /// 일반 SQL Query 문 실행 요청
        /// </summary>
        SqlString,

        /// <summary>
        /// Stored Procedure 실행 요청
        /// </summary>
        Procedure,
    }
}