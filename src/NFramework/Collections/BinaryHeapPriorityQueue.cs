using System;
using System.Collections;
using System.Collections.Generic;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// <see cref="BinaryHeap{T}"/> 을 이용하는 우선순위에 의해 내부 정렬을 수행하는 큐입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BinaryHeapPriorityQueue<T> : IQueue<T>, IEnumerable<T> where T : IComparable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static BinaryHeap<T> _heap;

        public BinaryHeapPriorityQueue() : this(Comparer<T>.Default) {}
        public BinaryHeapPriorityQueue(IComparer<T> comparer) : this(new List<T>(), comparer) {}

        public BinaryHeapPriorityQueue(IEnumerable<T> collection, IComparer<T> comparer) {
            _heap = new BinaryHeap<T>(collection, comparer);
        }

        public int Size {
            get { return _heap.Count; }
        }

        /// <summary>
        /// 최상위 값을 Peek 합니다.
        /// </summary>
        /// <returns></returns>
        public T Peek() {
            return _heap.PeekRoot();
        }

        #region Implementation of IQueue<T>

        public void Enqueue(T item) {
            _heap.Insert(item);
        }

        public T Dequeue() {
            return _heap.PopRoot();
        }

        #endregion

        public bool Contains(T item) {
            return _heap.Contains(item);
        }

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator() {
            return _heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}