using System.IO;
using System.Threading.Tasks;
using NSoft.NFramework.Compressions;

namespace NSoft.NFramework.Parallelism.Tools {

    /// <summary>
    /// /// <summary>
    /// 비동기 압축/복원을 지원하는 <see cref="ICompressor"/>의 확장 메소드를 제공합니다.
    /// </summary>
    /// </summary>
    public static class CompressorAsync {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 비동기 방식으로 정보를 <paramref name="compressor"/>로 압축합니다.
        /// </summary>
        /// <param name="compressor">압축기</param>
        /// <param name="input">압축할 내용</param>
        /// <returns>압축 작업</returns>
        public static Task<byte[]> CompressTask(this ICompressor compressor, byte[] input) {
            compressor.ShouldNotBeNull("compressor");
            input.ShouldNotBeEmpty("input");

            if(IsDebugEnabled)
                log.Debug("바이트 배열을 비동기 방식으로 압축합니다...");

            return Task.Factory.StartNew(() => compressor.Compress(input));
        }

        /// <summary>
        /// 지정한 <paramref name="stream"/>을 비동기적으로 압축하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="compressor">Compressor</param>
        /// <param name="stream">압축할 대상 스트림</param>
        /// <returns>압축된 스트림을 결과로 제공하는 <see cref="Task{Stream}"/></returns>
        public static Task<Stream> CompressTask(this ICompressor compressor, Stream stream) {
            compressor.ShouldNotBeNull("compressor");
            stream.ShouldNotBeNull("stream");

            // GZipStream, DeflateStream의 비동기 함수인 BeginRead, BeginWrite 등은 실제로는 StreamExtensions 에서 처리해주고, 
            // 메모리에 다 올라온 byte[]에 대해서 Computed-bound 비동기 방식으로 압축을 수행한다.

            return
                stream
                    .ReadAllBytesAsync()
                    .ContinueWith<Stream>(antecedent => new MemoryStream(compressor.CompressTask(antecedent.Result).Result),
                                          TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// 비동기 방식으로 압축된 정보를 <paramref name="compressor"/>로 복원합니다.
        /// </summary>
        /// <param name="compressor">압축기</param>
        /// <param name="input">압축 복원을 할 데이터</param>
        /// <returns>압축해제된 바이트 배열을 결과로 제공하는 <see cref="Task{Stream}"/></returns>
        public static Task<byte[]> DecompressTask(this ICompressor compressor, byte[] input) {
            if(IsDebugEnabled)
                log.Debug("압축된 바이트 배열을 비동기 방식으로 압축 해제합니다...");

            return Task.Factory.StartNew(() => compressor.Decompress(input), TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// 지정한 <paramref name="stream"/>을 비동기적으로 압축복원하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="compressor">Compressor</param>
        /// <param name="stream">압축 해제할 스트림</param>
        /// <returns>압축해제된 스트림을 결과로 제공하는 <see cref="Task{Stream}"/></returns>
        public static Task<Stream> DecompressTask(this ICompressor compressor, Stream stream) {
            compressor.ShouldNotBeNull("compressor");
            stream.ShouldNotBeNull("stream");

            // GZipStream, DeflateStream의 비동기 함수인 BeginRead, BeginWrite 등은 실제로는 StreamExtensions 에서 처리해주고, 
            // 메모리에 다 올라온 byte[]에 대해서 Computed-bound 비동기 방식으로 압축 해제를 수행한다.

            return
                stream.ReadAllBytesAsync()
                    .ContinueWith<Stream>(antecedent => new MemoryStream(compressor.DecompressTask(antecedent.Result).Result),
                                          TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}