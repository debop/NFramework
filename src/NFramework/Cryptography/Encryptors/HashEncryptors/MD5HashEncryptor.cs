using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    [Serializable]
    public class MD5HashEncryptor : HashEncryptor {
        public MD5HashEncryptor() : base(new MD5CryptoServiceProvider()) {}
    }
}