namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 큐형태의 저장소를 위한 기능을 제공하는 인터페이스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueue<T> {
        /// <summary>
        /// 요소를 큐에 넣는다.
        /// </summary>
        /// <param name="item">큐에 추가할 요소</param>
        void Enqueue(T item);

        /// <summary>
        /// 큐에서 요소를 꺼낸다.
        /// </summary>
        /// <returns></returns>
        T Dequeue();
    }
}