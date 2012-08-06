using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography {
    public interface HashEncryptor : IDisposable {
        /// <summary>
        /// Hash Algorithm 종류
        /// </summary>
        HashAlgorithm Algorithm { get; }

        /// <summary>
        /// 지정된 문자열을 암호화를 수행한다.
        /// </summary>
        /// <param name="plainText">암호화할 문자열</param>
        /// <returns>Hashing한 정보</returns>
        /// <exception cref="ArgumentNullException">암호화할 문자열이 빈 문자열일 때</exception>
        byte[] ComputeHash(string plainText);

        /// <summary>
        /// 지정된 문자열을 암호화를 수행하고, 값을 일반적인 값으로 반환한다.
        /// </summary>
        /// <param name="plainText">암호화할 문자열</param>
        /// <param name="format">암호화한 정보를 문자열로 표현시 사용할 포맷 (Base64/Hex)</param>
        /// <returns>Hashing한 정보</returns>
        /// <exception cref="ArgumentNullException">암호화할 문자열이 빈 문자열일 때</exception>
        string ComputeHash(string plainText, EncryptionStringFormat format);
    }
}