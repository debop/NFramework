using System;
using Devart.Data.Oracle;
using NUnit.Framework;

namespace NSoft.NFramework.Data.DevartOracle.Test.DevartSamples {
    [TestFixture]
    public class ConnectionStringBuilderFixture {
        [Test]
        public void ConnectionStringBuilding() {
            var builder = new OracleConnectionStringBuilder
                          {
                              Direct = true,
                              Server = "127.0.0.1",
                              Port = 1521,
                              Sid = "xe",
                              UserId = "NSoft",
                              Password = "realweb21",
                              MaxPoolSize = 150,
                              ConnectionTimeout = 30
                          };

            var connStr = builder.ConnectionString;

            Assert.IsNotEmpty(connStr);
            Assert.IsTrue(connStr.Contains("Max Pool Size"));
            Assert.IsTrue(connStr.Contains("User Id=NSoft"));

            Console.WriteLine("ConnectionString=[{0}]", connStr);
        }
    }
}