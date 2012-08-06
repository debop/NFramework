using System.IO;
using System.Text;
using System.Threading.Tasks;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// APM 방식의 비동기 파일 작업을 제공합니다.
    /// </summary>
    /// <remarks>
    /// 참고 사이트 :
    /// <list>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/ms228963.aspx</item>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/dd997423.aspx</item>
    /// </list>
    /// </remarks>
    public static class FileAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const int BufferSize = 0x2000;

        /// <summary>
        /// 비동기적으로 읽기 위한 FileStream 을 생성합니다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileStream OpenRead(string path) {
            path.ShouldNotBeWhiteSpace("path");

            if(IsDebugEnabled)
                log.Debug("파일을 비동기적으로 읽기 위해 파일을 엽니다...  path=[{0}]", path);

            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, true);
        }

        /// <summary>
        /// 지정한 파일을 비동기적으로 쓰기 위한 FileStream을 생성합니다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileStream OpenWrite(string path) {
            path.ShouldNotBeWhiteSpace("path");

            if(IsDebugEnabled)
                log.Debug("파일에 비동기적으로 Data를 쓰기 위해 파일을 엽니다... path=[{0}]", path);

            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, BufferSize, true);
        }

        /// <summary>
        /// 지정한 파일을 비동기적으로 읽어 byte[] 을 반환하는 <see cref="Task"/>를 빌드합니다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<byte[]> ReadAllBytes(string path) {
            var fileStream = OpenRead(path);

            var asyncRead = fileStream.ReadAllBytesAsync();

            // 파일을 비동기적으로 다 읽고 나서, 파일 Stream을 닫는 후처리 작업을 지정하여 반환한다.
            //
            var closedFile = asyncRead.ContinueWith(antecedent => {
                                                        if(IsDebugEnabled)
                                                            log.Debug("파일 내용을 모두 읽었습니다. FileStream을 닫습니다. path=[{0}]", path);

                                                        With.TryAction(() => {
                                                                           if(fileStream != null)
                                                                               fileStream.Close();
                                                                       });

                                                        return antecedent.Result;
                                                    },
                                                    TaskContinuationOptions.ExecuteSynchronously);

            return closedFile;
        }

        /// <summary>
        /// 지정한 경로의 파일 내용을 비동기적으로 모두 읽어 문자열로 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="path">읽을 파일 전체경로</param>
        /// <param name="encoding">파일 내용 인코딩 방식</param>
        /// <returns>파일 내용을 문자열로 반환하는 작업</returns>
        public static Task<string> ReadAllText(string path, Encoding encoding = null) {
            path.ShouldNotBeWhiteSpace("path");
            encoding.ShouldNotBeNull("encoding");

            encoding = encoding ?? Encoding.UTF8;

            if(IsDebugEnabled)
                log.Debug("파일 내용을 비동기적으로 모두 읽어, 반환하는 작업을 빌드합니다. path=[{0}], encoding=[{1}]", path, encoding);

            var fileStream = OpenRead(path);
            fileStream.ShouldNotBeNull("fileStream");

            var builder = new StringBuilder();

            // 실제 파일 내용을 읽는 작업
            var asyncRead = fileStream.ReadBufferAsync(BufferSize,
                                                       (buffer, count) => builder.Append(encoding.GetString(buffer, 0, count)));

            // 파일을 비동기적으로 다 읽고 나서, 파일 Stream을 닫는 후처리 작업을 지정하고 반환한다.
            //
            var closedFileTask =
                asyncRead.ContinueWith(antecedent => {
                                           if(IsDebugEnabled)
                                               log.Debug("비동기적으로 파일을 모두 읽었습니다... FileStream을 닫습니다... file=[{0}]", path);

                                           var ex = antecedent.Exception;

                                           With.TryAction(() => {
                                                              if(fileStream != null)
                                                                  fileStream.Close();
                                                          });

                                           if(ex != null)
                                               throw ex;

                                           return builder.ToString();
                                       },
                                       TaskContinuationOptions.ExecuteSynchronously);

            return closedFileTask;
        }

        /// <summary>
        /// 비동기적으로 <paramref name="bytes"/> 내용을 파일에 씁니다.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Task WriteAllBytes(string path, byte[] bytes) {
            path.ShouldNotBeWhiteSpace("path");
            bytes.ShouldNotBeNull("bytes");

            if(IsDebugEnabled)
                log.Debug("파일에 데이타를 비동기적으로 저장합니다... path=[{0}], bytes=[{1}]", path, ArrayTool.Copy(bytes, 0, 80).BytesToHex());

            var fileStream = OpenWrite(path);

            return
                fileStream
                    .WriteAsync(bytes, 0, bytes.Length)
                    .ContinueWith(antecedent => {
                                      var ex = antecedent.Exception;

                                      With.TryAction(() => {
                                                         if(fileStream != null)
                                                             fileStream.Close();
                                                     });
                                      if(ex != null)
                                          throw ex;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// 비동기적으로 지정된 내용을 지정된 인코딩 방식으로 파일에 씁니다.
        /// </summary>
        /// <param name="path">경로</param>
        /// <param name="contents">쓸 내용</param>
        /// <param name="encoding">인코딩 방식</param>
        /// <returns>파일 쓰기 작업</returns>
        public static Task WriteAllText(string path, string contents, Encoding encoding = null) {
            path.ShouldNotBeWhiteSpace("path");

            encoding = encoding ?? Encoding.UTF8;

            if(IsDebugEnabled)
                log.Debug("파일에 컨텐츠를 비동기적으로 저장합니다... path=[{0}], encoding=[{1}], contents=[{2}]", path, encoding,
                          contents.EllipsisChar(80));

            // contents 를 인코딩하여 byte[] 로 만듭니다.
            var encoded = Task<byte[]>.Factory.StartNew(state => encoding.GetBytes((string)state), contents);

            // 인코딩이 다 되면, 인코딩된 내용을 지정한 파일에 쓰는 Task를 정의합니다.
            // 이 쓰기 Task를 반환하는 Task (Task<Task> 를 래핑해제한 쓰기 작업 Task를 반환합니다.
            return
                encoded
                    .ContinueWith(antecedent => WriteAllBytes(path, antecedent.Result))
                    .Unwrap();
        }
    }
}