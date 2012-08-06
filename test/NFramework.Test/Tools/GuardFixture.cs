using System;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class GuardFixture : AbstractFixture {
        [Test]
        public void ShouldNotBeNullTest() {
            Company company = null;
            Assert.Throws<ArgumentNullException>(() => company.ShouldNotBeNull("company"));

            Company company2 = new Company();
            company2.ShouldNotBeNull("company2");
        }
    }
}