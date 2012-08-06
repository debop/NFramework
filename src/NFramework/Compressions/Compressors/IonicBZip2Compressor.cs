using System;
using System.IO;
using Ionic.BZip2;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// Ionic 라이브러리의 BZip2 알고리즘을 이용한 Compressor 입니다.
    /// </summary>
    public class IonicBZip2Compressor : AbstractCompressor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Initialize a new instance of SharpGZipCompressor with zip level : 6 
        /// </summary>
        public IonicBZip2Compressor() : this(6) {}

        /// <summary>
        /// Initialize a new instance of SharpGZipCompressor with zip level.
        /// </summary>
        /// <param name="zipLevel"></param>
        public IonicBZip2Compressor(int zipLevel) {
            ZipLevel = zipLevel;
        }

        private int _zipLevel;

        /// <summary>
        /// Compression level (1~9)
        /// </summary>
        public int ZipLevel {
            get { return _zipLevel; }
            set { _zipLevel = Math.Max(1, Math.Min(value, 9)); }
        }

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

            using(var outStream = new MemoryStream(input.Length)) {
                using(var bz2 = new BZip2OutputStream(outStream, ZipLevel)) {
                    bz2.Write(input, 0, input.Length);
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
            using(var inStream = new MemoryStream(input))
            using(var bz2 = new BZip2InputStream(inStream))

            using(var outStream = new MemoryStream(input.Length * 2)) {
                StreamTool.CopyStreamToStream(bz2, outStream, CompressorTool.BUFFER_SIZE);
                output = outStream.ToArray();
            }

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressResultMsg, input.Length, output.Length, output.Length / (double)input.Length);

            return output;
        }
    }
}