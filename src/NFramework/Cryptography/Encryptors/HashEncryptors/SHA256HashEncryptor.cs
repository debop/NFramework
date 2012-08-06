using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// SHA256 해시 알고리즘을 이용한 암호화기
    /// </summary>
    [Serializable]
    public class SHA256HashEncryptor : HashEncryptor {
        public SHA256HashEncryptor() : base(new SHA256Managed()) {}
    }
}