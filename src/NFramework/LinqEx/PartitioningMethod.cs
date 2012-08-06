namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// 특정 값을 배분하는 방식
    /// </summary>
    public enum PartitioningMethod {
        /// <summary>
        /// 수평균등
        /// </summary>
        HorizontaolUniform,

        /// <summary>
        /// 수직 전반 부하
        /// </summary>
        VerticalBegin,

        /// <summary>
        /// 수직 후반 부하
        /// </summary>
        VerticalEnd,

        /// <summary>
        /// 삼각 전반 부하
        /// </summary>
        TriangleBegin,

        /// <summary>
        /// 삼각 중반 부하
        /// </summary>
        TriangleMiddle,

        /// <summary>
        /// 삼각 후반 부하
        /// </summary>
        TriangleEnd,

        /// <summary>
        /// 2단계 전반 부하
        /// </summary>
        TwoStepBegin,

        /// <summary>
        /// 2단계 후반부하
        /// </summary>
        TwoStepEnd,

        /// <summary>
        /// 사다리 중반 부하
        /// </summary>
        TrapezoidMiddle,

        /// <summary>
        /// 종형 중반 부하 (정규분포 - NormalDistribution)
        /// </summary>
        Normal,

        /// <summary>
        /// 3단 중반 부하
        /// </summary>
        ThreeMiddle,
    }
}