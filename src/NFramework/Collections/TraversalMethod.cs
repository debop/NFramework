namespace NSoft.NFramework.Collections {
    /// <summary>
    /// Binary Tree를 탐색할 때의 방법을 말합니다.
    /// </summary>
    /// <remarks>
    /// Binary Tree의 요소들을 탐색할 때 순서에 따라 전혀 다른 결과를 나타낸다.
    /// </remarks>
    public enum TraversalMethod {
        /// <summary>
        /// 자신->Left->Right 순으로 
        /// </summary>
        PreOrder,

        /// <summary>
        /// Left->자신->Right 순으로 (오름차순)
        /// </summary>
        InOrder,

        /// <summary>
        /// Left->Right->자신 순으로
        /// </summary>
        PostOrder,

        /// <summary>
        /// Right->자신->Left 순으로 (내림차순)
        /// </summary>
        InRevOrder
    }
}