using NSoft.NFramework.Json;
using NSoft.NFramework.Serializations.Serializers;
using NUnit.Framework;

namespace NSoft.NFramework.Compressions {
    /// <summary>
    /// JsonByteSerializer 가 BinarySerializer 보다 성능이 좋습니다.
    /// </summary>
    [TestFixture]
    public class CompressSerializerFixture : CompressorFixtureBase {
        private static readonly UserInfo user = UserInfo.GetSample();

        [Test]
        public void Compress_BinarySerialize() {
            var serializer = new CompressSerializer<UserInfo>(new BinarySerializer<UserInfo>());

            var serializedUser = serializer.Serialize(user);
            var deserializedUser = serializer.Deserialize(serializedUser);

            Assert.AreEqual(user.FirstName, deserializedUser.FirstName);
            Assert.AreEqual(user.FavoriteMovies.Count, user.FavoriteMovies.Count);
        }

        [Test]
        public void Compress_JsonByteSerializer() {
            var serializer = new CompressSerializer<UserInfo>(new JsonByteSerializer<UserInfo>());

            var serializedUser = serializer.Serialize(user);
            var deserializedUser = serializer.Deserialize(serializedUser);

            Assert.AreEqual(user.FirstName, deserializedUser.FirstName);
            Assert.AreEqual(user.FavoriteMovies.Count, user.FavoriteMovies.Count);
        }

        [Test]
        public void Compress_BsonSerializer() {
            var serializer = new CompressSerializer<UserInfo>(new BsonSerializer<UserInfo>());

            var serializedUser = serializer.Serialize(user);
            var deserializedUser = serializer.Deserialize(serializedUser);

            Assert.AreEqual(user.FirstName, deserializedUser.FirstName);
            Assert.AreEqual(user.FavoriteMovies.Count, user.FavoriteMovies.Count);
        }
    }
}