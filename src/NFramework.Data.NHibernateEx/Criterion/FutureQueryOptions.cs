namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// Future Query를 수행시에 부가적인 작업에 대한 옵션
    /// </summary>
    public enum FutureQueryOptions {
        /// <summary>
        /// 부가적인 수행을 하지 않는다.
        /// </summary>
        None,

        /// <summary>
        /// 결과 값에 Total Count 값도 계산한다.
        /// </summary>
        WithTotalCount
    }
}