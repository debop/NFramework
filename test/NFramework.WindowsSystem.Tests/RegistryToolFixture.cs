using NUnit.Framework;

namespace NSoft.NFramework.WindowsSystem {
    [TestFixture]
    public class RegistryToolFixture {
        [TestCase(@"SOFTWARE\Microsoft\Windows\CurrentVersion", "SM_AccessoriesName", "Accessories")]
        [TestCase(@"SOFTWARE\NSoft\NFramework\Test", "MaxSize", "100")]
        public void Can_GetValue(string subKey, string name, string expectedValue) {
            var retrievedValue = RegistryTool.GetLocalMachineValue(subKey, name);
            Assert.AreEqual(expectedValue, retrievedValue);
        }

        [TestCase(@"SOFTWARE\NSoft\NFramework\Test", "MaxSize2", "200")]
        public void Can_GetValueNotExists(string subKey, string name, string expectedValue) {
            var retrievedValue = RegistryTool.GetLocalMachineValue(subKey, name);
            Assert.IsNull(retrievedValue);
        }

        [Ignore("64bit 로 컴파일해야 제대로 실행됩니다.")]
        [TestCase(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", "MSSQLSERVER", "MSSQL.1")]
        public void Can_GetValue_By_Registry64(string subKey, string name, string expectedValue) {
            var retrievedValue = Registry64.LocalMachine.GetValue(subKey, name);
            Assert.AreEqual(expectedValue, retrievedValue);
        }
    }
}