using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// ICSharpCode.SharpZipLib 의 GZip Alogorithm을 이용한 Compressor
    /// </summary>
    public sealed class SharpGZipCompressor : AbstractCompressor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Initialize a new instance of SharpGZipCompressor with zip level : 6 
        /// </summary>
        public SharpGZipCompressor() : this(6) {}

        /// <summary>
        /// Initialize a new instance of SharpGZipCompressor with zip level.
        /// </summary>
        /// <param name="zipLevel"></param>
        public SharpGZipCompressor(int zipLevel) {
            ZipLevel = zipLevel;
        }

        private int _zipLevel;

        /// <summary>
        /// Compression level (0~9)
        /// </summary>
        public int ZipLevel {
            get { return _zipLevel; }
            set { _zipLevel = Math.Max(0, Math.Min(value, 9)); }
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
            using(var compressedStream = new MemoryStream(input.Length)) {
                using(var gzs = new GZipOutputStream(compressedStream)) {
                    gzs.SetLevel(ZipLevel);
                    gzs.Write(input, 0, input.Length);
                    gzs.Finish();
                }
                output = compressedStream.ToArray();
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
            using(var compressedStream = new MemoryStream(input)) {
                using(var gzs = new GZipInputStream(compressedStream))
                using(var extractedStream = new MemoryStream(input.Length * 2)) {
                    StreamTool.CopyStreamToStream(gzs, extractedStream, CompressorTool.BUFFER_SIZE);
                    output = extractedStream.ToArray();
                }
            }

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressResultMsg, input.Length, output.Length, output.Length / (double)input.Length);

            return output;
        }
    }
}