using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 비동기 <see cref="ManualResetEvent"/>
    /// </summary>
    public class AsyncManualResetEvent {
        private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        public Task WaitAsync() {
            return _tcs.Task;
        }

        public void Set() {
            var tcs = _tcs;
            Task.Factory.StartNew(() => tcs.TrySetResult(true),
                                  CancellationToken.None,
                                  TaskCreationOptions.PreferFairness,
                                  TaskScheduler.Default);
            tcs.Task.Wait();
        }

        public void Reset() {
            while(true) {
                var tcs = _tcs;
                if(tcs.Task.IsCompleted == false ||
                   Interlocked.CompareExchange(ref _tcs, new TaskCompletionSource<bool>(), tcs) == tcs)
                    return;
            }
        }
    }
}