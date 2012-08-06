using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.Serializations.Serializers;
using NUnit.Framework;

namespace NSoft.NFramework.Cryptography {
    /// <summary>
    /// 객체의 직렬화/역직렬화시 암호화를 수행한다.
    /// </summary>
    [TestFixture]
    public class CryptoSerializerFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly ISymmetricEncryptor[] symmetricEncryptors = new ISymmetricEncryptor[]
                                                                     {
                                                                         new AriaSymmetricEncryptor(),
                                                                         new DESSymmetricEncryptor(),
                                                                         new RC2SymmetricEncryptor(),
                                                                         new RijndaelSymmetricEncryptor(),
                                                                         new TripleDESSymmetricEncryptor()
                                                                     };

        [Test]
        public void Serialize_By_SymmetricEncryptor() {
            var user = UserInfo.GetSample();

            foreach(var encryptor in symmetricEncryptors) {
                var serializer = new EncryptSerializer<UserInfo>(new BinarySerializer<UserInfo>(), encryptor);

                var data = serializer.Serialize(user);
                var user2 = serializer.Deserialize(data);

                Assert.AreEqual(user.FirstName, user2.FirstName);
                Assert.AreEqual(user.FavoriteMovies.Count, user.FavoriteMovies.Count);
            }
        }
    }
}