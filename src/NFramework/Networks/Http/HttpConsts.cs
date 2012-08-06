namespace NSoft.NFramework.Networks {
    /// <summary>
    /// Http 통신시에 설정되는 기본 값 정의
    /// </summary>
    public static class HttpConsts {
        /// <summary>
        /// content boundry in http protocol
        /// </summary>
        public const string HTTP_CONTENT_BOUNDARY = "---------------------";

        /// <summary>
        /// default buffer length
        /// </summary>
        public const int DEFAULT_BUFFER_LENGTH = 8192;

        /// <summary>
        /// default download buffer.
        /// </summary>
        public const int DEFAULT_DOWNLOAD_BUFFER = ushort.MaxValue + 1;

        /// <summary>
        /// 최소 통신 제한 시간 (15초)
        /// </summary>
        public const int HTTP_MIN_TIMEOUT = 15000; // msec 단위임. (5초)

        /// <summary>
        /// 기본 통신 제한 시간 (90초)
        /// </summary>
        public const int HTTP_DEFAULT_TIMEOUT = 90000; // msec (cf. System.Threading.Timeout.Infinite)

        /// <summary>
        /// localhost url string.
        /// </summary>
        public const string HTTP_LOCALHOST = "http://localhost";

        /// <summary>
        /// content type for http post.
        /// </summary>
        public const string HTTP_POST_CONTENT_TYPE = "application/x-www-form-urlencoded";

        /// <summary>
        /// content type for http file
        /// </summary>
        public const string HTTP_FILE_CONTENT_TYPE = "multipart/form-data";

        /// <summary>
        /// 파일의 기본 컨텐트 타입을 정의한다.
        /// </summary>
        public const string HTTP_DEFAULT_FILE_CONTENT_TYPE = "application/octet-stream";
    }
}