using System;

namespace NSoft.NFramework.Cryptography.Aria {
    /// <summary>
    /// This class is.
    /// This block is a block cipher algorithm, type the password should be an exact multiple of the size.
    /// If you, or an exact multiple of plaintext to be encrypted,
    /// encrypting the data before adding padding to support the operation.
    /// </summary>
    [Serializable]
    internal abstract class BlockPadding {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string ANSIX = @"ANSIX923";
        public const string ISO = @"ISO10126";
        public const string PKCS7 = @"PKCS7";

        private static BlockPadding _instance = new AnsiX923Padding();

        public static BlockPadding Instance {
            get { return _instance; }
        }

        /// <summary>
        /// To provide byte padding to create an instance.
        /// Byte padding to provide that kind of 'ANSI X.923', 'ISO 10126', 'PKCS7' is.
        /// </summary>
        /// <param name="paddingType">type The type of padding bytes (ANSI X.923 : ANSIX923, ISO 10126 : ISO10126, PKCS7 : PKCS7)</param>
        /// <returns>Byte padding instance, if that does not support the null of the padding bytes is returned.</returns>
        public static BlockPadding GetInstance(string paddingType) {
            paddingType.ShouldNotBeWhiteSpace("paddingType");

            var isCreate = false;

            if(_instance == null) {
                isCreate = true;
            }
            else if(_instance.PaddingType.Equals(paddingType) == false) {
                isCreate = true;
            }

            if(isCreate) {
                switch(paddingType.ToUpper()) {
                    case ANSIX:
                        _instance = new AnsiX923Padding();
                        break;
                    case ISO:
                    case PKCS7:
                    default:
                        _instance = null;
                        break;
                }
            }
            return _instance;
        }

        /// <summary>
        /// 현 인스턴스의 패딩 타입
        /// </summary>
        public abstract string PaddingType { get; }

        /// <summary>
        /// <paramref name="blockSize"/> 만큼 <paramref name="source"/>에 padding을 추가합니다.
        /// </summary>
        /// <param name="source">암호화를 위한 대상</param>
        /// <param name="blockSize">블록 크기</param>
        /// <returns>암호화 대상에 패딩 정보가 포함된 정보</returns>
        public abstract byte[] AddPadding(byte[] source, int blockSize);

        /// <summary>
        /// <paramref name="blockSize"/> 만큼 <paramref name="source"/>에 padding을 제거합니다.
        /// </summary>
        /// <param name="source">암호화를 위한 대상</param>
        /// <param name="blockSize">블록 크기</param>
        /// <returns>암호화 대상에 패딩 정보가 제거된 정보</returns>
        public abstract byte[] RemovePadding(byte[] source, int blockSize);
    }
}