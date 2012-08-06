using System.IO;
using ICSharpCode.SharpZipLib.BZip2;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// BZip2 Algorithm을 이용하여 압축/복원을 수행하는 Class
    /// </summary>
    public sealed class SharpBZip2Compressor : AbstractCompressor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 데이타를 압축한다.
        /// </summary>
        /// <param name="input">압축할 Data</param>
        /// <returns>압축된 Data</returns>
        public override byte[] Compress(byte[] input) {
            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.CompressStartMsg);

            // check input data
            if(input.IsZeroLength()) {
                if(IsDebugEnabled)
                    log.Debug(CompressorTool.SR.InvalidInputDataMsg);

                return CompressorTool.EmptyBytes;
            }

            byte[] output;
            // Compress
            using(var outStream = new MemoryStream(input.Length))
            using(var bz2 = new BZip2OutputStream(outStream, CompressorTool.BUFFER_SIZE)) {
                bz2.Write(input, 0, input.Length);
                bz2.Close();
                output = outStream.ToArray();
            }

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.CompressResultMsg, input.Length, output.Length, output.Length / (double)input.Length);

            return output;
        }

        /// <summary>
        /// 압축된 데이타를 복원한다.
        /// </summary>
        /// <param name="input">복원할 Data</param>
        /// <returns>복원된 Data</returns>
        public override byte[] Decompress(byte[] input) {
            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressStartMsg);

            // check input data
            if(input.IsZeroLength()) {
                if(IsDebugEnabled)
                    log.Debug(CompressorTool.SR.InvalidInputDataMsg);

                return CompressorTool.EmptyBytes;
            }

            byte[] output;
            using(var inStream = new MemoryStream(input)) {
                using(var bz2 = new BZip2InputStream(inStream))
                using(var outStream = new MemoryStream(input.Length * 2)) {
                    StreamTool.CopyStreamToStream(bz2, outStream, CompressorTool.BUFFER_SIZE);
                    output = outStream.ToArray();
                }
            }

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressResultMsg, input.Length, output.Length, output.Length / (double)input.Length);

            return output;
        }
    }
}