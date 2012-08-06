using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    // HINT: 세마포어 클래스(http://msdn.microsoft.com/ko-kr/library/kt3k0k2h%28v=VS.85%29.aspx)

    /// <summary>
    /// 리소스에 접근할 수 있는 스레드 수를 비동기 방식으로 제한합니다.
    /// </summary>
    /// <seealso cref="Semaphore"/>
    [Serializable]
    public sealed class AsyncSemaphore : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private int _currentCount;
        private int _maxCount;
        private readonly Queue<TaskCompletionSource<bool>> _waitingTasks;
        private static readonly Task completedTask = Task.Factory.FromResult(true);

        public AsyncSemaphore() : this(0, Int32.MaxValue) {}

        public AsyncSemaphore(int initialCount = 0, int maxCount = Int32.MaxValue) {
            maxCount.ShouldBePositive("maxCount");
            initialCount.ShouldBeGreaterOrEqual(0, "initialCount");
            maxCount.ShouldBeGreaterOrEqual(initialCount, "maxCount");

            if(IsDebugEnabled)
                log.Debug("AsyncSemaphore 인스턴스가 생성되었습니다. initialCount=[{0}], maxCount=[{1}]", initialCount, maxCount);

            _currentCount = initialCount;
            _maxCount = maxCount;
            _waitingTasks = new Queue<TaskCompletionSource<bool>>();
        }

        /// <summary>
        /// 현재 세마포어 수
        /// </summary>
        public int CurrentCount {
            get { return _currentCount; }
        }

        /// <summary>
        /// 최대 세마포어 수
        /// </summary>
        public int MaxCount {
            get { return _maxCount; }
        }

        /// <summary>
        /// 현재 세마포어에 들어가기 위해 기다리는 작업 수
        /// </summary>
        public int WaitingCount {
            get {
                lock(_waitingTasks)
                    return _waitingTasks.Count;
            }
        }

        /// <summary>
        /// 세마포어에 빈자리가 날때까지 기다린다.
        /// </summary>
        /// <returns>완료된 Task</returns>
        public Task WaitAsync() {
            ThrowIfDisposed();

            if(IsDebugEnabled)
                log.Debug("세마포어에 빈자리가 생길 때까지 기다립니다...");

            lock(_waitingTasks) {
                if(_currentCount > 0) {
                    --_currentCount;
                    return completedTask;
                }

                var waiter = new TaskCompletionSource<bool>();
                _waitingTasks.Enqueue(waiter);
                return waiter.Task;
            }
        }

        /// <summary>
        /// 실행할 Action을 큐에 보관합니다. 세마포어에 빈 자리가 생기면 실행합니다.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task Queue(Action action) {
            action.ShouldNotBeNull("action");

            return WaitAsync().ContinueWith(_ => With.TryAction(() => action(), null, () => Release()));
        }

        /// <summary>
        /// 실행할 함수를 큐에 보관합니다. 세마포어에 빈 자리가 생기면 실행합니다.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public Task<TResult> Queue<TResult>(Func<TResult> function) {
            function.ShouldNotBeNull("function");

            return WaitAsync().ContinueWith(_ => With.TryFunction(() => function(), finallyAction: () => Release()));
        }

        /// <summary>
        /// Releases a unit of work to the semaphore.
        /// </summary>
        public void Release() {
            TaskCompletionSource<bool> toRelease = null;

            lock(_waitingTasks) {
                if(_waitingTasks.Count > 0) {
                    toRelease = _waitingTasks.Dequeue();
                }
                else {
                    _currentCount++;
                }
            }
            if(toRelease != null)
                toRelease.SetResult(true);
        }

        private void ThrowIfDisposed() {
            Guard.Assert<ObjectDisposedException>(_maxCount > 0, @"AsyncSemaphore already disposed...");
        }

        public void Dispose() {
            if(_maxCount <= 0)
                return;

            _maxCount = 0;

            lock(_waitingTasks) {
                while(_waitingTasks.Count > 0)
                    _waitingTasks.Dequeue().SetCanceled();
            }

            if(IsDebugEnabled)
                log.Debug("AsyncSemaphore 인스턴스를 Dispose 했습니다!!!");
        }
    }
}