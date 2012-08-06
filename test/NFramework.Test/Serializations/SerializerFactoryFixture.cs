using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Serializations {
    [Microsoft.Silverlight.Testing.Tag("Serializer")]
    [TestFixture]
    public class SerializerFactoryFixture : SerializerFixtureBase {
        [Test]
        public void CreateSerializerTest() {
            foreach(var option in Options) {
                var serializer = SerializerTool.CreateSerializer<UserInfo>(option);
                serializer.Should().Not.Be.Null();
            }
        }
    }
}