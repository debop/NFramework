using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// SHA512 해시 알고리즘을 이용한 암호화기
    /// </summary>
    [Serializable]
    public class SHA512HashEncryptor : HashEncryptor {
        public SHA512HashEncryptor() : base(new SHA512CryptoServiceProvider()) {}
    }
}