using System;
using NSoft.NFramework.Cryptography.Aria;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// ARIA 알고리즘은 대칭형 암호화 알고리즘으로서, 암호화/복호화가 가능합니다.
    /// </summary>
    [Serializable]
    public sealed class AriaSymmetricEncryptor : ISymmetricEncryptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// ARIA 알고리즘에서 사용하는 BlockSize 크기 (16) 다른 건 128인데...
        /// </summary>
        private const int ARIA_BLOCK_SIZE = 16;

        private AriaEngine _engine;

        private string _password = CryptoTool.Password;

        /// <summary>
        /// 비밀번호
        /// </summary>
        public string Password {
            get { return _password ?? (_password = CryptoTool.Password); }
            set {
                value.ShouldNotBeWhiteSpace("Password");
                _password = value;
            }
        }

        /// <summary>
        /// 비밀 번호에 해당하는 문자열
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// 마스터 키의 크기 <see cref="Key"/>의 길이의 8배
        /// </summary>
        public int KeySize { get; set; }

        /// <summary>
        /// <see cref="Key"/>값을 변경합니다.
        /// </summary>
        private void SetupKey() {
            if(Key == null) {
                Key = CryptoTool.DerivePassword(Password, 32);
                KeySize = Key.Length * 8;
            }
        }

        /// <summary>
        /// AriaEngine을 빌드하고 반환합니다.
        /// </summary>
        /// <returns></returns>
        private AriaEngine GetEngine() {
            if(_engine == null) {
                SetupKey();

                _engine = new AriaEngine(KeySize);
                _engine.SetKey(Key);
                _engine.SetupRoundKeys();
            }

            return _engine;
        }

        /// <summary>
        /// 지정한 정보를 암호화한다.
        /// </summary>
        /// <param name="plainBytes">암호화할 정보</param>
        /// <returns>암호화된 정보</returns>
        public byte[] Encrypt(byte[] plainBytes) {
            plainBytes.ShouldNotBeNull("plainBytes");

            var engine = GetEngine();

            if(IsDebugEnabled)
                log.Debug("정보를 암호화 합니다... keySize=[{0}]", KeySize);

            var encrypt = BlockPadding.Instance.AddPadding(plainBytes, ARIA_BLOCK_SIZE);
            var blockCount = encrypt.Length / ARIA_BLOCK_SIZE;

            for(int i = 0; i < blockCount; i++) {
                var buffer = new byte[ARIA_BLOCK_SIZE];
                Buffer.BlockCopy(encrypt, (i * ARIA_BLOCK_SIZE), buffer, 0, ARIA_BLOCK_SIZE);

                buffer = engine.Encrypt(buffer, 0);
                Buffer.BlockCopy(buffer, 0, encrypt, (i * ARIA_BLOCK_SIZE), buffer.Length);
            }
            return encrypt;
        }

        /// <summary>
        /// 지정한 암호화된 정보를 복호화한다.
        /// </summary>
        /// <param name="cipher">암호화된 정보</param>
        /// <returns>복호화된 정보</returns>
        public byte[] Decrypt(byte[] cipher) {
            cipher.ShouldNotBeNull("cipher");

            if(IsDebugEnabled)
                log.Debug("암호화된 정보를 복호화합니다... keySize=[{0}]", KeySize);

            var decrypt = (byte[])cipher.Clone();

            var engine = GetEngine();
            var blockCount = decrypt.Length / ARIA_BLOCK_SIZE;

            for(var i = 0; i < blockCount; i++) {
                var buffer = new byte[ARIA_BLOCK_SIZE];
                Buffer.BlockCopy(decrypt, (i * ARIA_BLOCK_SIZE), buffer, 0, ARIA_BLOCK_SIZE);

                buffer = engine.Decrypt(buffer, 0);
                Buffer.BlockCopy(buffer, 0, decrypt, (i * ARIA_BLOCK_SIZE), buffer.Length);
            }

            return BlockPadding.Instance.RemovePadding(decrypt, ARIA_BLOCK_SIZE);
        }

        public void Dispose() {
            _engine = null;
        }
    }
}