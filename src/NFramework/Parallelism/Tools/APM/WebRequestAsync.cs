using System.IO;
using System.Net;
using System.Threading.Tasks;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="WebRequest"/>를 APM (Asynchronous Programming Model-비동기 프로그래밍 모델) 방식으로 수행하는 Extension Method를 제공합니다.
    /// </summary>
    /// <remarks>
    /// 참고 사이트 :
    /// <list>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/ms228963.aspx</item>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/dd997423.aspx</item>
    /// </list>
    /// </remarks>
    public static class WebRequestAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="webRequest"/>를 비동기적으로 실행하여, <see cref="WebResponse"/>를 얻는 <see cref="Task"/>를 생성합니다.
        /// 응답 결과가 gzip, deflate 방식으로 압축되어서 올 수 있습니다. Response Header의 Content-Encoding을 확인하여 처리하셔야 합니다.
        /// 아니면 DownloadDataAsync를 사용하세요.
        /// </summary>
        /// <param name="webRequest">Http 요청 객체</param>
        /// <returns></returns>
        public static Task<WebResponse> GetResponseAsync(this WebRequest webRequest) {
            webRequest.ShouldNotBeNull("webRequest");

            if(IsDebugEnabled)
                log.Debug("WebRequest의 Response를 비동기적으로 받는 Task를 생성합니다... webRquest=[{0}]", webRequest.RequestUri);

            return Task<WebResponse>.Factory.FromAsync(webRequest.BeginGetResponse,
                                                       webRequest.EndGetResponse,
                                                       webRequest);
        }

        /// <summary>
        /// <paramref name="webRequest"/>로부터 요청 스트림을 비동기적으로 얻는 <see cref="Task"/>를 생성합니다.
        /// </summary>
        /// <param name="webRequest">Http 요청 객체</param>
        /// <returns></returns>
        public static Task<Stream> GetResponseStreamAsync(this WebRequest webRequest) {
            webRequest.ShouldNotBeNull("webRequest");

            if(IsDebugEnabled)
                log.Debug("WebRequest의 ResponseStream를 비동기적으로 받는 Task를 생성합니다... webRquest=[{0}]", webRequest.RequestUri);

            return
                webRequest
                    .GetResponseAsync()
                    .ContinueWith(antecedent => antecedent.Result.GetResponseStreamByContentEncoding(),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="webRequest"/>로부터 데이타를 비동기적으로 다운받는 <see cref="Task"/>를 생성합니다.
        /// 압축된 결과가 왔을 경우, 복원하여 데이타를 전달합니다.
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        public static Task<byte[]> DownloadDataAsync(this WebRequest webRequest) {
            webRequest.ShouldNotBeNull("webRequest");

            if(IsDebugEnabled)
                log.Debug("WebRequest를 이용하여, 비동기적으로 ResponseStream을 얻고, 결과를 비동기적으로 Byte[]에 쓰는 Task를 빌드합니다... webRquest=[{0}]",
                          webRequest.RequestUri);

            // WebRequest의 Response로부터 ResponseStream을 열고, 거기서 byte[]로 가져옵니다.
            // 이 모든 과정의 각 단계가 모두 비동기적으로 됩니다!!!
            //
            return
                webRequest
                    .GetResponseAsync()
                    .ContinueWith(antecedent => {
                                      var response = antecedent.Result;
                                      return
                                          With.TryFunction(() => response.GetResponseDataByContentEncoding(),
                                                           finallyAction: response.Close);
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        private const string HeaderContentEncoding = @"Content-Encoding";

        /// <summary>
        /// 응답 정보의 헤더에 압축이 되어 있다면, 압축을 풀어서 제공하고, 아니면, 그냥 제공한다.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Stream GetResponseStreamByContentEncoding(this WebResponse response) {
            var stream = response.GetResponseStream();

            if(response.Headers[HeaderContentEncoding].EqualTo("gzip")) {
                if(IsDebugEnabled)
                    log.Debug("gzip 압축으로 응답이 왔습니다. 압축 복원하여 byte배열을 반환합니다...");

                return CompressorAsync.DecompressTask(gzipCompressor, stream).Result;
            }
#if !SILVERLIGHT
            if(response.Headers[HeaderContentEncoding].EqualTo("deflate")) {
                if(IsDebugEnabled)
                    log.Debug("deflate 압축으로 응답이 왔습니다. 압축 복원하여 byte배열을 반환합니다...");

                //var decompressedBytes = deflateCompressor.Decompress(stream.ToBytes());
                //return new MemoryStream(decompressedBytes);
                return CompressorAsync.DecompressTask(deflateCompressor, stream).Result;
            }
#endif
            return stream;
        }

        /// <summary>
        /// 응답 정보의 헤더에 압축이 되어 있다면, 압축을 풀어서 제공하고, 아니면, 그냥 제공한다.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static byte[] GetResponseDataByContentEncoding(this WebResponse response) {
            var responseStream = response.GetResponseStream();

            if(responseStream == null)
                return new byte[0];

            var responseBytes = responseStream.ToBytes();
            responseStream.Close();

            // NOTE: 압축된 데이터가 반환되었을 때, 압축을 풀어서 제공합니다.
            //
            if(response.Headers[HeaderContentEncoding].EqualTo("gzip")) {
                if(IsDebugEnabled)
                    log.Debug("gzip 압축으로 응답이 왔습니다. 압축 복원하여 byte배열을 반환합니다...");

                return gzipCompressor.Decompress(responseBytes);
            }
#if !SILVERLIGHT
            if(response.Headers[HeaderContentEncoding].EqualTo("deflate")) {
                if(IsDebugEnabled)
                    log.Debug("deflate 압축으로 응답이 왔습니다. 압축 복원하여 byte배열을 반환합니다...");

                return deflateCompressor.Decompress(responseBytes);
            }
#endif
            return responseBytes;
        }

        private static readonly SharpGZipCompressor gzipCompressor = new SharpGZipCompressor();
#if !SILVERLIGHT
        private static readonly DeflateCompressor deflateCompressor = new DeflateCompressor();
#endif
    }
}