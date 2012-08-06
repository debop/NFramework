using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 이진 트리 방식으로 데이타를 저장합니다. 저장소는 1차원 배열이지만, 정렬 순서는 이진 트리 방식을 사용하여, 검색 속도를 높힙니다.
    /// </summary>
    /// <typeparam name="T">힙 요수의 수형</typeparam>
    [Serializable]
    public class BinaryHeap<T> : ICollection<T> where T : IComparable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// BinaryHeap{T} 를 Ascending 정렬하여 반환합니다.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IList<T> Sort(IList<T> list, IComparer<T> comparer) {
            var result = new List<T>();

            var heap = new BinaryHeap<T>(list, comparer);

            while(heap.Count > 0) {
                result.Insert(0, heap.PopRoot());
            }
            return result;
        }

        private readonly IList<T> _list;
        private readonly IComparer<T> _comparer;

        public BinaryHeap() {
            _list = new List<T>();
            _comparer = Comparer<T>.Default;
            Count = _list.Count();
        }

        public BinaryHeap(IEnumerable<T> collection) : this(collection, Comparer<T>.Default) {}
        public BinaryHeap(IEnumerable<T> collection, IComparer<T> comparer) : this(collection, comparer, (int?)null) {}

        public BinaryHeap(IEnumerable<T> collection, IComparer<T> comparer, int? count) {
            _list = (collection == null)
                        ? new List<T>()
                        : (collection is IList<T>)
                              ? (IList<T>)collection
                              : new List<T>(collection);

            _comparer = comparer ?? Comparer<T>.Default;
            Count = count ?? _list.Count;

            Heapify();
        }

        public int Count { get; private set; }

        /// <summary>
        /// Root 값을 꺼냅니다.
        /// </summary>
        /// <returns></returns>
        public virtual T PopRoot() {
            Guard.Assert(() => Count > 0, "Heap 이 비었습니다.");

            var root = _list[0];

            SwapCells(0, Count - 1);
            Count--;
            HeapDown(0);

            return root;
        }

        /// <summary>
        /// Root 값을 조사합니다.
        /// </summary>
        /// <returns></returns>
        public virtual T PeekRoot() {
            Guard.Assert(() => Count > 0, "Heap 이 비었습니다.");
            return _list[0];
        }

        /// <summary>
        /// 요소를 추가합니다.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Insert(T item) {
            if(Count >= _list.Count)
                _list.Add(item);
            else
                _list[Count] = item;
            Count++;

            HeapUp(Count - 1);
        }

        private void Heapify() {
            for(var i = Parent(Count - 1); i >= 0; i--)
                HeapDown(i);
        }

        private void HeapUp(int index) {
            var item = _list[index];

            while(true) {
                var parent = Parent(index);

                if(parent < 0 || _comparer.Compare(_list[parent], item) >= 0)
                    break;

                SwapCells(index, parent);
                index = parent;
            }
        }

        private void HeapDown(int index) {
            while(true) {
                var leftChild = LeftChild(index);
                if(leftChild < 0)
                    break;

                var rightChild = RightChild(index);

                var child = (rightChild < 0)
                                ? leftChild
                                : ((_comparer.Compare(_list[leftChild], _list[rightChild]) > 0) ? leftChild : rightChild);

                if(_comparer.Compare(_list[child], _list[index]) < 0)
                    break;

                SwapCells(index, child);
                index = child;
            }
        }

        private int Parent(int index) {
            return SafeIndex((index - 1) / 2);
        }

        private int RightChild(int index) {
            return SafeIndex(2 * index + 2);
        }

        private int LeftChild(int index) {
            return SafeIndex(2 * index + 1);
        }

        private int SafeIndex(int index) {
            return (index < Count) ? index : -1;
        }

        private void SwapCells(int i, int j) {
            var temp = _list[i];
            _list[i] = _list[j];
            _list[j] = temp;
        }

        #region << ICollection<T> >>

        public void Add(T item) {
            Insert(item);
        }

        public void Clear() {
            _list.Clear();
            Count = 0;
        }

        public bool Contains(T item) {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item) {
            return false;
        }

        int ICollection<T>.Count {
            get { return Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        #endregion

        #region << IEnumerable >>

        public IEnumerator<T> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}