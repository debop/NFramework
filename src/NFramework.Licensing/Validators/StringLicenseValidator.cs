namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스 파일 내용을 검증합니다.
    /// </summary>
    public class StringLicenseValidator : AbstractLicenseValidator {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="publicKey">public key</param>
        /// <param name="license">라이선스 내용</param>
        public StringLicenseValidator(string publicKey, string license) : base(publicKey) {
            License = license;
        }

        /// <summary>
        /// License content
        /// </summary>
        protected override string License { get; set; }
    }
}