using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    [Serializable]
    public class SymmetricEncryptor : ISymmetricEncryptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SymmetricEncryptor() : this(new RC2CryptoServiceProvider(), CryptoTool.Password) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="algorithm"></param>
        public SymmetricEncryptor(SymmetricAlgorithm algorithm) : this(algorithm, CryptoTool.Password) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="key"></param>
        public SymmetricEncryptor(SymmetricAlgorithm algorithm, string key) {
            algorithm.ShouldNotBeNull("algorithm");
            key.ShouldNotBeEmpty("key");

            //if(algorithm.GetType().Equals(typeof(RijndaelManaged)))
            //    throw new NotSupportedException("RijndaelManaged 알고리즘은 지원하지 않습니다. TripleDESCryptoServiceProvider를 사용하시기 바랍니다.");

            Algorithm = algorithm;
            Key = key;

            var newLength = Algorithm.KeySize / 8;
            Algorithm.Key = CryptoTool.DerivePassword(Key, newLength);

            if(algorithm is Rijndael)
                Algorithm.BlockSize = Algorithm.KeySize;


            if(IsDebugEnabled)
                log.Debug("대칭형 암호화를 수행하는 SymmectricEncryptor를 생성했습니다. Algorithm=[{0}], KeySize=[{1}], BlockSize=[{2}]",
                          Algorithm, Algorithm.KeySize, Algorithm.BlockSize);
        }

        /// <summary>
        /// 대칭형 암호화 Algorithm
        /// </summary>
        public SymmetricAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// 암호화시 사용될 비밀번호
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 지정한 정보를 암호화한다.
        /// </summary>
        /// <param name="plainBytes">암호화할 정보</param>
        /// <returns>암호화된 정보</returns>
        public byte[] Encrypt(byte[] plainBytes) {
            plainBytes.ShouldNotBeEmpty("plainBytes");

            if(IsDebugEnabled)
                log.Debug("지정된 바이트 배열에 대해 암호화를 진행합니다... Algorithm=[{0}], keySize=[{1}]", Algorithm, Algorithm.KeySize);

            var ivBytes = CryptoTool.GetInitialVector(Algorithm.KeySize);
            var keyBytes = Algorithm.Key; // CryptoUtils.DerivePassword(Key, Algorithm.KeySize / 8);

            using(var transform = Algorithm.CreateEncryptor(keyBytes, ivBytes)) {
                return transform.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }
        }

        /// <summary>
        /// 지정한 암호화된 정보를 복호화한다.
        /// </summary>
        /// <param name="cipher">암호화된 정보</param>
        /// <returns>복호화된 정보</returns>
        public byte[] Decrypt(byte[] cipher) {
            cipher.ShouldNotBeEmpty("cipher");

            if(IsDebugEnabled)
                log.Debug("지정된 바이트 배열에 대해 복호화를 진행합니다... Algorithm=[{0}], keySize=[{1}]", Algorithm, Algorithm.KeySize);

            var ivBytes = CryptoTool.GetInitialVector(Algorithm.KeySize);
            var keyBytes = Algorithm.Key; // CryptoUtils.DerivePassword(Key, Algorithm.KeySize / 8);

            using(var transform = Algorithm.CreateDecryptor(keyBytes, ivBytes)) {
                return transform.TransformFinalBlock(cipher, 0, cipher.Length);
            }
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// 소멸자
        /// </summary>
        ~SymmetricEncryptor() {
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
                if(Algorithm != null)
                    Algorithm.Clear();
            }
            IsDisposed = true;
        }

        #endregion
    }
}