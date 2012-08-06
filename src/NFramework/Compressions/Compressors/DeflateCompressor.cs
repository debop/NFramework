using System;
using System.IO;
using System.IO.Compression;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// Deflate Algorithm을 이용하여 압축/복원을 수행하는 Class
    /// </summary>
    public sealed class DeflateCompressor : AbstractCompressor {
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
            using(var outStream = new MemoryStream()) {
                using(var deflate = new DeflateStream(outStream, CompressionMode.Compress)) {
                    deflate.Write(input, 0, input.Length);
                }
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
            var outStream = new MemoryStream();
            try {
                using(var inStream = new MemoryStream(input))
                using(var deflate = new DeflateStream(inStream, CompressionMode.Decompress, true)) {
                    StreamTool.CopyStreamToStream(deflate, outStream, CompressorTool.BUFFER_SIZE);
                    output = outStream.ToArray();
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("압축 복원 중 예외가 발생했습니다.", ex);

                throw;
            }
            finally {
                outStream.Close();
            }

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressResultMsg, input.Length, output.Length, output.Length / (double)input.Length);

            return output;
        }
    }
}