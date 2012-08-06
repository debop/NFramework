namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// Data를 복사만하고, Compression은 수행하지 않는 Class. Dummy로 사용할 때 필요한다.
    /// </summary>
    public sealed class CloneCompressor : AbstractCompressor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 데이타를 복사만 한다.
        /// </summary>
        /// <param name="input">압축할 Data</param>
        /// <returns>압축된 Data</returns>
        public override byte[] Compress(byte[] input) {
            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.CompressStartMsg);

            if(input.IsZeroLength())
                return CompressorTool.EmptyBytes;


            var output = (byte[])input.Clone();

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.CompressEndMsg);

            return output;
        }

        /// <summary>
        /// 데이타를 복사만 한다.
        /// </summary>
        /// <param name="input">복원할 Data</param>
        /// <returns>복원된 Data</returns>
        public override byte[] Decompress(byte[] input) {
            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressStartMsg);

            if(input.IsZeroLength())
                return CompressorTool.EmptyBytes;

            var output = (byte[])input.Clone();

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressEndMsg);

            return output;
        }
    }
}