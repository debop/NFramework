using System;
using System.IO;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tools {
    [TestFixture]
    public class SQLServerSetUpToolFixture {
        [Test]
        public void CanGetSqlServerDataDirectory() {
            var dataDir = SQLServerSetUpTool.GetSqlServerDataDirectory();
            Assert.IsNotEmpty(dataDir);
            Assert.IsTrue(Directory.Exists(dataDir));
            Console.WriteLine("Data Directory = " + dataDir);
        }
    }
}