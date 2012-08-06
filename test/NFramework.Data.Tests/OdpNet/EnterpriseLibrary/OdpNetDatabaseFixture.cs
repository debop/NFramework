using Microsoft.Practices.EnterpriseLibrary.Data;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.OdpNet.EnterpriseLibrary {
    [TestFixture]
    public class OdpNetDatabaseFixture : IoCSetupBase {
        [Test]
        public void CreateDatabaseTest() {
            var database = DatabaseFactory.CreateDatabase();
            database.Should().Not.Be.Null();
            database.ConnectionString.Should().Not.Be.Empty();
        }

        [Test]
        public void CreateDatabaseByFactoryTest() {
            var database = DatabaseFactory.CreateDatabase();
            database.ConnectionString.Should().Be(AdoTool.DefaultConnectionStringSettings.ConnectionString);
            database.Should().Be.InstanceOf<OdpNetDatabase>();
        }

        [Test]
        public void ExecuteScalarTest() {
            var database = DatabaseFactory.CreateDatabase();

            using(var command = database.GetSqlStringCommand("SELECT COUNT(*) FROM TAB"))
                database.ExecuteScalar(command).AsInt().Should().Be.GreaterThan(0);
        }
    }
}