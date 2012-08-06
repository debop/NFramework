using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 비동기 생산자-소비자 패턴의 컬렉션입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Count={CurrentCount}")]
    [Serializable]
    public sealed class AsyncProducerConsumerCollection<T> : IProducerConsumerCollection<T>, IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private AsyncSemaphore _semaphore = new AsyncSemaphore();
        private readonly IProducerConsumerCollection<T> _collection;

        /// <summary>
        /// 생성자
        /// </summary>
        public AsyncProducerConsumerCollection() : this(new ConcurrentQueue<T>()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection"></param>
        public AsyncProducerConsumerCollection(IProducerConsumerCollection<T> collection) {
            collection.ShouldNotBeNull("collection");
            _collection = collection;
        }

        /// <summary>
        /// 컬렉션에 요소를 추가합니다.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) {
            if(IsDebugEnabled)
                log.Debug("내부 버퍼에 요소를 추가합니다. item=[{0}]", item);

            if(_collection.TryAdd(item))
                _semaphore.Release();
            else
                throw new InvalidOperationException("요소를 추가하지 못했습니다!!! item=" + item);
        }

        /// <summary>
        /// 컬렉션으로부터 비동기적으로 요소를 취합니다.
        /// </summary>
        /// <returns>A Task that represents the element removed from the collection.</returns>
        public Task<T> Take() {
            if(IsDebugEnabled)
                log.Debug("내부 버퍼에 보관된 요소를 가져갑니다...");

            return
                _semaphore
                    .WaitAsync()
                    .ContinueWith(_ => {
                                      T result;

                                      if(_collection.TryTake(out result) == false)
                                          throw new InvalidOperationException("컬렉션에서 요소를 가져오지 못했습니다.");

                                      if(IsDebugEnabled)
                                          log.Debug("내부 버퍼에 보관된 요소를 꺼냈습니다... result=[{0}]", result);

                                      return result;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        public void CopyTo(T[] array, int index) {
            _collection.CopyTo(array, index);
        }

        public bool TryAdd(T item) {
            try {
                Add(item);
                return true;
            }
            catch(Exception) {
                return false;
            }
        }

        public bool TryTake(out T item) {
            try {
                item = Take().Result;
                return true;
            }
            catch(AggregateException age) {
                age.Handle(ex => true);

                item = default(T);
                return false;
            }
        }

        public T[] ToArray() {
            return _collection.ToArray();
        }

        void ICollection.CopyTo(Array array, int index) {
            if(array is T[]) {
                CopyTo((T[])array, index);
            }
        }

        /// <summary>
        /// 컬렉션 요소의 수
        /// </summary>
        public int Count {
            get { return _collection.Count; }
        }

        public object SyncRoot {
            get { return this; }
        }

        public bool IsSynchronized {
            get { return false; }
        }

        /// <summary>
        /// 컬렉션의 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; private set; }

        public void Dispose() {
            if(IsDisposed)
                return;

            if(IsDebugEnabled)
                log.Debug("AsyncProducerConsumerCollection 인스턴스를 Dispose 합니다...");

            if(_semaphore != null) {
                _semaphore.Dispose();
                _semaphore = null;
            }

            IsDisposed = true;
        }

        public IEnumerator<T> GetEnumerator() {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}