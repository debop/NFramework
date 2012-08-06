using System.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.Threading {
    [TestFixture]
    public class SpinWaitFixture : AbstractFixture {
        [Test]
        public void SpinTest() {
            System.Threading.SpinWait spinWait = new SpinWait();
            spinWait.SpinOnce();
        }
    }
}