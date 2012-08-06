using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 큐에 저장된 Task들을 순차적으로 실행하게 합니다. 
    /// 실행할 Task들을 큐에 넣고, Completed().Wait()를 호출하면 큐의 모든 Task가 순차적으로 실행됩니다.
    /// </summary>
    [Serializable]
    public class SerialTaskQueue {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 순차로 Task를 담고 있는 내부 큐이다.
        /// </summary>
        private readonly Queue<object> _tasks = new Queue<object>();

        /// <summary>
        /// 현재 작업중인 Task. 작업중인 Task가 없다면 null 값ㅇ르 가진다.
        /// </summary>
        private Task _taskInFlight;

        /// <summary>
        /// 지정한 작업 생성 함수로 생성되는 작업을 순차 수행을 위한 작업 큐에 넣습니다.
        /// </summary>
        /// <param name="taskFactory"></param>
        public void Enqueue(Func<Task> taskFactory) {
            taskFactory.ShouldNotBeNull("taskFactory");
            EnqueueInternal(taskFactory);
        }

        /// <summary>
        /// 시작하지 않은 작업을 순차 수행을 위한 작업 큐에 넣습니다.
        /// </summary>
        public Task Enqueue(Task task) {
            task.ShouldNotBeNull("task");
            EnqueueInternal(task);

            return task;
        }

        /// <summary>
        /// 큐에 남은 모든 작업이 완료되도록 한다.
        /// </summary>
        /// <returns></returns>
        public Task Completed() {
            // 더미 작업을 추가한다
            return Enqueue(new Task(() => { }));
        }

        /// <summary>
        /// 내부 큐에 Task를 추가합니다.
        /// </summary>
        /// <param name="taskOrFunction"></param>
        private void EnqueueInternal(object taskOrFunction) {
            taskOrFunction.ShouldNotBeNull("taskOrFunction");

            if(IsDebugEnabled)
                log.Debug("Enqueue Task or Function...");

            lock(_tasks) {
                if(_taskInFlight == null)
                    StartTask_CallUnderLock(taskOrFunction);
                else
                    _tasks.Enqueue(taskOrFunction);
            }
        }

        /// <summary>
        /// 지정된 Task 또는 Task 생성 함수로부터 생성한 작업을 시작합니다.
        /// </summary>
        /// <param name="nextItem"></param>
        private void StartTask_CallUnderLock(object nextItem) {
            var next = nextItem as Task ?? ((Func<Task>)nextItem)();

            if(next.Status == TaskStatus.Created)
                next.Start();

            _taskInFlight = next;

            next.ContinueWith(antecedent => OnTaskCompletion(antecedent));
        }

        /// <summary>
        /// 선행 작업이 완료되면 호출되어, 큐에서 새로운 Task을 꺼내 실행시킨다.
        /// </summary>
        /// <param name="antecedent">선행 작업</param>
        private void OnTaskCompletion(Task antecedent) {
            lock(_tasks) {
                _taskInFlight = null;
                if(_tasks.Count > 0)
                    StartTask_CallUnderLock(_tasks.Dequeue());
            }
        }
    }
}