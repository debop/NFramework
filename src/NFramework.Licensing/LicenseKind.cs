namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스 종류
    /// </summary>
    public enum LicenseKind {
        None,

        /// <summary>
        /// Trial use
        /// </summary>
        Trial,

        /// <summary>
        /// Standard license
        /// </summary>
        Standard,

        /// <summary>
        /// Personal use
        /// </summary>
        Personal,

        /// <summary>
        /// Floating license ( 서버 접속자에게 임시로 주어지는 라이선스, 로그아웃하면 라이선스를 회수)
        /// http://en.wikipedia.org/wiki/Floating_licensing
        /// </summary>
        Floating,

        /// <summary>
        /// Subscription 기반의 라이선스
        /// </summary>
        Subscription
    }
}