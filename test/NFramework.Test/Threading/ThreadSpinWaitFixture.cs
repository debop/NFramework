using System.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.Threading {
    [TestFixture]
    public class ThreadSpinWaitFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void ThreadInterruptTest() {
            var stayAwake = new StayAwake();
            var newThread = new Thread(stayAwake.ThreadMethod);
            newThread.Start();

            Thread.Sleep(1000);

            // The following line causes an exception to be thrown 
            // in ThreadMethod if newThread is currently blocked
            // or becomes blocked in the future.
            newThread.Interrupt();

            if(log.IsInfoEnabled)
                log.Info("2. Main thread calls Interrupt on newThread.");

            // Tell newThread to go to sleep.
            stayAwake.SleepSwitch = true;

            // Wait for newThread to end.
            newThread.Join();
            Assert.IsFalse(newThread.IsAlive);
        }
    }

    internal class StayAwake {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private bool _sleepSwitch;

        public bool SleepSwitch {
            set {
                lock(this)
                    _sleepSwitch = value;
            }
        }

        public void ThreadMethod() {
            if(log.IsInfoEnabled)
                log.Info("1. newThread is executing ThreadMethod.");

            while(_sleepSwitch == false) {
                // SpinWait는 Thread 상태를 대기 상태로 만들어서
                // Interrupt가 호출되면 바로 Thread를 다시 살린 후
                // ThreadInterruptException을 발생시킨다.
                Thread.SpinWait(10000);
            }
            try {
                if(log.IsInfoEnabled)
                    log.Info("3. newThread going to sleep.");

                // When newThread goes to sleep, it is immediately 
                // woken up by a ThreadInterruptedException.
                Thread.Sleep(Timeout.Infinite);
            }
            catch(ThreadInterruptedException) {
                if(log.IsInfoEnabled)
                    log.Info("4. newThread cannot go to sleep - " +
                             "interrupted by main thread.");
            }
        }
    }
}