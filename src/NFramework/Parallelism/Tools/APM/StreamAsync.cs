using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// 비동기적 스트림 작업에 쓰이는 Extension Method 들입니다.
    /// </summary>
    /// <remarks>
    /// 참고 사이트 :
    /// <list>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/ms228963.aspx</item>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/dd997423.aspx</item>
    /// </list>
    /// </remarks>
    public static class StreamAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const int BufferSize = 0x2000;

        /// <summary>
        /// 지정된 스트림을 비동기 방식으로 읽어, <paramref name="buffer"/>에 채웁니다. 작업의 결과는 읽은 바이트 수입니다.
        /// </summary>
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count) {
            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 스트림을 읽어 버퍼에 씁니다... offset=[{0}], count=[{1}]", offset, count);

            // Silverlight용 TPL에서는 지원하지 3개 이상의 인자를 지원하지 않는다.
            var ar = stream.BeginRead(buffer, offset, count, null, null);
            return Task.Factory.StartNew(() => stream.EndRead(ar));
        }

        /// <summary>
        /// <paramref name="buffer"/> 내용을 지정된 스트림에 비동기 방식으로 씁니다.
        /// </summary>
        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count) {
            if(IsDebugEnabled)
                log.Debug("버퍼의 내용을 비동기 방식으로 스트림에 씁니다... offset=[{0}], count=[{1}]", offset, count);

            // Silverlight용 TPL에서는 지원하지 3개 이상의 인자를 지원하지 않는다.
            var ar = stream.BeginWrite(buffer, offset, count, null, null);
            return Task.Factory.StartNew(() => stream.EndWrite(ar));
        }

        /// <summary>
        /// <paramref name="stream"/>의 모든 Data를 비동기 방식으로 읽는 작업을 생성합니다.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Task<byte[]> ReadAllBytesAsync(this Stream stream) {
            if(IsDebugEnabled)
                log.Debug("스트림의 내용을 비동기 방식으로 모두 읽어 바이트 배열로 반환하는 Task<byte[]>를 빌드합니다.");

            var initialCapacity = stream.CanSeek ? (int)stream.Length : BufferSize;
            var destStream = new MemoryStream(initialCapacity);

            return
                stream
                    .CopyStreamToStreamAsync(destStream)
                    .ContinueWith(antecedent => {
                        var result = destStream.ToArray();
                        destStream.Close();

                        if(IsDebugEnabled)
                            log.Debug("원본 스트림의 내용을 비동기 방식으로 모두 읽어 byte 배열로 만들었습니다.");

                        return result;
                    },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="stream"/> 을 비동기 방식으로 반복해서 읽어드리는 Task를 빌드합니다.
        /// </summary>
        /// <param name="stream">읽을 스트림 객체</param>
        /// <param name="bufferSize">한번에 읽을 버퍼 크기</param>
        /// <param name="bufferAvailable">읽은 버퍼가 유효한 것인지 판단하는 델리게이트</param>
        /// <returns>비동기 방식의 읽기 작업</returns>
        public static Task ReadBufferAsync(this Stream stream, int bufferSize, Action<byte[], int> bufferAvailable) {
            // 지정된 Stream 을 반복해서 읽어들인다. 읽어들인 data는 bufferAvailable delegate에 전달되고, 유효한 데이타인지 판단한다.
            //! NOTE: NET-3.5에서는 Cast<object>로 해야 한다. NET-4.0에서는 as IEnumerable<object> 로 해도 된다.
            //
            return Task.Factory.Iterate(ReadIterator(stream, bufferSize, bufferAvailable).Cast<object>());
        }

        private static IEnumerable<Task> ReadIterator(Stream input, int bufferSize, Action<byte[], int> bufferAvailable) {
            input.ShouldNotBeNull("input");
            bufferSize.ShouldBePositive("bufferSize");
            bufferAvailable.ShouldNotBeNull("bufferAvailable");

            var buffer = new byte[bufferSize];

            while(true) {
                var readTask = input.ReadAsync(buffer, 0, buffer.Length);
                yield return readTask;

                if(readTask.Result <= 0)
                    break;

                bufferAvailable(buffer, readTask.Result);
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// <paramref name="source"/>를 읽어 <paramref name="destinationPath"/> 의 파일에 비동기적으로 씁니다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public static Task CopyStreamToFileAsync(this Stream source, string destinationPath) {
            destinationPath.ShouldNotBeWhiteSpace("destinationPath");

            if(IsDebugEnabled)
                log.Debug("Stream 내용을 지정된 경로의 파일에 비동기 방식으로 저장합니다. destinationPath=[{0}]", destinationPath);

            var destinationStream = FileAsync.OpenWrite(destinationPath);

            return
                CopyStreamToStreamAsync(source, destinationStream)
                    .ContinueWith(task => {
                                      var ex = task.Exception;

                                      if(destinationStream != null)
                                          destinationStream.Close();

                                      if(IsDebugEnabled)
                                          log.Debug("스트림 내용을 모두 읽어 대상 파일에 저장했습니다. destinationPath=[{0}]", destinationPath);

                                      if(ex != null) {
                                          if(log.IsWarnEnabled) {
                                              log.Warn("파일을 복사하는데 실패했습니다. destinationPath=" + destinationPath, ex);
                                          }
                                      }
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }
#endif

        /// <summary>
        /// <paramref name="source"/>를 읽어 <paramref name="destination"/>에 비동기적으로 씁니다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Task CopyStreamToStreamAsync(this Stream source, Stream destination) {
            //source.ShouldNotBeNull("source");
            //destination.ShouldNotBeNull("destination");

            if(IsDebugEnabled)
                log.Debug("원본 스트림에서 대상 스트림으로 비동기 방식으로 복사를 수행합니다...");

            // 지정된 Stream 을 반복해서 복사합니다.
            // NOTE: NET-3.5에서는 Cast<object>로 해야 한다. NET-4.0에서는 as IEnumerable<object> 로 해도 된다.

            return Task.Factory.Iterate(CopyStreamIterator(source, destination));
        }

        private static IEnumerable<Task> CopyStreamIterator(Stream input, Stream output) {
            input.ShouldNotBeNull("input");
            output.ShouldNotBeNull("output");

            if(IsDebugEnabled)
                log.Debug("입출력 스트림 간의 데이타 복사를 수행하는 Task의 열거자를 생성합니다...");

            // 두개의 버퍼를 만든다.
            // 하나는 읽기 전용, 하나는 쓰기 전용. 계속해서 두개의 버퍼 내용을 스위칭하여 사용한다.
            //
            var buffers = new[] { new byte[BufferSize], new byte[BufferSize] };
            var filledBufferNo = 0;
            Task writeTask = null;

            while(true) {
                // 비동기 stream 읽기 작업
                var readTask = input.ReadAsync(buffers[filledBufferNo], 0, buffers[filledBufferNo].Length);

                if(writeTask == null) {
                    yield return readTask;

                    // Wait() 호출은 Task 작업 중에 예외발생 시, 예외를 전파시키기 위해서이다.
                    readTask.Wait();
                }
                else {
                    var tasks = new[] { readTask, writeTask };
                    yield return Task.Factory.WhenAll(tasks);

                    // Wait() 호출은 Task 작업 중에 예외발생시, 예외를 전파시키기 위해서이다.
                    Task.WaitAll(tasks);
                }

                // 더 이상 읽을 데이타가 없다면 중지한다.
                if(readTask.Result <= 0)
                    break;

                // 읽어들인 버퍼 내용을 output stream에 비동기적으로 씁니다.
                writeTask = output.WriteAsync(buffers[filledBufferNo], 0, readTask.Result);

                // Swap buffers
                filledBufferNo ^= 1;
            }
        }
    }
}