using System.DirectoryServices;
using NUnit.Framework;

namespace NSoft.NFramework.WindowsSystem {
    [TestFixture]
    public class AdServiceToolFixture {
        private const string ActiveDirectorySchema = @"LDAP://";
        // private const string NT_PREFIX = "WinNT://";

        private const string Host = "localhost"; //"10.10.11.47";

        /// <summary>
        /// Ad Server에 등록된 사용자인지 검사한다.
        /// </summary>
        [Test]
        public void VerifyUser() {
            var adPath = ActiveDirectorySchema + Host;

            Assert.IsTrue(AdServiceTool.Authenticate(adPath, "administrator", "real21"));

            Assert.IsFalse(AdServiceTool.Authenticate(adPath, "xxxx", "1111", AuthenticationTypes.Secure));
        }
    }
}