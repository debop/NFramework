using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Core {
    [TestFixture]
    public class MongoServerFixture : MongoFixtureBase {
        private static readonly MongoServerAddress[] LocalServers = new MongoServerAddress[] { new MongoServerAddress("localhost") };

        [Test]
        public void ConnectServerTest() {
            var server = MongoServer.Create();
            server.Connect();

            server.Ping();

            server.Instances.Count().Should().Be(1); // 접속 전에는 Instance 수가 0이다.

            server.Settings.DefaultCredentials.Should().Be.Null();
            server.Settings.GuidRepresentation.Should().Be(MongoDefaults.GuidRepresentation);
            server.Settings.SafeMode.Should().Be(SafeMode.False);
            server.Settings.SlaveOk.Should().Be(false);
            server.Settings.Servers.SequenceEqual(LocalServers).Should().Be.True();

            server.State.Should().Be(MongoServerState.Connected);

            server.Disconnect();
        }

        [Test]
        public void DatabaseExistsTest() {
            Database.Drop();
            Server.DatabaseExists(DefaultDatabaseName).Should().Be.False();

            Database["test"].Insert(new BsonDocument("x", 1)).Ok.Should().Be.True();
            Server.DatabaseExists(DefaultDatabaseName).Should().Be.True();
        }

        [Test]
        public void DropDatabaseTest() {
            var test = Database["test"];
            test.Insert(new BsonDocument()).Ok.Should().Be.True();

            Server.GetDatabaseNames().Any(name => name == DefaultDatabaseName).Should().Be.True();

            Server.DropDatabase(DefaultDatabaseName).Ok.Should().Be.True();
            Server.GetDatabaseNames().Any(name => name == DefaultDatabaseName).Should().Be.False();
        }

        [Test]
        public void ReconnectTest() {
            Server.Reconnect();
            Server.State.Should().Be(MongoServerState.Connected);
        }

        [Test]
        public void RunAdminCommandAs() {
            ((CommandResult)Server.RunAdminCommandAs(typeof(CommandResult), "ping")).Ok.Should().Be.True();
            Server.RunAdminCommandAs<CommandResult>("ping").Ok.Should().Be.True();
        }

        [Test]
        public void BuildInfoTest() {
            Server.BuildInfo.Version.Should().Not.Be(new Version(0, 0, 0, 0));
        }
    }
}