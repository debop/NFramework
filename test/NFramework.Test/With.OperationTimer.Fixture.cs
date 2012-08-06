using System;
using NSoft.NFramework.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework {
    /// <summary>
    /// 성능 측정을 위한 Helper class 
    /// </summary>
    [TestFixture]
    public class WithOperationTimerFixture {
        [Test]
        public void CallsDelegateAndReturnNonZeroTiming() {
            bool called = false;
            double msecs = With.OperationTimer(delegate {
                                                   called = true;
                                                   ThreadTool.Sleep(40);
                                               });

            // The only reliable way of testing this that I can think of.
            Assert.IsTrue(msecs > 0);
            Assert.IsTrue(called);

            Console.WriteLine("Operation elapsed time (msec): " + msecs);
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(5, CallsDelegateAndReturnNonZeroTiming);
        }
    }
}