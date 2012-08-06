using System;

namespace NSoft.NFramework {
    /// <summary>
    /// Event argument has item which type is T
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    [Serializable]
    public class EventArgs<T> : EventArgs {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="item">Event 정보를 가진 객체</param>
        public EventArgs(T item) {
            Item = item;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="item">Event 정보를 가진 객체</param>
        /// <param name="data">부가 데이터</param>
        public EventArgs(T item, object data) {
            Item = item;
            Data = data;
        }

        /// <summary>
        /// represent data in event argument
        /// </summary>
        public T Item { get; private set; }

        /// <summary>
        /// 부가 데이터
        /// </summary>
        public object Data { get; private set; }
    }
}