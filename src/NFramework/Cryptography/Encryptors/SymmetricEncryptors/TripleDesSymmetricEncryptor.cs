using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// Triple DES 대칭형 알고리즘을 사용한 암호화 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class TripleDESSymmetricEncryptor : SymmetricEncryptor {
        public TripleDESSymmetricEncryptor() : base(new TripleDESCryptoServiceProvider()) {}
    }
}