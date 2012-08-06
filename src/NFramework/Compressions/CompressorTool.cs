namespace NSoft.NFramework.Compressions {
    public static class CompressorTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Decompress시에 사용할 버퍼의 크기
        /// </summary>
        public static readonly int BUFFER_SIZE = 0x2000;

        /// <summary>
        /// 빈 바이크 배열
        /// </summary>
        public static readonly byte[] EmptyBytes = new byte[0];

        /// <summary>
        /// 빈 데이타인지 검사한다.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsZeroLength(this byte[] input) {
            return (input == null || input.Length == 0);
        }

        /// <summary>
        /// string Resources for Compressor
        /// </summary>
        internal static class SR {
            /// <summary>
            /// Invalid input data message
            /// </summary>
            public const string InvalidInputDataMsg = @"입력이 NULL이거나 길이가 0입니다. 길이가 0인 바이트 배열을 반환합니다.";

            /// <summary>
            /// Compress is stating message
            /// </summary>
            public const string CompressStartMsg = @"데이터 압축을 시작합니다...";

            /// <summary>
            /// Asynchronous compress is stating message
            /// </summary>
            public const string AsynchronousCompressStartMsg = "비동기 방식으로 압축을 시작합니다...";

            /// <summary>
            /// Compression result message
            /// </summary>
            public const string CompressResultMsg = @"압축 결과: 원본=[{0}] ==> 압축=[{1}] bytes, Compression Ratio:{2:P}";

            /// <summary>
            /// Compression end message
            /// </summary>
            public const string CompressEndMsg = @"데이터 압축 성공!!!";

            /// <summary>
            /// Extract start message
            /// </summary>
            public const string DecompressStartMsg = @"데이터 압축 복원을 시작합니다...";

            /// <summary>
            /// Asynchronous decompress is stating message
            /// </summary>
            public const string AsynchronousDecompressStartMsg = @"비동기 방식으로 압축 복원을 시작합니다...";

            /// <summary>
            /// Extract result message
            /// </summary>
            public const string DecompressResultMsg = @"압축 복원 결과: 압축=[{0}] ==> 복원=[{1}] bytes, Decompression Ratio:{2:P}";

            /// <summary>
            /// Extract end message
            /// </summary>
            public const string DecompressEndMsg = @"압축 복원 성공!!!";
        }
    }
}