using System.Security.Cryptography;
using NUnit.Framework;

namespace NSoft.NFramework.Cryptography.Encryptors {
    [TestFixture]
    public class HashEncryptorFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string PlainText_KR = @"동해물과 백두산이 마르고 닳도록";

        [Test]
        public void Default() {
            using(var encryptor = new HashEncryptor())
                AssertHashValue(encryptor);
        }

        [Test]
        public void HMACMD5() {
            using(var encryptor = new HashEncryptor(new HMACMD5()))
                AssertHashValue(encryptor);
        }

        [Test]
        public void MACTripleDES() {
            using(var encryptor = new HashEncryptor(new MACTripleDES()))
                AssertHashValue(encryptor);
        }

        [Test]
        public void MD5() {
            using(var encryptor = new HashEncryptor(new MD5CryptoServiceProvider()))
                AssertHashValue(encryptor);
        }

        [Test]
        public void SHA1() {
            using(var encryptor = new HashEncryptor(new SHA1CryptoServiceProvider()))
                AssertHashValue(encryptor);
        }

        [Test]
        public void SHA256() {
            using(var encryptor = new HashEncryptor(new SHA256CryptoServiceProvider()))
                AssertHashValue(encryptor);
        }

        [Test]
        public void SHA384() {
            using(var encryptor = new HashEncryptor(new SHA384CryptoServiceProvider()))
                AssertHashValue(encryptor);
        }

        [Test]
        public void SHA512() {
            using(var encryptor = new HashEncryptor(new SHA512CryptoServiceProvider()))
                AssertHashValue(encryptor);
        }

        private static void AssertHashValue(IHashEncryptor encryptor) {
            encryptor.ShouldNotBeNull("encryptor");

            var hash1 = encryptor.ComputeHash(PlainText_KR, EncryptionStringFormat.HexDecimal);
            var hash2 = encryptor.ComputeHash(PlainText_KR, EncryptionStringFormat.HexDecimal);

            Assert.AreEqual(hash1, hash2);

            if(IsDebugEnabled)
                log.Debug("암호화=" + hash1);
        }
    }
}