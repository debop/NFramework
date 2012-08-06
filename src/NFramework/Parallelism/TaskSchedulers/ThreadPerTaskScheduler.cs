using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.TaskSchedulers {
    /// <summary>
    /// 작업당 하나의 Thread가 할당되는 스케쥴러입니다.
    /// </summary>
    [Serializable]
    public class ThreadPerTaskScheduler : TaskScheduler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="T:System.Threading.Tasks.Task"/>를 스케줄러의 큐에 대기합니다.
        /// </summary>
        /// <param name="task">큐에 대기할 <see cref="T:System.Threading.Tasks.Task"/>입니다.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="task"/> 인수가 null인 경우</exception>
        protected override void QueueTask(Task task) {
            if(IsDebugEnabled)
                log.Debug("작업당 스레드를 만드는 스케쥴러이므로 작업 예약이 들어오면, 무조건 스레드를 만들어서 작업을 시작합니다. task=[{0}]", task);

            var thread = new Thread(() => TryExecuteTask(task)) { IsBackground = true };
            thread.Start();
        }

        /// <summary>
        /// 제공된 <see cref="T:System.Threading.Tasks.Task"/>를 이 호출에서 동기적으로 실행할 수 있는지 확인하고 실행 가능할 경우 실행합니다.
        /// </summary>
        /// <returns>
        /// 작업이 인라인으로 실행되었는지 여부를 나타내는 부울 값입니다.
        /// </returns>
        /// <param name="task">실행할 <see cref="T:System.Threading.Tasks.Task"/>입니다.</param>
        /// <param name="taskWasPreviouslyQueued">
        /// 작업이 이전에 큐에 대기되었는지 여부를 나타내는 부울입니다.
        /// 이 매개 변수가 True이면 작업이 이전에 큐에 대기된 것일 수 있습니다.
        /// False이면 작업이 큐에 대기되지 않은 것입니다. 작업을 큐에 대기하지 않고 인라인으로 실행하려면 이 호출을 수행합니다.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="task"/> 인수가 null인 경우</exception>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="task"/>가 이미 실행되었습니다.</exception>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) {
            return TryExecuteTask(task);
        }

        /// <summary>
        /// 현재 스케줄러의 큐에 대기되어 실행을 기다리고 있는 <see cref="T:System.Threading.Tasks.Task"/> 인스턴스의 열거 가능한 형식을 생성합니다.
        /// </summary>
        /// <returns>
        /// 현재 이 스케줄러의 큐에 대기된 작업의 통과를 허용하는 열거 가능한 형식입니다.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">이 스케줄러는 현재 대기 중인 작업 목록을 생성할 수 없습니다.</exception>
        protected override IEnumerable<Task> GetScheduledTasks() {
            return Enumerable.Empty<Task>();
        }
    }
}