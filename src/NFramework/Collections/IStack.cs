namespace NSoft.NFramework.Collections {
    /// <summary>
    /// Stack을 표현하는 인터페이스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStack<T> {
        /// <summary>
        /// Stack에 요소 추가
        /// </summary>
        /// <param name="item">추가할 요소</param>
        void Push(T item);

        /// <summary>
        /// Stack에서 요소 제거
        /// </summary>
        /// <returns>제거한 요소</returns>
        T Pop();

        /// <summary>
        /// Stack의 가장 위에 있는 요소를 조회
        /// </summary>
        /// <returns></returns>
        T Peek();
    }
}