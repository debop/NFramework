using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Cryptography.Encryptors {
    [Serializable]
    public class SHA1HashEncryptor : HashEncryptor {
        public SHA1HashEncryptor() : base(new SHA1Managed()) {}
    }
}