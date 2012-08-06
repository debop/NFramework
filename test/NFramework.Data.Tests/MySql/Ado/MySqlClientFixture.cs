using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MySql.Ado {
    [TestFixture]
    public class MySqlClientFixture : MySqlAdoFixtureBase {
        #region <<logger>>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public static string DefaultConnectionString {
            get { return AdoTool.DefaultConnectionStringSettings.ConnectionString; }
        }

        [Test]
        public void ConnectionTest() {
            DefaultConnectionString.Should().Not.Be.Empty();

            using(var conn = new MySqlConnection(DefaultConnectionString)) {
                conn.Open();
                conn.Ping().Should().Be.True();
            }
        }

        [Test]
        public void ExecuteReaderTest() {
            using(var conn = new MySqlConnection(DefaultConnectionString))
            using(var cmd = new MySqlCommand(SQL_CUSTOMER_SELECT, conn)) {
                conn.Open();
                using(var reader = cmd.ExecuteReader()) {
                    var customers = reader.Map<Customer>(() => new Customer(), new TrimNameMapper());
                    customers.Count.Should().Be.GreaterThan(0);

                    customers.All(customer => customer.CompanyName.IsNotWhiteSpace()).Should().Be.True();
                }
            }
        }

        [Test]
        public void ExecuteReaderAsyncTest() {
            using(var conn = new MySqlConnection(DefaultConnectionString))
            using(var cmd = new MySqlCommand(SQL_CUSTOMER_SELECT, conn)) {
                conn.Open();

                //! MySqlCommandAsync 를 참조하세요
                //
                var ar = cmd.BeginExecuteReader();

                Thread.Sleep(1);

                using(var reader = cmd.EndExecuteReader(ar)) {
                    var customers = reader.Map<Customer>(() => new Customer(), new TrimNameMapper());
                    customers.Count.Should().Be.GreaterThan(0);

                    customers.All(customer => customer.CompanyName.IsNotWhiteSpace()).Should().Be.True();
                }
            }
        }

        [TestCase(4, 5)]
        [TestCase(2, 10)]
        public void MultiThreadTest(int repeat, int concurrent) {
            for(int i = 0; i < repeat; i++) {
                TestTool.RunTasks(concurrent,
                                  ConnectionTest,
                                  ExecuteReaderTest,
                                  ExecuteReaderAsyncTest);
                Thread.Sleep(10);
            }
        }
    }
}