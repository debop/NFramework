using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 
    /// </summary>
    public class AsyncReaderWriterLock {
        private readonly Task<Releaser> _readerReleaser;
        private readonly Task<Releaser> _writerReleaser;

        private readonly Queue<TaskCompletionSource<Releaser>> _waitingWriters =
            new Queue<TaskCompletionSource<Releaser>>();

        private TaskCompletionSource<Releaser> _waitingReader = new TaskCompletionSource<Releaser>();
        private int _readersWaiting;
        private int _status;

        public AsyncReaderWriterLock() {
            _readerReleaser = Task.Factory.FromResult(new Releaser(this, false));
            _writerReleaser = Task.Factory.FromResult(new Releaser(this, true));
        }

        public Task<Releaser> ReaderLockAsync() {
            lock(_waitingWriters) {
                if(_status >= 0 && _waitingWriters.Count == 0) {
                    ++_status;
                    return _readerReleaser;
                }

                ++_readersWaiting;
                return _waitingReader.Task;
            }
        }

        private void ReaderRelease() {
            TaskCompletionSource<Releaser> toWake = null;

            lock(_waitingWriters) {
                --_status;

                if(_status == 0 && _waitingWriters.Count > 0) {
                    _status = -1;
                    toWake = _waitingWriters.Dequeue();
                }
            }

            if(toWake != null)
                toWake.SetResult(new Releaser(this, true));
        }

        public Task<Releaser> WriterLockAsync() {
            lock(_waitingWriters) {
                if(_status == 0) {
                    _status = -1;
                    return _writerReleaser;
                }

                var waiter = new TaskCompletionSource<Releaser>();
                _waitingWriters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        private void WriterRelease() {
            TaskCompletionSource<Releaser> toWake = null;
            bool toWakeIsWriter = false;

            lock(_waitingWriters) {
                if(_waitingWriters.Count > 0) {
                    toWake = _waitingWriters.Dequeue();
                    toWakeIsWriter = true;
                }
                else if(_readersWaiting > 0) {
                    toWake = _waitingReader;
                    _status = _readersWaiting;
                    _readersWaiting = 0;
                    _waitingReader = new TaskCompletionSource<Releaser>();
                }
                else
                    _status = 0;
            }

            if(toWake != null) {
                toWake.SetResult(new Releaser(this, toWakeIsWriter));
            }
        }

        public struct Releaser : IDisposable {
            private readonly AsyncReaderWriterLock _toRelease;
            private readonly bool _isWriter;

            public Releaser(AsyncReaderWriterLock toRelease, bool isWriter) {
                toRelease.ShouldNotBeNull("toRelease");
                _toRelease = toRelease;
                _isWriter = isWriter;
            }

            public void Dispose() {
                if(_toRelease != null) {
                    if(_isWriter)
                        _toRelease.WriterRelease();
                    else
                        _toRelease.ReaderRelease();
                }
            }
        }
    }
}