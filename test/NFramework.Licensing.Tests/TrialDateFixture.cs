using System.IO;
using NUnit.Framework;

namespace NSoft.NFramework.Licensing {
    [TestFixture]
    public class TrialDateFixture : AbstractLicenseFixture {
        [Test]
        public void LicenseNotFoundExceptionTest() {
            var validator = new LicenseValidator(public_only, Path.GetTempFileName());
            Assert.Throws<LicenseNotFoundException>(() => validator.AssertValidLicense());
        }

        [Test]
        public void LicenseFileNotFoundExceptionTest() {
            var validator = new LicenseValidator(public_only, "not_there");
            Assert.Throws<LicenseFileNotFoundException>(() => validator.AssertValidLicense());
        }
    }
}