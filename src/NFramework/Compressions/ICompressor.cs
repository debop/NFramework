namespace NSoft.NFramework.Compressions {
    /// <summary>
    /// 압축/복원을 수행하는 압축기의 기본 Interface
    /// </summary>
    public interface ICompressor {
        /// <summary>
        /// 지정된 데이타를 압축한다.
        /// </summary>
        /// <param name="input">압축할 Data</param>
        /// <returns>압축된 Data</returns>
        byte[] Compress(byte[] input);

        /// <summary>
        /// 압축된 데이타를 복원한다.
        /// </summary>
        /// <param name="input">복원할 Data</param>
        /// <returns>복원된 Data</returns>
        byte[] Decompress(byte[] input);
    }
}