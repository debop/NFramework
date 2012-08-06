using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 생산자-소비자 컬렉션을 래핑하여 부가적인 기능을 제공하도록 하는 생산자-소비자 컬렉션 래퍼입니다. (일종의 Decorator의 기본 클래스입니다.)
    /// </summary>
    /// <typeparam name="T">항목의 수형</typeparam>
    [Serializable]
    public abstract class ProducerConsumerCollectionBase<T> : IProducerConsumerCollection<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="innerCollection">내부 버퍼</param>
        protected ProducerConsumerCollectionBase(IProducerConsumerCollection<T> innerCollection) {
            innerCollection.ShouldNotBeNull("innerCollection");
            InnerCollection = innerCollection;
        }

        /// <summary>
        /// 내부 컬렉션
        /// </summary>
        protected IProducerConsumerCollection<T> InnerCollection { get; private set; }

        /// <summary>
        /// 열거자 얻기
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() {
            return InnerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index) {
            InnerCollection.CopyTo(array, index);
        }

        /// <summary>
        /// 요수 수
        /// </summary>
        public int Count {
            get { return InnerCollection.Count; }
        }

        object ICollection.SyncRoot {
            get { return InnerCollection.SyncRoot; }
        }

        bool ICollection.IsSynchronized {
            get { return InnerCollection.IsSynchronized; }
        }

        /// <summary>
        /// 요소를 지정한 Array에 복사한다.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(T[] array, int index) {
            InnerCollection.CopyTo(array, index);
        }

        protected virtual bool TryAdd(T item) {
            if(IsDebugEnabled)
                log.Debug("요소를 추가합니다... item=[{0}]", item);

            return InnerCollection.TryAdd(item);
        }

        /// <summary>
        /// 지정한 요소를 내부 버퍼에 추가하기를 시도합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IProducerConsumerCollection<T>.TryAdd(T item) {
            return TryAdd(item);
        }

        /// <summary>
        /// 요소를 내부 버퍼에서 꺼내기를 시도합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool TryTake(out T item) {
            if(IsDebugEnabled)
                log.Debug("요소를 꺼내려고 합니다...");

            return InnerCollection.TryTake(out item);
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item) {
            return TryTake(out item);
        }

        /// <summary>
        /// 내부 버퍼의 모든 요소를 배열로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() {
            return InnerCollection.ToArray();
        }
    }
}