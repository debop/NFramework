using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class ActionCountdownEventTestCase : ParallelismFixtureBase {
        [Test]
        public void Run_Action_When_Countdown_Is_Zero() {
            var isRunned = false;
            var countdown = new ActionCountdownEvent(0, () => isRunned = true);

            Assert.AreEqual(true, isRunned);
        }

        [Test]
        public void Run_Action_When_Countdown_Completed() {
            const int CountDownNumber = 10;
            var isRunned = false;

            var countdown = new ActionCountdownEvent(CountDownNumber, () => isRunned = true);

            // Countdown이 0으로 떨어지기 전까지는 지정한 action이 수행되지 않지만, 0으로 떨어지면, 지정한 action을 수행합니다.
            for(int i = 0; i < CountDownNumber; i++) {
                Assert.IsFalse(isRunned);
                countdown.Signal();
            }
            Assert.IsTrue(isRunned);
        }
    }
}