using NSoft.NFramework.Serializations.Serializers;
using NUnit.Framework;

namespace NSoft.NFramework.Serializations {
    [TestFixture]
    public class BinarySerializerFixture {
        private static readonly ISerializer<UserInfo> UserSerializer = new BinarySerializer<UserInfo>();

        [Test]
        public void ObjectTest() {
            var user = UserInfo.GetSample();

            var data = UserSerializer.Serialize(user);
            var user2 = UserSerializer.Deserialize(data);

            Assert.AreEqual(user.FirstName, user2.FirstName);
            Assert.AreEqual(user.FavoriteMovies.Count, user2.FavoriteMovies.Count);
        }
    }
}