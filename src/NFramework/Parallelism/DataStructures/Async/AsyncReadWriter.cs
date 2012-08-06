using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 비동기 방식으로 쓰기 잠금 및 동시 읽기 실행을 지원하는 ReaderWriter입니다.
    /// </summary>
    [Serializable]
    public sealed class AsyncReaderWriter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public AsyncReaderWriter() : this(Task.Factory) {}

        public AsyncReaderWriter(TaskFactory taskFactory = null) {
            _taskFactory = taskFactory ?? Task.Factory;
        }

        private readonly object _syncLock = new object();

        /// <summary>
        /// concurrent reader 들이 실행하기를 위해 대기하는 큐입니다. (읽기 작업들의 대기 큐)
        /// </summary>
        private readonly Queue<Task> _waitingConcurrent = new Queue<Task>();

        /// <summary>
        /// 배타적 쓰기 작업들이 실행하기를 위해 대기하는 큐입니다. (쓰기 작업들의 대기 큐)
        /// </summary>
        private readonly Queue<Task> _waitingExclusive = new Queue<Task>();

        /// <summary>
        /// 현재 실행되는 읽기 작업의 갯수
        /// </summary>
        private int _currentConcurrent;

        /// <summary>
        /// 현재 배타적 잠금 상태인지를 나타낸다.
        /// </summary>
        private bool _currentlyExclusive;

        /// <summary>
        /// 읽기/쓰기 작업을 생성하기 위한 Task Factory
        /// </summary>
        private readonly TaskFactory _taskFactory;

        /// <summary>
        /// 대기중인 쓰기 작업의 갯수
        /// </summary>
        public int WaitingExclusive {
            get { lock(_syncLock) return _waitingExclusive.Count; }
        }

        /// <summary>
        /// 대기중인 읽기 작업의 갯수
        /// </summary>
        public int WaitingConcurrent {
            get { lock(_syncLock) return _waitingConcurrent.Count; }
        }

        /// <summary>
        /// 현재 실행되는 읽기 작업의 갯수
        /// </summary>
        public int CurrentConcurrent {
            get { lock(_syncLock) return _currentConcurrent; }
        }

        /// <summary>
        ///  현재 배타적 잠금 상태인지를 나타낸다.
        /// </summary>
        public bool CurrentlyExclusive {
            get { lock(_syncLock) return _currentlyExclusive; }
        }

        /// <summary>
        /// 쓰기 잠금을 위한 작업을 큐에 추가합니다.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task QueueExclusiveWriter(Action action) {
            action.ShouldNotBeNull("action");

            if(IsDebugEnabled)
                log.Debug("배타적 쓰기 작업을 큐에 넣습니다...");

            var task = _taskFactory.Create(state => With.TryAction(() => ((Action)state)(), null, () => FinishExclusiveWriter()), action);

            lock(_syncLock) {
                var isTaskRunningOrWaitingTaskExists = (_currentlyExclusive || _currentConcurrent > 0 || _waitingExclusive.Count > 0);

                if(isTaskRunningOrWaitingTaskExists)
                    _waitingExclusive.Enqueue(task);
                else
                    RunExclusive_RequiresLock(task);
            }

            return task;
        }

        /// <summary>
        /// 쓰기 잠금을 위한 작업을 큐에 추가합니다.
        /// </summary>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public Task<TResult> QueueExclusiveWriter<TResult>(Func<TResult> valueFactory) {
            valueFactory.ShouldNotBeNull("valueFactory");

            var task =
                _taskFactory.Create(
                    state => With.TryFunction(() => ((Func<TResult>)state)(), valueFactory, null, () => FinishExclusiveWriter()));

            lock(_syncLock) {
                var isTaskRunningOrWaitingTaskExists = (_currentlyExclusive || _currentConcurrent > 0 || _waitingExclusive.Count > 0);
                if(isTaskRunningOrWaitingTaskExists)
                    _waitingExclusive.Enqueue(task);
                else
                    RunExclusive_RequiresLock(task);
            }
            return task;
        }

        /// <summary>
        /// 읽기 작업을 큐에 대기 시킵니다.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task QueueConcurrentReader(Action action) {
            action.ShouldNotBeNull("action");

            var task = _taskFactory.Create(state => With.TryAction(() => ((Action)state)(), null, () => FinishConcurrentReader()),
                                           action);

            lock(_syncLock) {
                var existsExclusiveTask = (_currentlyExclusive || _waitingExclusive.Count > 0);

                if(existsExclusiveTask)
                    _waitingConcurrent.Enqueue(task);
                else
                    RunConcurrent_RequiresLock(task);
            }
            return task;
        }

        /// <summary>
        /// 읽기 작업을 큐에 대기 시킵니다.
        /// </summary>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public Task<TResult> QueueConcurrentReader<TResult>(Func<TResult> valueFactory) {
            valueFactory.ShouldNotBeNull("valueFactory");

            var task =
                _taskFactory.Create(
                    state => With.TryFunction(() => ((Func<TResult>)state)(), valueFactory, null, () => FinishConcurrentReader()));

            lock(_syncLock) {
                var existsExclusiveTask = (_currentlyExclusive || _waitingExclusive.Count > 0);

                if(existsExclusiveTask)
                    _waitingConcurrent.Enqueue(task);
                else
                    RunConcurrent_RequiresLock(task);
            }
            return task;
        }

        private void RunExclusive_RequiresLock(Task exclusiveTask) {
            _currentlyExclusive = true;
            exclusiveTask.Start(_taskFactory.GetTargetScheduler());
        }

        private void RunConcurrent_RequiresLock(Task concurrentTask) {
            _currentConcurrent++;
            concurrentTask.Start(_taskFactory.GetTargetScheduler());
        }

        /// <summary>
        /// 읽기 작업 큐에 있는 모든 작업을 수행합니다.
        /// </summary>
        private void RunConcurrent_RequiresLock() {
            while(_waitingConcurrent.Count > 0)
                RunConcurrent_RequiresLock(_waitingConcurrent.Dequeue());
        }

        /// <summary>
        /// Concurrent Reader 작업을 완료합니다
        /// </summary>
        private void FinishConcurrentReader() {
            lock(_syncLock) {
                _currentConcurrent--;

                // 읽기 작업이 없고, 쓰기 작업이 대기하고 있다면, 쓰기 작업을 수행합니다.
                //
                if(_currentConcurrent == 0 && _waitingExclusive.Count > 0)
                    RunExclusive_RequiresLock(_waitingExclusive.Dequeue());

                    // 반대로 쓰기 작업 대기가 없고, 읽기 작업이 대기하고 있다면, 읽기 작업을 수행합니다.
                else if(_waitingExclusive.Count == 0 && _waitingConcurrent.Count > 0)
                    RunConcurrent_RequiresLock();
            }
        }

        /// <summary>
        /// 배타적 쓰기 작업을 완료합니다.
        /// </summary>
        private void FinishExclusiveWriter() {
            if(IsDebugEnabled)
                log.Debug("배타적 쓰기 작업을 종료합니다...");

            lock(_syncLock) {
                _currentlyExclusive = false;

                // 쓰기 작업이 대기하고 있다면, 쓰기 작업을 수행합니다.
                if(_waitingExclusive.Count > 0)
                    RunExclusive_RequiresLock(_waitingExclusive.Dequeue());

                    // 반대로 읽기 작업만 대기하고 있다면, 읽기 작업을 수행합니다.
                else if(_waitingConcurrent.Count > 0)
                    RunConcurrent_RequiresLock();
            }

            if(IsDebugEnabled)
                log.Debug("배타적 쓰기 작업을 종료했습니다!!!");
        }
    }
}