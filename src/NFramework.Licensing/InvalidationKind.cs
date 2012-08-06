namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 무효 종류
    /// </summary>
    public enum InvalidationKind {
        /// <summary>
        /// 새 라이선스 발급 불가
        /// </summary>
        CannotGetNewLicense,

        /// <summary>
        /// 라이선스 유효기간 초과
        /// </summary>
        TimeExpired
    }
}