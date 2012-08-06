using MongoDB.Driver;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Util {
    [TestFixture]
    public class MongoToolFixture {
        public static readonly string ConnectionString = "server=localhost;safe=true;database=NHCaches";

        [Test]
        public void CreateServer() {
            var server = MongoTool.CreateMongoServer(ConnectionString);
            server.Connect();

            server.State.Should().Be(MongoServerState.Connected);
        }
    }
}