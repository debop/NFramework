using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// Rijndael 대칭형 알고리즘을 사용하는 암호화기입니다. 가장 많이 사용되어지는 알고리즘입니다.
    /// </summary>
    [Serializable]
    public sealed class RijndaelSymmetricEncryptor : SymmetricEncryptor {
        public RijndaelSymmetricEncryptor() : base(new RijndaelManaged()) {}
    }
}