using System;
using System.IO;
using System.Text;
using System.Threading;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Compressions {
    /// <summary>
    /// IoC를 통해 내부 ICompressor를 이용하여 Compression기능을 제공하는 Utility Class입니다.<br/>
    /// 기본적으로 GZipCompressor를 사용합니다.
    /// </summary>
    /// <example>
    ///		<code>
    ///		byte[] plain = plainText.ToBytes();
    ///		byte[] compressed = Compressor.Compress(plain);
    ///		byte[] recovery = Compressor.Decompress(compressed);
    ///		Assert.AreEqual(plain, recovery);
    ///		</code>
    /// </example>
    public static class Compressor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();
        private static ICompressor _innerCompressor;

        public static byte[] EmptyBytes = new byte[0];

        ///<summary>
        /// 압축기 기본 문자 인코딩 방식 (<see cref="Encoding.UTF8"/>)
        ///</summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// Instance of inner compressor
        /// </summary>
        public static ICompressor InnerCompressor {
            get {
                if(_innerCompressor == null)
                    lock(_syncLock)
                        if(_innerCompressor == null) {
                            var compressor = ResolveCompressor();
                            Thread.MemoryBarrier();
                            _innerCompressor = compressor;
                        }

                return _innerCompressor;
            }
            set {
                if(value != null)
                    _innerCompressor = value;
            }
        }

        /// <summary>
        /// Resolve compressor by Inversion Of Control
        /// </summary>
        /// <returns></returns>
        private static ICompressor ResolveCompressor() {
            if(IoC.IsInitialized)
                return IoC.TryResolve<ICompressor, SharpGZipCompressor>();

            return new SharpGZipCompressor();
        }

        /// <summary>
        /// 지정된 데이타를 압축한다.
        /// </summary>
        /// <param name="input">압축할 Data</param>
        /// <returns>압축된 Data</returns>
        [Obsolete("CompressorEx.Compress() 를 이용하세요")]
        public static byte[] Compress(byte[] input) {
            if(input.IsZeroLength()) {
                if(log.IsWarnEnabled)
                    log.Warn(CompressorTool.SR.InvalidInputDataMsg);

                return EmptyBytes;
            }

            return InnerCompressor.Compress(input);
        }

        /// <summary>
        /// 압축된 데이타를 복원한다.
        /// </summary>
        /// <param name="input">복원할 Data</param>
        /// <returns>복원된 Data</returns>
        [Obsolete("CompressorEx.Decompress() 를 이용하세요")]
        public static byte[] Decompress(byte[] input) {
            if(input.IsZeroLength()) {
                if(log.IsWarnEnabled)
                    log.Warn(CompressorTool.SR.InvalidInputDataMsg);

                return EmptyBytes;
            }

            return InnerCompressor.Decompress(input);
        }

        /// <summary>
        /// 지정된 문자열을 압축하고, 압축된 데이타를 Base64 인코딩으로 변환하여 반환한다.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Obsolete("CompressorEx.CompressText() 를 이용하세요")]
        public static string Compress(string input) {
            if(input.IsWhiteSpace())
                return string.Empty;

            if(IsDebugEnabled)
                log.Debug("Compress string is starting... Input:" + input.EllipsisChar(30));

            return Compress(input.ToBytes(Encoding.UTF8)).Base64Encode();
        }

        /// <summary>
        /// Base64로 인코딩된 압축 문자열을 복원하여 <see cref="Encoding.UTF8"/> 문자열로 반환한다.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Obsolete("CompressorEx.DecompressText() 를 이용하세요")]
        public static string Decompress(string input) {
            if(input.IsWhiteSpace())
                return string.Empty;

            if(IsDebugEnabled)
                log.Debug("Decompress string is starting... Input:" + input.EllipsisChar(30));

            return Decompress(input.Base64Decode()).ToText(Encoding.UTF8);
        }

        /// <summary>
        /// 스트림을 압축한다.
        /// </summary>
        /// <param name="input">압축할 <see cref="Stream"/></param>
        /// <returns>압축된 <see cref="MemoryStream"/></returns>
        [Obsolete("CompressorEx.CompressStream() 를 이용하세요")]
        public static Stream Compress(Stream input) {
            return new MemoryStream(Compress(input.ToBytes()));
        }

        /// <summary>
        /// 압축된 스트림을 복원한다.
        /// </summary>
        /// <param name="input">압축된 Stream</param>
        /// <returns>복원된 <see cref="MemoryStream"/></returns>
        [Obsolete("CompressorEx.DecompressStream() 를 이용하세요")]
        public static Stream Decompress(Stream input) {
            return new MemoryStream(Decompress(input.ToBytes()));
        }
    }
}