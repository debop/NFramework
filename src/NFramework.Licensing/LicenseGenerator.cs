using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스 생성자
    /// </summary>
    public class LicenseGenerator {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly string _privateKey;
        protected static readonly IDictionary<string, string> EmptyAttributes = new Dictionary<string, string>();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="privateKey">제품 private key</param>
        public LicenseGenerator(string privateKey) {
            _privateKey = privateKey;
        }

        /// <summary>
        /// 새로운 Floating 라이선스를 생성합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public string GenerateFloatingLicense(string name, string publicKey) {
            return LicenseTool.GenerateFloatingLicense(_privateKey, name, publicKey);
        }

        /// <summary>
        /// 새로운 라이선스를 생성합니다.
        /// </summary>
        /// <param name="name">라이선스 소유자 명</param>
        /// <param name="id">라이선스 소유자 Id</param>
        /// <param name="expirationDate">라이선스 유효기간</param>
        /// <param name="licenseKind">라이선스 종류</param>
        /// <returns>라이선스 내용</returns>
        public string Generate(string name, Guid id, DateTime expirationDate, LicenseKind licenseKind) {
            return Generate(name, id, expirationDate, EmptyAttributes, licenseKind);
        }

        /// <summary>
        /// 새로운 라이선스를 생성합니다.
        /// </summary>
        /// <param name="name">라이선스 소유자 명</param>
        /// <param name="id">라이선스 소유자 Id</param>
        /// <param name="expirationDate">라이선스 유효기간</param>
        /// <param name="attributes">라이선스 속성</param>
        /// <param name="licenseKind">라이선스 종류</param>
        /// <returns>라이선스 내용</returns>
        public string Generate(string name, Guid id, DateTime expirationDate, IDictionary<string, string> attributes,
                               LicenseKind licenseKind) {
            return LicenseTool.GenerateLicense(_privateKey, name, id, expirationDate, attributes, licenseKind);
        }
    }
}