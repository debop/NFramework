using System;

namespace NSoft.NFramework.Cryptography {
    /// <summary>
    /// 대칭형 암호화를 수행합니다.
    /// </summary>
    public interface ISymmetricEncryptor : IDisposable {
        ///// <summary>
        ///// 대칭형 암호화 Algorithm
        ///// </summary>
        //SymmetricAlgorithm Algorithm { get; }

        ///// <summary>
        ///// 암호화시 사용될 비밀번호
        ///// </summary>
        //string Key { get; }

        /// <summary>
        /// 지정한 정보를 암호화한다.
        /// </summary>
        /// <param name="plainBytes">암호화할 정보</param>
        /// <returns>암호화된 정보</returns>
        byte[] Encrypt(byte[] plainBytes);

        /// <summary>
        /// 지정한 암호화된 정보를 복호화한다.
        /// </summary>
        /// <param name="cipher">암호화된 정보</param>
        /// <returns>복호화된 정보</returns>
        byte[] Decrypt(byte[] cipher);
    }
}