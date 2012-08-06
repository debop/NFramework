using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// Data를 대상 Stream에 Background Thread를 통해 쓰게 하는 Stream입니다. 
    /// Network 용 Stream 같은데 사용하면, 실제 쓸 데이터가 있는 경우만 쓰기 작업을 수행하고, 나머지 시간은 작업 대기를 하므로, CPU 사용이 거의 없다.
    /// </summary>
    [Serializable]
    public sealed class TransferStream : AbstractStream {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// The underlying stream to target
        /// </summary>
        private readonly Stream _writableTargetStream;

        /// <summary>
        /// 쓰려는 데이타 조각의 컬렉션
        /// </summary>
        private readonly BlockingCollection<byte[]> _chunks;

        /// <summary>
        /// 백그라운드에서 쓰기 작업에 사용될 Task
        /// </summary>
        private readonly Task _processingTask;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="writableTargetStream">실제 Data가 쓰일 대상 Stream</param>
        public TransferStream(Stream writableTargetStream) {
            writableTargetStream.ShouldNotBeNull("writableTargetStream");
            Guard.Assert(writableTargetStream.CanWrite, "Target stream is not writable.");

            _writableTargetStream = writableTargetStream;
            _chunks = new BlockingCollection<byte[]>();

            // _chunks 에 입력이 들어올 때까지 기다렸다가, 대상 Stream에 쓰는 Background 작업을 정의합니다.
            _processingTask =
                Task.Factory.StartNew(() => {
                                          if(IsDebugEnabled)
                                              log.Debug("스트림 정보 복사를 시작합니다...");

                                          foreach(var chunk in _chunks.GetConsumingEnumerable())
                                              _writableTargetStream.Write(chunk, 0, chunk.Length);

                                          if(IsDebugEnabled)
                                              log.Debug("스트림 정보 복사를 완료되었습니다!!!");
                                      });
        }

        /// <summary>
        /// 쓰기 가능한 Stream 인지를 나타냄 (쓰기 가능함)
        /// </summary>
        public override bool CanWrite {
            get { return true; }
        }

        /// <summary>
        /// <paramref name="buffer"/>의 내용을 복사해서 targetStream에 쓰는 작업을 예약합니다.
        /// </summary>
        /// <param name="buffer">대상 배열</param>
        /// <param name="offset">오프셋</param>
        /// <param name="count">쓸 바이트 수</param>
        public override void Write(byte[] buffer, int offset, int count) {
            if(_chunks.IsAddingCompleted)
                return;

            buffer.ShouldNotBeNull("buffer");
            offset.ShouldBeInRange(0, buffer.Length, "offset");
            count.ShouldBeInRange(0, buffer.Length - offset + 1, "count");

            if(count == 0)
                return;

            if(_chunks.IsAddingCompleted)
                return;

            var chunk = new byte[count];
            Buffer.BlockCopy(buffer, offset, chunk, 0, count);

            if(_chunks.IsAddingCompleted)
                return;

            _chunks.Add(chunk);
        }

        /// <summary>
        /// 스트림을 닫고, 모든 관련 리소스를 해제합니다.
        /// </summary>
        public override void Close() {
            if(IsDebugEnabled)
                log.Debug("더 이상 전달할 데이타를 받지않고, 전달 작업을 완료하도록 합니다...");

            try {
                // 더 이상 입력이 없음을 알립니다. 이후로 Write를 호출하면 예외가 발생합니다.
                //
                _chunks.CompleteAdding();

                With.TryActionAsync(() => _processingTask.Wait(), age => age.Handle(ex => true));

                base.Close();

                if(IsDebugEnabled)
                    log.Debug("모든 전달 작업이 완료되었습니다!!!");
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("전달 작업을 완료하는데 실패했습니다!!!", ex);
                throw;
            }
        }
    }
}