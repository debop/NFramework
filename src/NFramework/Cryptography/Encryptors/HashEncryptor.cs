using System;
using System.Security.Cryptography;
using System.Text;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// 해시 암호화를 수행합니다.
    /// </summary>
    [Serializable]
    public class HashEncryptor : IHashEncryptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// SHA256 알고리즘을 이용한 Hash 암호화 클래스입니다.
        /// </summary>
        public HashEncryptor() : this(new SHA256Managed()) {}

        /// <summary>
        /// 지정된 HashAlgorithm을 이용하는 HashEncryptor를 생성합니다.
        /// </summary>
        /// <param name="algorithm"></param>
        public HashEncryptor(HashAlgorithm algorithm) {
            algorithm.ShouldNotBeNull("algorithm");
            Algorithm = algorithm;

            if(IsDebugEnabled)
                log.Debug("Hash 계산을 위해 HashEncryptor를 생성합니다... Algorithm=[{0}]", algorithm.GetType().FullName);
        }

        /// <summary>
        /// Hash Algorithm
        /// </summary>
        public virtual HashAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// 지정된 문자열을 암호화를 수행한다.
        /// </summary>
        /// <param name="plainText">암호화할 문자열</param>
        /// <returns>Hashing한 정보</returns>
        /// <exception cref="ArgumentNullException">암호화할 문자열이 빈 문자열일 때</exception>
        public virtual byte[] ComputeHash(string plainText) {
            plainText.ShouldNotBeEmpty("plainText");

            if(IsDebugEnabled)
                log.Debug("문자열을 [{0}] 방식으로 Hash값을 계산합니다. PlainText=[{1}]",
                          Algorithm, plainText.EllipsisChar(80));

            return Algorithm.ComputeHash(Encoding.Unicode.GetBytes(plainText));
        }

        /// <summary>
        /// 지정된 문자열을 암호화를 수행하고, 값을 일반적인 값으로 반환한다.
        /// </summary>
        /// <param name="plainText">암호화할 문자열</param>
        /// <param name="format">암호화한 정보를 문자열로 표현시 사용할 포맷 (Base64/Hex)</param>
        /// <returns>Hashing한 정보</returns>
        /// <exception cref="ArgumentNullException">암호화할 문자열이 빈 문자열일 때</exception>
        public virtual string ComputeHash(string plainText, EncryptionStringFormat format) {
            return ComputeHash(plainText).BytesToString(format);
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// 소멸자
        /// </summary>
        ~HashEncryptor() {
            Dispose(false);
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                if(Algorithm != null) {
                    Algorithm.Clear();
                    Algorithm = null;
                }
            }
            IsDisposed = true;
        }

        #endregion
    }
}