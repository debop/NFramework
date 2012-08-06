using System.IO;
using System.Text;
using System.Threading.Tasks;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Compressions {
    /// <summary>
    /// <see cref="ICompressor"/> 의 Entension Methods 을 제공합니다.
    /// </summary>
    public static class CompressorEx {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 비동기 방식으로 지정한 정보를 압축합니다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="plain">압축할 데이터</param>
        /// <returns>압축을 수행하는 Task</returns>
        public static Task<byte[]> CompressAsync(this ICompressor compressor, byte[] plain) {
            return Task.Factory.StartNew(() => compressor.Compress(plain));
        }

        /// <summary>
        /// 비동기 방식으로 압축된 데이타를 복원합니다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="cipher">압축 복원할 데이터</param>
        /// <returns>압축 복원 Task</returns>
        public static Task<byte[]> DecompressAsync(this ICompressor compressor, byte[] cipher) {
            return Task.Factory.StartNew(() => compressor.Decompress(cipher));
        }

        /// <summary>
        /// 지정된 문자열을 압축하고, 압축된 데이타를 Base64 인코딩으로 변환하여 반환한다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="text">압축할 문자열</param>
        /// <returns></returns>
        public static string CompressString(this ICompressor compressor, string text) {
            if(text.IsEmpty())
                return string.Empty;

            if(IsDebugEnabled)
                log.Debug("Compress string is starting... Input=[{0}]", text.EllipsisChar(30));

            return compressor.Compress(text.ToBytes(Encoding.UTF8)).Base64Encode();
        }

        /// <summary>
        /// Base64로 인코딩된 압축 문자열을 복원하여 <see cref="Encoding.UTF8"/> 문자열로 반환한다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="text">압축 복원을 할 문자열</param>
        /// <returns></returns>
        public static string DecompressString(this ICompressor compressor, string text) {
            if(text.IsEmpty())
                return string.Empty;

            if(IsDebugEnabled)
                log.Debug("Decompress string is starting... Input=[{0}]", text.EllipsisChar(30));

            return compressor.Decompress(text.Base64Decode()).ToText(Encoding.UTF8);
        }

        /// <summary>
        /// 지정된 문자열을 압축하고, 압축된 데이타를 Base64 인코딩으로 변환하여 반환한다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="text">압축할 문자열</param>
        /// <returns></returns>
        public static Task<string> CompressStringAsync(this ICompressor compressor, string text) {
            return Task.Factory.StartNew(() => compressor.CompressString(text));
        }

        /// <summary>
        /// Base64로 인코딩된 압축 문자열을 복원하여 <see cref="Encoding.UTF8"/> 문자열로 반환한다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="text">압축 복원을 할 문자열</param>
        /// <returns></returns>
        public static Task<string> DecompressStringAsync(this ICompressor compressor, string text) {
            return Task.Factory.StartNew(() => compressor.DecompressString(text));
        }

        /// <summary>
        /// 스트림을 압축한다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="stream">압축할 <see cref="Stream"/></param>
        /// <returns>압축된 <see cref="MemoryStream"/></returns>
        public static Stream CompressStream(this ICompressor compressor, Stream stream) {
            return new MemoryStream(compressor.Compress(stream.ToBytes()));
        }

        /// <summary>
        /// 압축된 스트림을 복원한다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="stream">압축된 Stream</param>
        /// <returns>복원된 <see cref="MemoryStream"/></returns>
        public static Stream DecompressStream(this ICompressor compressor, Stream stream) {
            return new MemoryStream(compressor.Decompress(stream.ToBytes()));
        }

        /// <summary>
        /// 스트림을 압축한다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="stream">압축할 <see cref="Stream"/></param>
        /// <returns>압축된 <see cref="MemoryStream"/></returns>
        public static Task<Stream> CompressStreamAsync(this ICompressor compressor, Stream stream) {
            return Task.Factory.StartNew(() => compressor.CompressStream(stream));
        }

        /// <summary>
        /// 압축된 스트림을 복원한다.
        /// </summary>
        /// <param name="compressor"><see cref="ICompressor"/> 구현 객체</param>
        /// <param name="stream">압축된 Stream</param>
        /// <returns>복원된 <see cref="MemoryStream"/></returns>
        public static Task<Stream> DecompressStreamAsync(this ICompressor compressor, Stream stream) {
            return Task.Factory.StartNew(() => compressor.DecompressStream(stream));
        }
    }
}