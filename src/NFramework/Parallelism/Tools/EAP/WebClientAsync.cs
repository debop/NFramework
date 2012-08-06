using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="WebClient"/>를 EAP (Event-driven Asynchronous Pattern) 방식의 비동기 작업을 수행하는 확장 메소드를 제공합니다.<br />
    /// </summary>
    /// <remarks>
    /// 참고사이트:
    /// <list>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/wewwczdw.aspx</item>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/dd997423.aspx</item>
    /// </list>
    /// </remarks>
    public static class WebClientAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly ICompressor _gzip = new SharpGZipCompressor(); //new GZipCompressor();
        private static readonly ICompressor _deflate = new IonicDeflateCompressor();

#if !SILVERLIGHT

        /// <summary>
        /// <paramref name="address"/>의 리소스를 비동기적으로 다운받아 byte array로 반환하는 Task{byte[]}를 빌드합니다.
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="address">리소스 위치</param>
        /// <returns></returns>
        public static Task<byte[]> DownloadDataTask(this WebClient webClient, string address) {
            address.ShouldNotBeWhiteSpace("address");
            return DownloadDataTask(webClient, new Uri(address));
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스를 비동기적으로 다운받아 byte array로 반환하는 Task{byte[]}를 빌드합니다.
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="address">리소스 위치</param>
        /// <returns></returns>
        public static Task<byte[]> DownloadDataTask(this WebClient webClient, Uri address) {
            var tcs = new CancellationTokenSource();
            return DownloadDataTask(webClient, tcs.Token, address);
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스를 비동기적으로 다운받아 byte array로 반환하는 Task{byte[]}를 빌드합니다.
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="token">작업 취소를 위한 Token</param>
        /// <param name="address">리소스 위치</param>
        /// <returns></returns>
        public static Task<byte[]> DownloadDataTask(this WebClient webClient, CancellationToken token, string address) {
            return DownloadDataTask(webClient, token, new Uri(address));
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스를 비동기적으로 다운받아 byte array로 반환하는 Task{byte[]}를 빌드합니다.
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="token">작업 취소를 위한 Token</param>
        /// <param name="address">리소스 위치</param>
        /// <returns></returns>
        public static Task<byte[]> DownloadDataTask(this WebClient webClient, CancellationToken token, Uri address) {
            webClient.ShouldNotBeNull("webClient");
            token.ShouldNotBeNull("token");
            address.ShouldNotBeNull("address");

            if(IsDebugEnabled)
                log.Debug("WebClient를 이용하여 지정된 주소로부터 리소스를 비동기 방식으로 다운받습니다... address=[{0}]", address.AbsoluteUri);

            var tcs = new TaskCompletionSource<byte[]>(address);
            token.Register(webClient.CancelAsync);

            // 비동기 완료 시의 처리를 정의합니다.
            DownloadDataCompletedEventHandler handler = null;
            handler =
                (sender, args) =>
                EventAsyncPattern.HandleCompletion(tcs, args, () => args.Result, () => webClient.DownloadDataCompleted -= handler);
            webClient.DownloadDataCompleted += handler;

            try {
                webClient.DownloadDataAsync(address, tcs);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("WebClient를 이용하여 리소스 Data를 비동기적으로 다운받는데 실패했습니다. address=[{0}]", address.AbsoluteUri);
                    log.Warn(ex);
                }

                webClient.DownloadDataCompleted -= handler;
                tcs.TrySetException(ex);
            }

            var result = tcs.Task;

            if(result.IsCompleted)
                result = result.ContinueWith(antecedent => DecompressByContentEncoding(webClient, antecedent.Result),
                                             TaskContinuationOptions.ExecuteSynchronously);
            return result;
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스 정보를 파일로 저장합니다.
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="address">다운 받을 리소스의 주소</param>
        /// <param name="filename">다운로드 리소스를 저장할 로컬 파일명</param>
        /// <returns></returns>
        public static Task DownloadFileTask(this WebClient webClient, string address, string filename) {
            address.ShouldNotBeWhiteSpace("address");
            return DownloadFileTask(webClient, new Uri(address), filename);
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스 정보를 비동기적으로 다운받아 파일로 저장합니다.
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="address">다운 받을 리소스의 주소</param>
        /// <param name="filename">다운로드 리소스를 저장할 로컬 파일명</param>
        /// <returns></returns>
        public static Task DownloadFileTask(this WebClient webClient, Uri address, string filename) {
            webClient.ShouldNotBeNull("webClient");
            address.ShouldNotBeNull("address");
            filename.ShouldNotBeWhiteSpace("filename");

            if(IsDebugEnabled)
                log.Debug("WebClient를 이용하여 지정된 주소의 리소스를 비동기적으로 다운받아 파일로 저장합니다... address=[{0}], filename=[{1}]",
                          address.AbsoluteUri, filename);


            // NOTE: 압축 통신을 대비해서, Byte 통신 후, 저장한다.
            //
            return
                DownloadDataTask(webClient, address)
                    .ContinueWith(antecedent => FileAsync.WriteAllBytes(filename, antecedent.Result),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스 정보를 비동기적으로 다운받아 파일로 저장합니다.
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="token">작업 취소용 Token</param>
        /// <param name="address">다운 받을 리소스의 주소</param>
        /// <param name="filename">다운로드 리소스를 저장할 로컬 파일명</param>
        /// <returns></returns>
        public static Task DownloadFileTask(this WebClient webClient, CancellationToken token, string address, string filename) {
            return DownloadFileTask(webClient, token, new Uri(address), filename);
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스 정보를 비동기적으로 다운받아 파일로 저장합니다.
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="token">작업 취소용 Token</param>
        /// <param name="address">다운 받을 리소스의 주소</param>
        /// <param name="filename">다운로드 리소스를 저장할 로컬 파일명</param>
        /// <returns></returns>
        public static Task DownloadFileTask(this WebClient webClient, CancellationToken token, Uri address, string filename) {
            webClient.ShouldNotBeNull("webClient");
            token.ShouldNotBeNull("token");
            address.ShouldNotBeNull("address");
            filename.ShouldNotBeWhiteSpace("filename");

            if(IsDebugEnabled)
                log.Debug("WebClient를 이용하여 지정된 주소의 리소스를 비동기적으로 다운받아 파일로 저장합니다... address=[{0}], filename=[{1}]",
                          address.AbsoluteUri, filename);


            // NOTE: 압축 통신을 대비해서, Byte 통신 후, 저장한다.
            //
            return
                DownloadDataTask(webClient, token, address)
                    .ContinueWith(antecedent => FileAsync.WriteAllBytes(filename, antecedent.Result),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

#endif

#if SILVERLIGHT

    /// <summary>
    /// <paramref name="address"/>의 리소스를 비동기적으로 다운받아 byte array로 반환하는 Task{byte[]}를 빌드합니다.
    /// </summary>
    /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
    /// <param name="token">작업 취소를 위한 Token</param>
    /// <param name="address">리소스 위치</param>
    /// <returns></returns>
		private static Task<string> DownloadStringTaskInternal(this WebClient webClient, CancellationToken token, Uri address)
		{
			webClient.ShouldNotBeNull("webClient");
			token.ShouldNotBeNull("token");
			address.ShouldNotBeNull("address");

			if(IsDebugEnabled)
				log.Debug("WebClient를 이용하여 지정된 주소로부터 리소스를 비동기 방식으로 다운받습니다... address=[{0}]", address.AbsoluteUri);

			var tcs = new TaskCompletionSource<string>(address);
			token.Register(webClient.CancelAsync);

			// 비동기 완료 시의 처리를 정의합니다.
			DownloadStringCompletedEventHandler handler = null;
			handler = (sender, args) => EAPCommon.HandleCompletion(tcs, args, () => args.Result, () => webClient.DownloadStringCompleted -= handler);
			webClient.DownloadStringCompleted += handler;

			try
			{
				webClient.DownloadStringAsync(address, tcs);
			}
			catch(Exception ex)
			{
				if(log.IsWarnEnabled)
				{
					log.Warn("WebClient를 이용하여 리소스 Data를 비동기적으로 다운받는데 실패했습니다. address=[{0}]", address.AbsoluteUri);
					log.Warn(ex);
				}

				webClient.DownloadStringCompleted -= handler;
				tcs.TrySetException(ex);
			}

			var result = tcs.Task;

			//if(result.IsCompleted)
			//    result = result.ContinueWith(antecedent => DecompressByContentEncoding(webClient, antecedent.Result),
			//                                 TaskContinuationOptions.ExecuteSynchronously);
			return result;
		}
#endif

        /// <summary>
        /// 지정된 Uri로부터, 리소스를 문자열로 비동기적으로 다운 받습니다. 
        /// 인코딩 문제가 있을 때에는 <see cref="DownloadDataTask(System.Net.WebClient,string)"/>를 이용하여 Encoding을 직접 설정해 주시기 바랍니다.
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static Task<string> DownloadStringTask(this WebClient webClient, string address) {
            address.ShouldNotBeWhiteSpace("address");
            return DownloadStringTask(webClient, new Uri(address));
        }

        /// <summary>
        /// 지정된 Uri로부터, 리소스를 문자열로 비동기적으로 다운 받습니다. 
        /// 인코딩 문제가 있을 때에는 <see cref="DownloadDataTask(System.Net.WebClient,System.Uri)"/>를 이용하여 Encoding을 직접 설정해 주시기 바랍니다.
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="address">리소스 위치</param>
        /// <returns></returns>
        public static Task<string> DownloadStringTask(this WebClient webClient, Uri address) {
            webClient.ShouldNotBeNull("webClient");
            address.ShouldNotBeNull("address");

            if(IsDebugEnabled)
                log.Debug("WebClient로 지정된 주소로부터 비동기적으로 문자열을 다운받습니다... address=[{0}]", address.AbsoluteUri);

            // NOTE: 압축 통신시 응답 정보가 압축되어 있을 경우, 압축해제해서 결과를 반환해야 한다. 그래서 DownloadDataTask를 사용한다.
            //
#if !SILVERLIGHT
            return
                DownloadDataTask(webClient, address)
                    .ContinueWith(antecedent => antecedent.Result.ToText(),
                                  TaskContinuationOptions.ExecuteSynchronously);
#else
			return DownloadStringTaskInternal(webClient, CancellationToken.None, address);

#endif
        }

        /// <summary>
        /// 지정된 Uri로부터, 리소스를 문자열로 비동기적으로 다운 받습니다. 
        /// 인코딩 문제가 있을 때에는 <see cref="DownloadDataTask(System.Net.WebClient,System.Uri)"/>를 이용하여 Encoding을 직접 설정해 주시기 바랍니다.
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="token">작업 취소를 위한 Token</param>
        /// <param name="address">리소스 위치</param>
        /// <returns></returns>
        public static Task<string> DownloadStringTask(this WebClient webClient, CancellationToken token, string address) {
            return DownloadStringTask(webClient, token, new Uri(address));
        }

        /// <summary>
        /// 지정된 Uri로부터, 리소스를 문자열로 비동기적으로 다운 받습니다. 
        /// 인코딩 문제가 있을 때에는 <see cref="DownloadDataTask(System.Net.WebClient,System.Uri)"/>를 이용하여 Encoding을 직접 설정해 주시기 바랍니다.
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="token">작업 취소를 위한 Token</param>
        /// <param name="address">리소스 위치</param>
        /// <returns></returns>
        public static Task<string> DownloadStringTask(this WebClient webClient, CancellationToken token, Uri address) {
            webClient.ShouldNotBeNull("webClient");
            token.ShouldNotBeNull("token");
            address.ShouldNotBeNull("address");

            if(IsDebugEnabled)
                log.Debug("WebClient로 지정된 주소로부터 비동기적으로 문자열을 다운받습니다... address=[{0}]", address.AbsoluteUri);

            // NOTE: 압축 통신시 응답 정보가 압축되어 있을 경우, 압축해제해서 결과를 반환해야 한다. 그래서 DownloadDataTask를 사용한다.
            //
#if !SILVERLIGHT
            return
                DownloadDataTask(webClient, token, address)
                    .ContinueWith(antecedent => antecedent.Result.ToText(),
                                  TaskContinuationOptions.ExecuteSynchronously);
#else
			return DownloadStringTaskInternal(webClient, CancellationToken.None, address);
#endif
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스 정보를 읽기 위해 <see cref="Stream"/>을 비동기적으로 엽니다.
        /// </summary>
        /// <param name="webClient">WebClient 인스턴스</param>
        /// <param name="address">리소스의 주소</param>
        /// <returns></returns>
        public static Task<Stream> OpenReadTask(this WebClient webClient, string address) {
            address.ShouldNotBeWhiteSpace("address");
            return OpenReadTask(webClient, new Uri(address));
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스 정보를 읽기 위해 <see cref="Stream"/>을 비동기적으로 엽니다.
        /// </summary>
        /// <param name="webClient">WebClient 인스턴스</param>
        /// <param name="address">리소스의 주소</param>
        /// <returns></returns>
        public static Task<Stream> OpenReadTask(this WebClient webClient, Uri address) {
            return OpenReadTask(webClient, CancellationToken.None, address);
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스 정보를 읽기 위해 <see cref="Stream"/>을 비동기적으로 엽니다.
        /// </summary>
        /// <param name="webClient">WebClient 인스턴스</param>
        /// <param name="token">작업 취소를 위한 Token</param>
        /// <param name="address">리소스의 주소</param>
        /// <returns></returns>
        public static Task<Stream> OpenReadTask(this WebClient webClient, CancellationToken token, string address) {
            address.ShouldNotBeWhiteSpace("address");
            return OpenReadTask(webClient, token, new Uri(address));
        }

        /// <summary>
        /// <paramref name="address"/>의 리소스 정보를 읽기 위해 <see cref="Stream"/>을 비동기적으로 엽니다.
        /// </summary>
        /// <param name="webClient">WebClient 인스턴스</param>
        /// <param name="token">작업 취소를 위한 Token</param>
        /// <param name="address">리소스의 주소</param>
        /// <returns></returns>
        public static Task<Stream> OpenReadTask(this WebClient webClient, CancellationToken token, Uri address) {
            webClient.ShouldNotBeNull("webClient");
            address.ShouldNotBeNull("address");

            if(IsDebugEnabled)
                log.Debug("지정된 주소로부터 리소스를 비동기 방식으로 다운로드 받아 Stream으로 변환합니다... address=[{0}]", address.AbsoluteUri);

            // NOTE: 압축 통신시 응답 정보가 압축되어 있을 경우, 압축해제해서 결과를 반환해야 한다. 그래서 DownloadDataTask를 사용한다.
            //
#if !SILVERLIGHT
            return
                DownloadDataTask(webClient, token, address)
                    .ContinueWith(antecedent => antecedent.Result.ToStream(),
                                  TaskContinuationOptions.ExecuteSynchronously);
#else
			return OpenReadTaskInternal(webClient, token, address);
#endif
        }

#if SILVERLIGHT
		private static Task<Stream> OpenReadTaskInternal(WebClient webClient, CancellationToken token, Uri address)
		{
			var tcs = new TaskCompletionSource<Stream>(address);
			token.Register(webClient.CancelAsync);

			OpenReadCompletedEventHandler handler = null;
			handler = (s, e) => EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => webClient.OpenReadCompleted -= handler);
			webClient.OpenReadCompleted += handler;

			try
			{
				webClient.OpenReadAsync(address, tcs);
			}
			catch(Exception ex)
			{
				if(log.IsWarnEnabled)
				{
					log.Warn("WebClient를 이용하여 데이타 읽기용 Stream을 오픈하는데 실패했습니다. address=[{0}]", address.AbsoluteUri);
					log.Warn(ex);
				}

				webClient.OpenReadCompleted -= handler;
				tcs.TrySetException(ex);
			}

			return tcs.Task;
		}
#endif

        /// <summary>
        /// <paramref name="address"/>에 비동기적으로 data를 전송하기 위한 쓰기용 Stream을 반환합니다.
        /// </summary>
        /// <param name="webClient">WebClient 인스턴스</param>
        /// <param name="address">리소스의 주소</param>
        /// <param name="method">전송 방법 : Http인 경우는 POST, FTP인 경우는 STOR입니다.</param>
        /// <returns></returns>
        public static Task<Stream> OpenWriteTask(this WebClient webClient, string address, string method) {
            address.ShouldNotBeWhiteSpace("address");
            return OpenWriteTask(webClient, new Uri(address), method);
        }

        /// <summary>
        /// <paramref name="address"/>에 비동기적으로 data를 전송하기 위한 쓰기용 Stream을 반환합니다.
        /// </summary>
        /// <param name="webClient">WebClient 인스턴스</param>
        /// <param name="address">리소스의 주소</param>
        /// <param name="method">전송 방법 : Http인 경우는 POST, FTP인 경우는 STOR입니다.</param>
        /// <returns></returns>
        public static Task<Stream> OpenWriteTask(this WebClient webClient, Uri address, string method) {
            return OpenWriteTask(webClient, CancellationToken.None, address, method);
        }

        /// <summary>
        /// <paramref name="address"/>에 비동기적으로 data를 전송하기 위한 쓰기용 Stream을 반환합니다.
        /// </summary>
        /// <param name="webClient">WebClient 인스턴스</param>
        /// <param name="token">작업 취소를 위한 Token</param>
        /// <param name="address">리소스의 주소</param>
        /// <param name="method">전송 방법 : Http인 경우는 POST, FTP인 경우는 STOR입니다.</param>
        /// <returns></returns>
        public static Task<Stream> OpenWriteTask(this WebClient webClient, CancellationToken token, string address, string method) {
            address.ShouldNotBeWhiteSpace("address");
            return OpenWriteTask(webClient, token, new Uri(address), method);
        }

        /// <summary>
        /// <paramref name="address"/>에 비동기적으로 data를 전송하기 위한 쓰기용 Stream을 반환합니다.
        /// </summary>
        /// <param name="webClient">WebClient 인스턴스</param>
        /// <param name="token">작업 취소를 위한 Token</param>
        /// <param name="address">리소스의 주소</param>
        /// <param name="method">전송 방법 : Http인 경우는 POST, FTP인 경우는 STOR입니다.</param>
        /// <returns></returns>
        public static Task<Stream> OpenWriteTask(this WebClient webClient, CancellationToken token, Uri address, string method) {
            webClient.ShouldNotBeNull("webClient");
            token.ShouldNotBeNull("token");
            address.ShouldNotBeNull("address");

            if(IsDebugEnabled)
                log.Debug("지정된 주소에 데이타 쓰기용 Stream을 엽니다... address=[{0}], method=[{1}]", address.AbsoluteUri, method);


            var tcs = new TaskCompletionSource<Stream>(address);
            token.Register(webClient.CancelAsync);

            OpenWriteCompletedEventHandler handler = null;
            handler =
                (s, e) => EventAsyncPattern.HandleCompletion(tcs, e, () => e.Result, () => webClient.OpenWriteCompleted -= handler);
            webClient.OpenWriteCompleted += handler;

            try {
                webClient.OpenWriteAsync(address, method, tcs);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("WebClient를 이용하여 데이타를 전송하기 위해 쓰기용 Stream을 오픈하는데 실패했습니다. address=[{0}]", address.AbsoluteUri);
                    log.Warn(ex);
                }

                webClient.OpenWriteCompleted -= handler;
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

#if !SILVERLIGHT

        /// <summary>
        /// <paramref name="address"/>에 <paramref name="data"/>를 비동기적으로 전송합니다.
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="address">데이타를 전송할 주소</param>
        /// <param name="method">데이타 전송 방법 (HTTP는 POST, FTP는 STOR)</param>
        /// <param name="data">전송할 데이타</param>
        /// <returns></returns>
        public static Task<byte[]> UploadDataTask(this WebClient webClient, string address, string method, byte[] data) {
            address.ShouldNotBeWhiteSpace("address");
            return UploadDataTask(webClient, new Uri(address), method, data);
        }

        /// <summary>
        /// <paramref name="address"/>에 <paramref name="data"/>를 비동기적으로 전송합니다.
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="address">데이타를 전송할 주소</param>
        /// <param name="method">데이타 전송 방법 (HTTP는 POST, FTP는 STOR)</param>
        /// <param name="data">전송할 데이타</param>
        /// <returns></returns>
        public static Task<byte[]> UploadDataTask(this WebClient webClient, Uri address, string method, byte[] data) {
            webClient.ShouldNotBeNull("webClient");
            address.ShouldNotBeNull("address");
            data.ShouldNotBeNull("data");

            if(IsDebugEnabled)
                log.Debug("지정된 주소에 데이타를 비동기 방식으로 전송합니다... address=[{0}], method=[{1}]", address.AbsoluteUri, method);

            var tcs = new TaskCompletionSource<byte[]>(address);

            UploadDataCompletedEventHandler handler = null;
            handler =
                (s, e) => EventAsyncPattern.HandleCompletion(tcs, e, () => e.Result, () => webClient.UploadDataCompleted -= handler);
            webClient.UploadDataCompleted += handler;

            try {
                webClient.UploadDataAsync(address, method ?? "POST", data, tcs);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("WebClient를 이용하여 데이타를 비동기 전송에 실패했습니다. address=[{0}]" + address.AbsoluteUri);
                    log.Warn(ex);
                }

                webClient.UploadDataCompleted -= handler;
                tcs.TrySetException(ex);
            }
            return tcs.Task;
        }

        /// <summary>
        /// <paramref name="address"/>에 <paramref name="filename"/>의 파일을 전송합니다. (HTTP나 FTP나 같습니다)
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="address">전송할 주소</param>
        /// <param name="method">전송 방법 (HTTP는 POST, FTP는 STOR)</param>
        /// <param name="filename">전송할 파일의 전체경로</param>
        /// <returns></returns>
        public static Task<byte[]> UploadFileTask(this WebClient webClient, string address, string method, string filename) {
            address.ShouldNotBeWhiteSpace("address");
            filename.ShouldNotBeWhiteSpace("filename");

            Guard.Assert<FileNotFoundException>(File.Exists(filename), "File not found. filename=[{0}]", filename);

            return UploadFileTask(webClient, new Uri(address), method, filename);
        }

        /// <summary>
        /// <paramref name="address"/>에 <paramref name="filename"/>의 파일을 전송합니다. (HTTP나 FTP나 같습니다)
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="address">전송할 주소</param>
        /// <param name="method">전송 방법 (HTTP는 POST, FTP는 STOR)</param>
        /// <param name="filename">전송할 파일의 전체경로</param>
        /// <returns></returns>
        public static Task<byte[]> UploadFileTask(this WebClient webClient, Uri address, string method, string filename) {
            webClient.ShouldNotBeNull("webClient");
            address.ShouldNotBeNull("address");
            filename.ShouldNotBeWhiteSpace("filename");
            Guard.Assert<FileNotFoundException>(File.Exists(filename), "File not found. filename=[{0}]", filename);

            if(IsDebugEnabled)
                log.Debug("지정된 주소에 파일을 비동기 방식으로 Upload합니다... address=[{0}], method=[{1}], filename=[{2}]", address.AbsoluteUri, method,
                          filename);

            var tcs = new TaskCompletionSource<byte[]>(address);

            UploadFileCompletedEventHandler handler = null;
            handler =
                (s, e) => EventAsyncPattern.HandleCompletion(tcs, e, () => e.Result, () => webClient.UploadFileCompleted -= handler);
            webClient.UploadFileCompleted += handler;

            try {
                webClient.UploadFileAsync(address, method, filename, tcs);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("WebClient를 이용하여 파일을 비동기 방식 Upload에 실패했습니다. address=[{0}]" + address.AbsoluteUri);
                    log.Warn(ex);
                }

                webClient.UploadFileCompleted -= handler;
                tcs.TrySetException(ex);
            }
            return tcs.Task;
        }

#endif

        /// <summary>
        /// <paramref name="address"/>에 <paramref name="data"/> 문자열을 전송합니다. (HTTP나 FTP나 같습니다)
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="address">전송할 주소</param>
        /// <param name="method">전송 방법 (HTTP는 POST, FTP는 STOR)</param>
        /// <param name="data">전송할 문자열</param>
        /// <returns></returns>
        public static Task<string> UploadStringTask(this WebClient webClient, string address, string method, string data) {
            address.ShouldNotBeWhiteSpace("address");
            // data.ShouldNotBeEmpty("data");

            return UploadStringTask(webClient, new Uri(address), method, data);
        }

        /// <summary>
        /// <paramref name="address"/>에 <paramref name="data"/> 문자열을 전송합니다. (HTTP나 FTP나 같습니다)
        /// </summary>
        /// <param name="webClient"><see cref="WebClient"/> 인스턴스</param>
        /// <param name="address">전송할 주소</param>
        /// <param name="method">전송 방법 (HTTP는 POST, FTP는 STOR)</param>
        /// <param name="data">전송할 문자열</param>
        /// <returns></returns>
        public static Task<string> UploadStringTask(this WebClient webClient, Uri address, string method, string data) {
            webClient.ShouldNotBeNull("webClient");
            address.ShouldNotBeNull("address");
            // data.ShouldNotBeEmpty("data");

            if(IsDebugEnabled)
                log.Debug("지정된 주소에 문자열을 비동기 Upload합니다... address=[{0}], method=[{1}], data=[{2}]",
                          address.AbsoluteUri, method, data.EllipsisChar(255));

            var tcs = new TaskCompletionSource<string>(address);

            UploadStringCompletedEventHandler handler = null;
            handler =
                (s, e) => EventAsyncPattern.HandleCompletion(tcs, e, () => e.Result, () => webClient.UploadStringCompleted -= handler);
            webClient.UploadStringCompleted += handler;

            try {
                webClient.UploadStringAsync(address, method, data, tcs);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("WebClient를 이용하여 문자열을 비동기 Upload 하는데 실패했습니다. address=[{0}]", address.AbsoluteUri);
                    log.Warn(ex);
                }

                webClient.UploadStringCompleted -= handler;
                tcs.TrySetException(ex);
            }
            return tcs.Task;
        }

        /// <summary>
        /// 응답 바이트가 압축되어 있다면, 압축을 복원해서 반환합니다.
        /// </summary>
        private static byte[] DecompressByContentEncoding(WebClient webClient, byte[] responseBytes) {
            if(responseBytes == null || responseBytes.Length == 0)
                return new byte[0];

            const string ContentEncoding = @"Content-Encoding";
            var contentEncoding = webClient.ResponseHeaders[ContentEncoding];

            if("gzip".Compare(contentEncoding, true) == 0)
                return _gzip.Decompress(responseBytes);

            if("deflate".Compare(contentEncoding, true) == 0)
                return _deflate.Decompress(responseBytes);

            return responseBytes;
        }
    }
}