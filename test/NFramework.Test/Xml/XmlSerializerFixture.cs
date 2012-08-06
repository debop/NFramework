using System;
using System.Collections.Generic;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.Serializations.Serializers;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Xml {
    [TestFixture]
    public class XmlSerializerFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static List<User> _users;

        public static List<User> Users {
            get { return _users ?? (_users = GetSampleUsers(100)); }
        }

        private static List<User> GetSampleUsers(int count) {
            var users = new List<User>();

            for(int i = 0; i < count; i++) {
                var user = new User
                           {
                               Id = Guid.NewGuid(),
                               Name = "User " + i.ToString("X2"),
                               Description = "Xml Serializer 테스트 "
                           };
                users.Add(user);
            }
            return users;
        }

        [Test]
        public void Serialize_Deserialize_User() {
            var users = GetSampleUsers(10);

            foreach(var user in users) {
                var userData = XmlSerializer<User>.Instance.Serialize(user);
                var deserialized = XmlSerializer<User>.Instance.Deserialize(userData);

                Assert.AreEqual(user, deserialized);
            }
        }

        [Test]
        public void Serialize_Deserialize_UserCollection() {
            var users = GetSampleUsers(10);

            var usersData = XmlSerializer<List<User>>.Instance.Serialize(users);
            var deserializedUsers = XmlSerializer<List<User>>.Instance.Deserialize(usersData);

            CollectionAssert.AreEqual(users, deserializedUsers);
        }

        [Test, Combinatorial]
        public void XmlSerialize_With_Compress_Encryption([Values(typeof(SharpBZip2Compressor),
                                                              typeof(GZipCompressor),
                                                              typeof(DeflateCompressor),
                                                              typeof(SevenZipCompressor))] Type compressorType,
                                                          [Values(typeof(AriaSymmetricEncryptor),
                                                              typeof(RC2SymmetricEncryptor),
                                                              typeof(TripleDESSymmetricEncryptor))] Type encryptorType) {
            var xmlSerializer = XmlSerializer<List<User>>.Instance;
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var serializer = new EncryptSerializer<List<User>>(new CompressSerializer<List<User>>(xmlSerializer, compressor), encryptor);

            var users = GetSampleUsers(10);

            var usersData = serializer.Serialize(users);
            var deserializedUsers = serializer.Deserialize(usersData);

            CollectionAssert.AreEqual(users, deserializedUsers);
        }

        [Test, Combinatorial]
        public void XmlSerialize_With_Encryption_Compress([Values(typeof(SharpBZip2Compressor),
                                                              typeof(GZipCompressor),
                                                              typeof(DeflateCompressor),
                                                              typeof(SevenZipCompressor))] Type compressorType,
                                                          [Values(typeof(AriaSymmetricEncryptor),
                                                              typeof(RC2SymmetricEncryptor),
                                                              typeof(TripleDESSymmetricEncryptor))] Type encryptorType) {
            var xmlSerializer = XmlSerializer<List<User>>.Instance;
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var serializer = new CompressSerializer<List<User>>(new EncryptSerializer<List<User>>(xmlSerializer, encryptor), compressor);

            var users = GetSampleUsers(10);

            var usersData = serializer.Serialize(users);
            var deserializedUsers = serializer.Deserialize(usersData);

            CollectionAssert.AreEqual(users, deserializedUsers);
        }
    }
}