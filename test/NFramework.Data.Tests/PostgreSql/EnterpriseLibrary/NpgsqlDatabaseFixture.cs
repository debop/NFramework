using System;
using System.Data;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using Npgsql;
using SharpTestsEx;

namespace NSoft.NFramework.Data.PostgreSql.EnterpriseLibrary {

    [TestFixture]
    public class NpgsqlDatabaseFixture : IoCSetupBase {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly string ConnectionString = ConfigTool.GetConnectionString("Northwind.PostgreSql");

        [Test]
        public void CreateDatabaseTest() {

            var database = new NpgsqlDatabase(ConnectionString);
            database.Should().Not.Be.Null();
        }

        [Test]
        public void CreateConnectionTest() {

            var database = new NpgsqlDatabase(ConnectionString);
            database.Should().Not.Be.Null();

            using(var connection = (NpgsqlConnection)database.CreateConnection()) {
                connection.Open();

                connection.State.Should().Be(ConnectionState.Open);
            }
        }

        [Test]
        public void ExecuteReaderTest() {

            var database = new NpgsqlDatabase(ConnectionString);
            database.Should().Not.Be.Null();

            using(var connection = (NpgsqlConnection)database.CreateConnection()) {
                connection.Open();
                connection.State.Should().Be(ConnectionState.Open);

                using(var cmd = new NpgsqlCommand("SELECT * FROM Invoices", connection)) {
                    using(var reader = cmd.ExecuteReader()) {
                        Console.WriteLine(reader.ToString(true));
                    }
                }
            }
        }
    }
}