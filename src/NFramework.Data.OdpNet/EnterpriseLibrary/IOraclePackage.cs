namespace NSoft.NFramework.Data.OdpNet {
    /// <summary>
    /// 오라클 패키지 매핑을 표현합니다.
    /// </summary>
    /// <remarks>
    /// IOraclePackage는 Oracle stored procedure 명을 package를 포함한 전체 명칭으로 변환하는데 사용됩니다.
    /// </remarks>
    /// <seealso cref="OracleDatabase"/>
    public interface IOraclePackage {
        /// <summary>
        /// 패키지 명
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 패키지 접두사
        /// </summary>
        string Prefix { get; }
    }
}