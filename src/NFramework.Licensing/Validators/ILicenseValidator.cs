using System;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스 유효 검증자
    /// </summary>
    public interface ILicenseValidator {
        /// <summary>
        /// 라이선스 갱신일자
        /// </summary>
        DateTime ExpirationDate { get; }

        /// <summary>
        /// 등록 서비스의 Endpoint 정보
        /// </summary>
        string SubscriptionEndpoint { get; set; }

        /// <summary>
        /// 라이선스 종류
        /// </summary>
        LicenseKind LicenseKind { get; }

        /// <summary>
        /// Floating License 불가 여부
        /// </summary>
        bool DisableFloatingLicenses { get; set; }
    }
}