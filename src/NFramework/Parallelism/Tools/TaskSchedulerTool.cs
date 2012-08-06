using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="TaskScheduler"/> 를 위한 확장 메소드들입니다.
    /// </summary>
    public static class TaskSchedulerTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="scheduler"/>에 의해 메시지 전달 요청 작업이 스케쥴링되는 <see cref="SynchronizationContext"/>를 빌드합니다.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        /// <seealso cref="TaskSchedulerSynchronizationContext"/>
        public static SynchronizationContext ToSynchronizationContext(this TaskScheduler scheduler) {
            if(IsDebugEnabled)
                log.Debug("지정한 스케쥴러를 가지는 SynchrnonizationContext를 생성합니다...");

            return new TaskSchedulerSynchronizationContext(scheduler);
        }
    }
}