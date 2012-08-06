namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 구간 경계값 포함 여부
    /// </summary>
    public enum IntervalKind {
        /// <summary>
        /// 양쪽 경계 포함 안됨	: (a,b)
        /// </summary>
        Open,

        /// <summary>
        /// 양쪽 경계 포함 : [a, b]
        /// </summary>
        Closed,

        /// <summary>
        /// Min 경계값 포함 안됨, Max 경계값은 포함 : (a, b]
        /// </summary>
        OpenClosed,

        /// <summary>
        /// Min 경계값은 포함, Max 경계값은 포함안됨 : [a, b)
        /// </summary>
        ClosedOpen
    }
}