using System;
using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.DataStructures {
    public class AsyncCountdownEvent {
        private readonly AsyncManualResetEvent _amre = new AsyncManualResetEvent();
        private int _count;

        public AsyncCountdownEvent() : this(3) {}

        public AsyncCountdownEvent(int initialCount) {
            initialCount.ShouldBePositive("initialCount");
            _count = initialCount;
        }

        public Task WaitAsync() {
            return _amre.WaitAsync();
        }

        public void Signal() {
            if(_count <= 0)
                throw new InvalidOperationException();

            int newCount = Interlocked.Decrement(ref _count);

            if(newCount == 0)
                _amre.Set();
            else if(newCount < 0)
                throw new InvalidOperationException();
        }

        public Task SignalAndWait() {
            Signal();
            return WaitAsync();
        }
    }
}