using System;
using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 비동기 Barrier.
    /// 병렬로 특정 메소드를 수행할 때, 초기 지정한 참여자가 모두 신호를 보낼 때까지, 전체 작업 완료를 보류합니다. 즉 장벽을 쳐서, 모두 완료될 때가지 기다립니다. Fork-Join 과 같은 기능입니다.
    /// 참고 : http://msdn.microsoft.com/ko-kr/library/system.threading.barrier.aspx
    /// </summary>
    /// <seealso cref="Barrier"/>
    [Serializable]
    public class AsyncBarrier {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Barrier 참여자의 수
        /// </summary>
        private readonly int _participantCount;

        /// <summary>
        /// 현재 라운드의 완료 설정하는데 사용되는 Task
        /// </summary>
        private TaskCompletionSource<bool> _tcs;

        /// <summary>
        /// 남은 참여자 수 (남은 참여자 수가 0이면 작업이 완료되었다고 본다)
        /// </summary>
        private int _remainingParticipants;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="participantCount">참여자 수 (0보다 커야 합니다)</param>
        public AsyncBarrier() : this(3) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="participantCount">참여자 수 (0보다 커야 합니다)</param>
        public AsyncBarrier(int participantCount) {
            participantCount.ShouldBePositive("participantCount");

            _participantCount = participantCount;
            _remainingParticipants = participantCount;
            _tcs = new TaskCompletionSource<bool>();
        }

        /// <summary>
        /// Participant count
        /// </summary>
        public int ParticipantCount {
            get { return _participantCount; }
        }

        /// <summary>
        /// Remaining participant count
        /// </summary>
        public int RemainingCount {
            get { return _remainingParticipants; }
        }

        /// <summary>
        /// 이 함수를 호출하여, 참여에서 빠져나갔음을 알려주고, 전체 작업이 완료되었는지 알 수 있도록 <see cref="Task"/>를 반환합니다.
        /// 여기서 Task를 반환받아, Wait를 수행하면, 비동기적으로 Barrier에 참여하는 것이 된다.
        /// </summary>
        /// <returns>A Task that will be signaled when the current round completes.</returns>
        public Task SignalAndWait() {
            if(IsDebugEnabled)
                log.Debug("비동기 Barrier에서 시그널 호출을 받았습니다... 현재 Task를 반환합니다... RemainingCount=[{0}]", _remainingParticipants);

            var tcs = _tcs;

            if(Interlocked.Decrement(ref _remainingParticipants) == 0) {
                _remainingParticipants = _participantCount;
                _tcs = new TaskCompletionSource<bool>();

                tcs.SetResult(true);
            }
            return tcs.Task;
        }
    }
}