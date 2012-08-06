using NUnit.Framework;

namespace NSoft.NFramework.WindowsSystem {
    [TestFixture]
    public class RegistryClientFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string TestSubKey = @"SOFTWARE\NSoft\NFramework\Test";
        private const int MaxSize = 100;

        private static RegistryClient GetLocalMachineRegistry() {
            return new RegistryClient(Microsoft.Win32.Registry.LocalMachine);
        }

        //[TestFixtureTearDown]
        //public void ClassTearDown()
        //{
        //    using (var reg = GetLocalMachineRegistry())
        //    {
        //        reg.DeleteSubKey(TestSubKey);
        //    }
        //}

        [Test]
        public void ReadWrite() {
            int readSize = 0;

            using(var reg = GetLocalMachineRegistry()) {
                Assert.IsNotNull(reg);
                reg.SubKeyName = TestSubKey;
                reg.SetValue("MaxSize", MaxSize);
            }

            using(var reg = GetLocalMachineRegistry()) {
                Assert.IsNotNull(reg);
                reg.SubKeyName = TestSubKey;
                readSize = reg.GetValueDef("MaxSize", 0);
            }

            Assert.AreEqual(MaxSize, readSize);
        }
    }
}