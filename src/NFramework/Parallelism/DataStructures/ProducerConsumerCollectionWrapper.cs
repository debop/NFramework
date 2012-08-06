using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// <see cref="BlockingCollection{T}"/>을 저장소로 하는 생산자-소비자 패턴의 컬렉션의 래핑 클래스입니다.
    /// </summary>
    /// <typeparam name="T">컬렉션 요소의 수형</typeparam>
    [Serializable]
    internal sealed class ProducerConsumerCollectionWrapper<T> : IProducerConsumerCollection<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly BlockingCollection<T> _collection;
        private readonly int _msecTimeout;
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">inner buffer</param>
        /// <param name="cancellationToken">작업 취소용 Token</param>
        public ProducerConsumerCollectionWrapper(BlockingCollection<T> collection, CancellationToken cancellationToken)
            : this(collection, Timeout.Infinite, cancellationToken) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">inner buffer</param>
        /// <param name="msecTimeout">add, take 시의 제한시간</param>
        /// <param name="cancellationToken">작업 취소용 Token</param>
        public ProducerConsumerCollectionWrapper(BlockingCollection<T> collection, int msecTimeout, CancellationToken cancellationToken) {
            collection.ShouldNotBeNull("collection");
            Guard.Assert(msecTimeout >= Timeout.Infinite, @"msecTimeout 값 설정이 잘못되었습니다. -1 이상이어야 합니다. mescTimeout=[{0}]", msecTimeout);

            _collection = collection;
            _msecTimeout = msecTimeout;
            _cancellationToken = cancellationToken;
        }

        public void CopyTo(T[] array, int index) {
            _collection.CopyTo(array, index);
        }

        public bool TryAdd(T item) {
            return _collection.TryAdd(item, _msecTimeout, _cancellationToken);
        }

        public bool TryTake(out T item) {
            return _collection.TryTake(out item, _msecTimeout, _cancellationToken);
        }

        public T[] ToArray() {
            return _collection.ToArray();
        }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)_collection).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index) {
            ((ICollection)_collection).CopyTo(array, index);
        }

        public int Count {
            get { return _collection.Count; }
        }

        public object SyncRoot {
            get { return ((ICollection)_collection).SyncRoot; }
        }

        public bool IsSynchronized {
            get { return ((ICollection)_collection).IsSynchronized; }
        }
    }
}