using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace NSoft.NFramework.Cryptography.Encryptors {
    [TestFixture]
    public class SymmetricEncryptorFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string PlainTextKR = @"동해물과 백두산이 마르고 닳도록, 하느님이 보우하사";
        private const string PlainTextUS = @"Hello World! .NET Framework is Best";

        [Test]
        public void TripleDES() {
            using(var encryptor = new SymmetricEncryptor(new TripleDESCryptoServiceProvider())) {
                ValidateSymmetricEncryptor(encryptor);
            }
        }

        [Test]
        public void RC2() {
            using(var encryptor = new SymmetricEncryptor(new RC2CryptoServiceProvider())) {
                ValidateSymmetricEncryptor(encryptor);
            }
        }

        [Test]
        public void DES() {
            using(var encryptor = new SymmetricEncryptor(new DESCryptoServiceProvider())) {
                ValidateSymmetricEncryptor(encryptor);
            }
        }

        [Test]
        public void Rijndael() {
            using(var encryptor = new SymmetricEncryptor(new RijndaelManaged { Mode = CipherMode.CBC })) {
                ValidateSymmetricEncryptor(encryptor);
            }
        }

        protected static void ValidateSymmetricEncryptor(ISymmetricEncryptor encryptor) {
            var cipherKR = encryptor.Encrypt(Encoding.Unicode.GetBytes(PlainTextKR));
            var clearKR = Encoding.Unicode.GetString(encryptor.Decrypt(cipherKR));
            Assert.AreEqual(PlainTextKR, clearKR);

            var cipherUS = encryptor.Encrypt(Encoding.Unicode.GetBytes(PlainTextUS));
            var clearUS = Encoding.Unicode.GetString(encryptor.Decrypt(cipherUS));
            Assert.AreEqual(PlainTextUS, clearUS);
        }

        [Test]
        [TestCase("리얼웹은 BPM 전문 솔루션 업체입니다.")]
        [TestCase("RealWeb is BPM solution vendor.")]
        public void TripleDES_CrossTest(string plainText) {
            using(var encryptor1 = new SymmetricEncryptor(new TripleDESCryptoServiceProvider()))
            using(var encryptor2 = new SymmetricEncryptor(new TripleDESCryptoServiceProvider())) {
                var bytes = Encoding.Unicode.GetBytes(plainText);

                var cipher1 = encryptor1.Encrypt(bytes);
                var cipher2 = encryptor2.Encrypt(bytes);

                var clean1 = Encoding.Unicode.GetString(encryptor1.Decrypt(cipher1));
                var clean2 = Encoding.Unicode.GetString(encryptor2.Decrypt(cipher2));

                Assert.AreEqual(clean1, clean2);

                var cross1 = Encoding.Unicode.GetString(encryptor1.Decrypt(cipher2));
                var cross2 = Encoding.Unicode.GetString(encryptor2.Decrypt(cipher1));

                Assert.AreEqual(cross1, cross2);
            }
        }

        [Test]
        [TestCase("리얼웹은 BPM 전문 솔루션 업체입니다.")]
        [TestCase("RealWeb is BPM solution vendor.")]
        public void RC2_CrossTest(string plainText) {
            using(var encryptor1 = new SymmetricEncryptor(new RC2CryptoServiceProvider()))
            using(var encryptor2 = new SymmetricEncryptor(new RC2CryptoServiceProvider())) {
                var plainByte1 = Encoding.Unicode.GetBytes(plainText);
                var plainByte2 = Encoding.Unicode.GetBytes(plainText);

                Assert.AreEqual(plainByte1, plainByte2);

                var cipher1 = encryptor1.Encrypt(plainByte1);
                var cipher2 = encryptor2.Encrypt(plainByte2);

                Assert.AreEqual(cipher1, cipher2);
                Assert.AreEqual(cipher1.BytesToHex(), cipher2.BytesToHex());

                var clean1 = Encoding.Unicode.GetString(encryptor1.Decrypt(cipher1));
                var clean2 = Encoding.Unicode.GetString(encryptor2.Decrypt(cipher2));

                Assert.AreEqual(clean1, clean2);

                var cross1 = Encoding.Unicode.GetString(encryptor1.Decrypt(cipher2));
                var cross2 = Encoding.Unicode.GetString(encryptor2.Decrypt(cipher1));

                Assert.AreEqual(cross1, cross2);
            }
        }

        [Test]
        [TestCase("리얼웹은 BPM 전문 솔루션 업체입니다!?!~~")]
        [TestCase("RealWeb is BPM solution vendor!?!~~")]
        public void Rijndael_CrossTest(string plainText) {
            using(var encryptor1 = new SymmetricEncryptor(new RijndaelManaged()))
            using(var encryptor2 = new SymmetricEncryptor(new RijndaelManaged())) {
                var plainByte1 = Encoding.Unicode.GetBytes(plainText);
                var plainByte2 = Encoding.Unicode.GetBytes(plainText);

                Assert.AreEqual(plainByte1, plainByte2);

                var cipher1 = encryptor1.Encrypt(plainByte1);
                var cipher2 = encryptor2.Encrypt(plainByte2);

                Assert.AreEqual(cipher1, cipher2);
                Assert.AreEqual(cipher1.BytesToHex(), cipher2.BytesToHex());

                var clean1 = Encoding.Unicode.GetString(encryptor1.Decrypt(cipher1));
                var clean2 = Encoding.Unicode.GetString(encryptor2.Decrypt(cipher2));

                Assert.AreEqual(clean1, clean2);

                var cross1 = Encoding.Unicode.GetString(encryptor1.Decrypt(cipher2));
                var cross2 = Encoding.Unicode.GetString(encryptor2.Decrypt(cipher1));

                Assert.AreEqual(cross1, cross2);
            }
        }
    }
}