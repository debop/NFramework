namespace NSoft.NFramework.DataServices.WebHost {
    /// <summary>
    /// Http 통신시 사용하는 Parameter Name 들
    /// </summary>
    public static class HttpParams {
        /// <summary>
        /// 제품 정보를 지정하는 키
        /// </summary>
        public const string Product = "Product";

        /// <summary>
        /// 실행할 메소드를 지정하는 Parameter 명
        /// </summary>
        public const string Method = "Method";

        /// <summary>
        /// 응답 포맷을 지정하는 Parameter 명	( <see cref="ResponseFormatKind"/> 의 None | Scalar | ResultSet )
        /// </summary>
        public const string ResponseFormat = "ResponseFormat";
    }
}