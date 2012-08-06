namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// Compressor의 기본 클래스입니다.
    /// </summary>
    public abstract class AbstractCompressor : ICompressor {
        /// <summary>
        /// 지정된 데이타를 압축한다.
        /// </summary>
        /// <param name="input">압축할 Data</param>
        /// <returns>압축된 Data</returns>
        public virtual byte[] Compress(byte[] input) {
            return input;
        }

        /// <summary>
        /// 압축된 데이타를 복원한다.
        /// </summary>
        /// <param name="input">복원할 Data</param>
        /// <returns>복원된 Data</returns>
        public virtual byte[] Decompress(byte[] input) {
            return input;
        }
    }
}