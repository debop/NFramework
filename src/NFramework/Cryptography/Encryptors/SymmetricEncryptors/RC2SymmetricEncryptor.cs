using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// RC2 대칭형 알고리즘을 사용하는 암호화 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class RC2SymmetricEncryptor : SymmetricEncryptor {
        public RC2SymmetricEncryptor() : base(new RC2CryptoServiceProvider()) {}
    }
}