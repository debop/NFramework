using System;
using System.IO;
using Ionic.Zlib;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// Ionic 라이브러리의 Deflate 알고리즘을 이용한 Compressor 입니다.
    /// </summary>
    public class IonicDeflateCompressor : AbstractCompressor {
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
                using(var deflate = new DeflateStream(outStream, CompressionMode.Compress, CompressionLevel.BestSpeed)) {
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
                using(var deflate = new DeflateStream(inStream, CompressionMode.Decompress, CompressionLevel.BestSpeed, true)) {
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