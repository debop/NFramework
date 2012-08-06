using System.IO;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains {
    /// <summary>
    /// NHibernate Domain Model에 대한 DB Schema를 생성해줍니다.
    /// </summary>
    [TestFixture]
    public class FluentDomainSchemaTestFixture : FluentDomainTestCaseBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void GenerateSchema() {
            if(IsDebugEnabled)
                log.Debug("Generate Schema");

            new SchemaExport(CurrentContext.NHConfiguration)
                .SetDelimiter(";")
                .Create(true, false);
        }

        [Test]
        public void GenerateSchemaToFile() {
            const string filepath = @".\..\..\NFramework.Data.DevartOracle.sql";

            if(IsDebugEnabled)
                log.Debug("Schema를 생성하고, DB를 실제로 생성합니다...");

            new SchemaExport(CurrentContext.NHConfiguration)
                .SetOutputFile(filepath)
                .SetDelimiter(";")
                .Create(true, true);

            if(IsDebugEnabled)
                log.Debug("Schema를 생성하고, DB를 실제로 생성했습니다!!!");

            File.Exists(filepath).Should().Be.True();
            File.Delete(filepath);
        }
    }
}