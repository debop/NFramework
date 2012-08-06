using System;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 비동기 Lock 객체
    /// </summary>
    /// <example>
    /// <code>
    /// private static readonly AsyncLock _lock = new AsyncLock();
    /// 
    /// using(var releaser = _lock.LockAsync())
    /// {
    ///     ....
    /// }
    /// </code>
    /// </example>
    public class AsyncLock {
        private readonly AsyncSemaphore _semaphore;
        private readonly Task<Releaser> _releaser;

        public AsyncLock() {
            _semaphore = new AsyncSemaphore(1);
            _releaser = Task.Factory.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync() {
            var wait = _semaphore.WaitAsync();

            return wait.IsCompleted
                       ? _releaser
                       : wait.ContinueWith(delegate { return new Releaser((AsyncLock)this); },
                                           CancellationToken.None,
                                           TaskContinuationOptions.ExecuteSynchronously,
                                           TaskScheduler.Default);
        }

        public struct Releaser : IDisposable {
            private readonly AsyncLock _toReleaseLock;

            internal Releaser(AsyncLock toReleaseLock) {
                _toReleaseLock = toReleaseLock;
            }

            public void Dispose() {
                if(_toReleaseLock != null)
                    _toReleaseLock._semaphore.Release();
            }
        }
    }
}