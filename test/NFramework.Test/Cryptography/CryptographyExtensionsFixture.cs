using System.Collections.Generic;
using System.Text;
using NSoft.NFramework.Cryptography.Encryptors;
using NUnit.Framework;

namespace NSoft.NFramework.Cryptography {
    [TestFixture]
    public class CryptographyExtensionsFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string PlainText = @"ARIA12동해물과백두산이";
        private const string PlainTextKR = @"동해물과 백두산이 마르고 닳도록, 하느님이 보우하사";
        private const string PlainTextUS = @"Hello World! .NET Framework is Best";

        private readonly List<IHashEncryptor> hashers = new List<IHashEncryptor>
                                                        {
                                                            new MD5HashEncryptor(),
                                                            new SHA1HashEncryptor(),
                                                            new SHA256HashEncryptor(),
                                                            new SHA384HashEncryptor(),
                                                            new SHA512HashEncryptor()
                                                        };

        private readonly List<ISymmetricEncryptor>
            symmetricEncryptors = new List<ISymmetricEncryptor>
                                  {
                                      new DESSymmetricEncryptor(),
                                      new TripleDESSymmetricEncryptor(),
                                      new RC2SymmetricEncryptor(),
                                      new RijndaelSymmetricEncryptor(),
                                      new AriaSymmetricEncryptor()
                                  };

        [TestCase(PlainText)]
        [TestCase(PlainTextKR)]
        [TestCase(PlainTextUS)]
        public void ComputeHashToBytesTest(string plainText) {
            foreach(var hasher in hashers) {
                var hash1 = hasher.ComputeHashToBytes(plainText);
                var hash2 = hasher.ComputeHashToBytes(plainText);

                Assert.AreEqual(hash1, hash2);

                var hash3 = hasher.ComputeHashToBytes(plainText + " ");
                Assert.AreNotEqual(hash1, hash3);
            }
        }

        [TestCase(PlainText)]
        [TestCase(PlainTextKR)]
        [TestCase(PlainTextUS)]
        public void ComputeHashToStringTest(string plainText) {
            foreach(var hasher in hashers) {
                var hash1 = hasher.ComputeHashToString(plainText);
                var hash2 = hasher.ComputeHashToString(plainText);

                Assert.AreEqual(hash1, hash2);

                var hash3 = hasher.ComputeHashToString(plainText + " ");
                Assert.AreNotEqual(hash1, hash3);
            }
        }

        [TestCase(PlainText)]
        [TestCase(PlainTextKR)]
        [TestCase(PlainTextUS)]
        public void EncryptBytesTest(string plainText) {
            var plainBytes = Encoding.Unicode.GetBytes(plainText);

            foreach(var encryptor in symmetricEncryptors) {
                var cipherBytes = encryptor.EncryptBytes(plainBytes);
                var restoreBytes = encryptor.DecryptBytes(cipherBytes);

                var restoreText = Encoding.Unicode.GetString(restoreBytes).Trim('\0');

                Assert.AreEqual(plainText, restoreText);
            }
        }

        [TestCase(PlainText)]
        [TestCase(PlainTextKR)]
        [TestCase(PlainTextUS)]
        public void EncryptStringTest(string plainText) {
            foreach(var encryptor in symmetricEncryptors) {
                var cipherText = encryptor.EncryptString(plainText);
                var restoreText = encryptor.DecryptString(cipherText);
                Assert.AreEqual(plainText, restoreText);
            }
        }
    }
}