using System;
using System.IO;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// Disk에 저장된 라이선스 파일에 대한 라이선스의 유효화에 대한 검증을 수행합니다.
    /// </summary>
    public class LicenseValidator : AbstractLicenseValidator {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly string _licensePath;
        private string _inMemoryLicense;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="publicKey">public key</param>
        /// <param name="licensePath">라이선스 파일 전체 경로</param>
        public LicenseValidator(string publicKey, string licensePath)
            : base(publicKey) {
            _licensePath = licensePath;
        }

        public LicenseValidator(string publicKey, string licensePath, string licenseServerUrl, Guid clientId)
            : base(publicKey, licenseServerUrl, clientId) {
            _licensePath = licensePath;
        }

        /// <summary>
        /// 라이선스 정보
        /// </summary>
        protected override string License {
            get { return _inMemoryLicense ?? File.ReadAllText(_licensePath); }
            set {
                try {
                    File.WriteAllText(_licensePath, value);
                }
                catch(Exception ex) {
                    _inMemoryLicense = value;
                    if(log.IsWarnEnabled)
                        log.WarnException("라이선스 내용을 파일에 쓰는데 실패했습니다. 대신 메모리 라이선스를 사용하도록 합니다.", ex);
                }
            }
        }

        /// <summary>
        /// 로드된 라이선스를 검증합니다.
        /// </summary>
        public override void AssertValidLicense() {
            if(File.Exists(_licensePath) == false) {
                var message = string.Format("라이선스 파일이 존재하지 않습니다. 파일경로=[{0}]", _licensePath);
                log.Warn(message);
                throw new LicenseFileNotFoundException(message);
            }

            base.AssertValidLicense();
        }

        /// <summary>
        /// 기존 라이선스 정보를 제거합니다.
        /// </summary>
        public override void RemoveExistingLicense() {
            if(File.Exists(_licensePath))
                File.Delete(_licensePath);
        }
    }
}