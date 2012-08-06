using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// DES 대칭형 알고리즘을 사용한 암호 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class DESSymmetricEncryptor : SymmetricEncryptor {
        public DESSymmetricEncryptor() : base(new DESCryptoServiceProvider()) {}
    }
}