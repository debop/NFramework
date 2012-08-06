using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// SHA384 해시 알고리즘을 이용한 암호화기. 차라리 <see cref="SHA512HashEncryptor"/>를 사용하세요.
    /// </summary>
    [Serializable]
    public sealed class SHA384HashEncryptor : HashEncryptor {
        public SHA384HashEncryptor() : base(new SHA384CryptoServiceProvider()) {}
    }
}