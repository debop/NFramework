using System;
using System.Collections;
using System.Collections.Generic;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 양방향으로 넣고 빼기가 가능한 큐이다.
    /// </summary>
    /// <remarks>
    /// DoubleQueue 인스턴스는 멀티 쓰레드에 안전하지 않습니다.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DoubleQueue<T> : IEnumerable<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly List<T> _internalList;

        /// <summary>
        /// 생성자
        /// </summary>
        public DoubleQueue() {
            _internalList = new List<T>();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="capacity">큐의 크기</param>
        public DoubleQueue(int capacity) {
            _internalList = new List<T>(capacity);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection">큐에 추가할 요소</param>
        public DoubleQueue(IEnumerable<T> collection) {
            _internalList = new List<T>(collection);
        }

        /// <summary>
        /// 현재 큐에 저장된 요소의 수
        /// </summary>
        /// <value></value>
        public int Count {
            get { return _internalList.Count; }
        }

        /// <summary>
        /// 현재 큐의 크기
        /// </summary>
        public int Capacity {
            get { return _internalList.Capacity; }
            set { _internalList.Capacity = value; }
        }

        /// <summary>
        /// 큐의 요소를 모두 제거한다.
        /// </summary>
        public void Clear() {
            _internalList.Clear();
        }

        /// <summary>
        /// 큐에 지정된 item이 존재하는지 검사한다.
        /// </summary>
        /// <param name="item">검사할 요소</param>
        /// <returns>존재여부</returns>
        public bool Contains(T item) {
            return _internalList.Contains(item);
        }

        /// <summary>
        /// 큐의 모든 요소를 지정된 배열에 복사한다.
        /// </summary>
        /// <param name="array">대상 배열</param>
        public void CopyTo(T[] array) {
            _internalList.CopyTo(array);
        }

        /// <summary>
        /// 큐의 요소를 지정된 배열에 복사한다.
        /// </summary>
        /// <param name="array">대상 배열</param>
        /// <param name="arrayIndex">복사 위치</param>
        public void CopyTo(T[] array, int arrayIndex) {
            _internalList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 큐의 첫번째 요소를 큐에서 꺼낸다.
        /// </summary>
        /// <returns>꺼낸 요소</returns>
        /// <exception cref="InvalidOperationException">큐에 더이상의 요소가 없을 때</exception>
        public T DequeueHead() {
            _internalList.ShouldNotBeEmpty("queue");

            var result = _internalList[0];
            _internalList.RemoveAt(0);
            return result;
        }

        /// <summary>
        /// 큐의 첫번째 요소를 조회한다.
        /// </summary>
        /// <returns>첫번째 요소, 빈 큐인 경우에는 null을 반환합니다.</returns>
        public T PeekHead() {
            if(_internalList.Count > 0)
                return _internalList[0];

            const object item = null;
            return (T)item;
        }

        /// <summary>
        /// 큐의 끝의 요소를 꺼냅니다.
        /// </summary>
        /// <returns>요소</returns>
        /// <exception cref="InvalidOperationException">큐에 더이상의 요소가 없을 때</exception>
        public T DequeueTail() {
            _internalList.ShouldNotBeEmpty("queue");

            var result = _internalList[_internalList.Count - 1];
            _internalList.RemoveAt(_internalList.Count - 1);
            return result;
        }

        /// <summary>
        /// 큐의 끝의 요소를 반환한다.
        /// </summary>
        /// <returns>마지막 요소, 빈 큐인 경우에는 null을 반환합니다.</returns>
        public T PeekTail() {
            if(_internalList.Count > 0)
                return _internalList[_internalList.Count - 1];

            const object item = null;
            return (T)item;
        }

        /// <summary>
        /// 큐의 처음부분에 요소를 추가한다.
        /// </summary>
        /// <param name="item">요소</param>
        public void EnqueueHead(T item) {
            _internalList.Insert(0, item);
        }

        /// <summary>
        /// 큐의 끝에 요소를 추가한다.
        /// </summary>
        /// <param name="item">요소</param>
        public void EnqueueTail(T item) {
            _internalList.Add(item);
        }

        /// <summary>
        /// 요소의 순서를 반대로 정렬한다.
        /// </summary>
        public void Reverse() {
            _internalList.Reverse();
        }

        /// <summary>
        /// 큐의 요소를 배열로 반환한다.
        /// </summary>
        public T[] ToArray() {
            return _internalList.ToArray();
        }

        /// <summary>
        /// 큐의 내부 버퍼를 실제 크기와 같게 한다.
        /// </summary>
        public void TrimExcess() {
            _internalList.TrimExcess();
        }

        /// <summary>
        /// 인스턴스의 내용을 표현
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.CollectionToString();
        }

        /// <summary>
        /// Return enumerator by In Order traversal method.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() {
            return _internalList.GetEnumerator();
        }

        /// <summary>
        /// 현재 RwDoubleQueue 인스턴스 개체의 복사본을 만든다. Shallow copy를 수행한다.
        /// </summary>
        public object Clone() {
            return new DoubleQueue<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}