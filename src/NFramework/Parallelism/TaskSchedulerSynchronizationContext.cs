using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism {

    /// <summary>
    /// <see cref="TaskScheduler"/>에 의해 동기화 컨텍스트에 Post, Send 작업을 스케쥴링합니다.
    /// </summary>
    public class TaskSchedulerSynchronizationContext : SynchronizationContext {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly TaskScheduler _scheduler;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="scheduler">작업 스케쥴러</param>
        public TaskSchedulerSynchronizationContext(TaskScheduler scheduler) {
            scheduler.ShouldNotBeNull("scheduler");
            _scheduler = scheduler;
        }

        /// <summary>
        /// 파생 클래스에서 재정의될 때 비동기 메시지를 동기화 컨텍스트로 디스패치합니다.
        /// </summary>
        /// <param name="callback">호출할 <see cref="T:System.Threading.SendOrPostCallback"/> 대리자입니다.</param>
        /// <param name="state">대리자에 전달된 개체입니다.</param><filterpriority>2</filterpriority>
        public override void Post(SendOrPostCallback callback, object state) {
            callback.ShouldNotBeNull("callback");

            if(IsDebugEnabled)
                log.Debug("SynchronizationContext 하에서, Post 호출합니다... state=[{0}]", state);

            Task.Factory.StartNew(() => callback(state), CancellationToken.None, TaskCreationOptions.None, _scheduler);

            if(IsDebugEnabled)
                log.Debug("SynchronizationContext 하에서, Post 호출을 완료하였습니다.");
        }

        /// <summary>
        /// 파생 클래스에서 재정의될 때 동기 메시지를 동기화 컨텍스트로 디스패치합니다.
        /// </summary>
        /// <param name="callback">호출할 <see cref="T:System.Threading.SendOrPostCallback"/> 대리자입니다.</param>
        /// <param name="state">대리자에 전달된 개체입니다.</param><filterpriority>2</filterpriority>
        public override void Send(SendOrPostCallback callback, object state) {
            callback.ShouldNotBeNull("callback");

            if(IsDebugEnabled)
                log.Debug("SynchronizationContext 하에서, Send 호출합니다... state=[{0}]", state);

            With.TryActionAsync(() => {
                                    var task = new Task(() => callback(state));
                                    task.RunSynchronously(_scheduler);
                                    task.Wait();
                                });

            if(IsDebugEnabled)
                log.Debug("SynchronizationContext 하에서, Send 호출을 완료하였습니다.");
        }
    }
}