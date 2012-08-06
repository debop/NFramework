using System.Text;
using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Cryptography.Aria {
    [Microsoft.Silverlight.Testing.Tag("Cryptography")]
    [TestFixture]
    public class AriaSymmetricEncrptorFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string PlainText = @"ARIA12동해물과백두산이-어쩌라구요?-#모르지요-ARIA Algorithm이 뭐 이래...";
        private const string PlainTextKR = @"동해물과 백두산이 마르고 닳도록, 하느님이 보우하사, 우리나라 만쉐이^^ 와우 괞찮았어?!!! Hello Hello Mr. Mockey";
        private const string PlainTextUS = @"Hello World! .NET Framework is Best!!! 정말이냐?";

        private AriaSymmetricEncryptor _aria;

        [SetUp]
        public void SetUp() {
            _aria = new AriaSymmetricEncryptor();
        }

        [Test]
        public void SimpleTest() {
            var plainBytes = Encoding.Unicode.GetBytes(PlainText);
            var cipherBytes = _aria.Encrypt(plainBytes);
            var restoreBytes = _aria.Decrypt(cipherBytes);
            var restoreText = StringTool.GetString(Encoding.Unicode, restoreBytes);

            if(IsDebugEnabled) {
                log.Debug("PlainText=[{0}]", PlainText);
                log.Debug("PlainBytes=  [{0}], Length=[{1}]", plainBytes.BytesToString(EncryptionStringFormat.HexDecimal),
                          plainBytes.Length);
                log.Debug("CipherBytes= [{0}], Length=[{1}]", cipherBytes.BytesToString(EncryptionStringFormat.HexDecimal),
                          cipherBytes.Length);
                log.Debug("RestoreBytes=[{0}], Length=[{1}]", restoreBytes.BytesToString(EncryptionStringFormat.HexDecimal),
                          restoreBytes.Length);
                log.Debug("restoreText=[{0}]", restoreText);
            }

            Assert.AreEqual(PlainText, restoreText);
        }

        [TestCase(PlainTextKR)]
        [TestCase(PlainTextUS)]
        public void EncryptTest(string plainText) {
            var cipherText = _aria.Encrypt(Encoding.Unicode.GetBytes(plainText));
            var plainBytes = _aria.Decrypt(cipherText);
            var restoreText = StringTool.GetString(Encoding.Unicode, plainBytes);
            Assert.AreEqual(plainText, restoreText);
        }

        [Test]
        [TestCase(PlainTextKR)]
        [TestCase(PlainTextUS)]
        public void CrossTest(string plainText) {
            using(var encryptor1 = new AriaSymmetricEncryptor())
            using(var encryptor2 = new AriaSymmetricEncryptor()) {
                var plainByte1 = Encoding.Unicode.GetBytes(plainText);
                var plainByte2 = Encoding.Unicode.GetBytes(plainText);

                Assert.AreEqual(plainByte1, plainByte2);

                var cipher1 = encryptor1.Encrypt(plainByte1);
                var cipher2 = encryptor2.Encrypt(plainByte2);

                Assert.AreEqual(cipher1, cipher2);
                Assert.AreEqual(cipher1.BytesToHex(), cipher2.BytesToHex());

                var clean1 = StringTool.GetString(Encoding.Unicode, encryptor1.Decrypt(cipher1));
                var clean2 = StringTool.GetString(Encoding.Unicode, encryptor2.Decrypt(cipher2));

                Assert.AreEqual(clean1, clean2);

                var cross1 = StringTool.GetString(Encoding.Unicode, encryptor1.Decrypt(cipher2));
                var cross2 = StringTool.GetString(Encoding.Unicode, encryptor2.Decrypt(cipher1));

                Assert.AreEqual(cross1, cross2);
            }
        }
    }
}