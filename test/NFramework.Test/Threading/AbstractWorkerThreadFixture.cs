using System;
using System.IO;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Threading {
    [TestFixture]
    public class AbstractWorkerThreadFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(2,
                              () => {
                                  CountUpThreadTest();
                                  HanoiThreadTest();
                                  MultiThreadTest();
                              });
        }

        /// <summary>
        /// Counts the up thread test.
        /// </summary>
        [Test]
        public void CountUpThreadTest() {
            if(log.IsInfoEnabled)
                log.Info("main : BEGIN");

            try {
                var thread = new CountUpThread("Counter Thread");

                thread.ThreadStarted += ThreadThreadStarted;
                thread.ThreadFinished += ThreadThreadFinished;
                thread.ShutDownRequested += ThreadShutDownRequested;
                thread.ThreadProgressChanged += ThreadThreadProgressChanged;

                thread.Start();

                ThreadTool.Sleep(5000);

                if(log.IsInfoEnabled)
                    log.Info("main : Call ShutDown() ");

                thread.ShutDown();

                if(log.IsInfoEnabled)
                    log.Info("main : Call Join() ");

                thread.Join();

                Assert.IsFalse(thread.IsAlive);
            }
            catch(ThreadInterruptedException) {
                if(IsDebugEnabled)
                    log.Debug("스레드가 Interrupt되었습니다.");
            }

            if(log.IsInfoEnabled)
                log.Info("main : END");
        }

        /// <summary>
        /// Honois the thread test.
        /// </summary>
        [Test]
        public void HanoiThreadTest() {
            if(log.IsInfoEnabled)
                log.Info("main : BEGIN");

            try {
                var thread = new HanoiThread("Hanoi Thread");

                thread.ThreadStarted += ThreadThreadStarted;
                thread.ThreadFinished += ThreadThreadFinished;
                thread.ShutDownRequested += ThreadShutDownRequested;

                thread.Start();

                ThreadTool.Sleep(5000);

                if(log.IsInfoEnabled)
                    log.Info("main : Call ShutDown() ");

                thread.ShutDown();

                if(log.IsInfoEnabled)
                    log.Info("main : Call Join() ");

                thread.Join();

                Assert.IsFalse(thread.IsAlive);
            }
            catch(ThreadInterruptedException) {
                if(IsDebugEnabled)
                    log.Debug("쓰레트가 중단되었습니다.");
            }

            if(log.IsInfoEnabled)
                log.Info("main : END");
        }

        [Test]
        public void MultiThreadTest() {
            using(new OperationTimer("MultiThreadMixingTest")) {
                CountUpThreadTest();
                HanoiThreadTest();
            }
        }

        private static void ThreadThreadStarted(object sender, WorkerThreadEventArgs args) {
            if(IsDebugEnabled)
                log.Debug(((AbstractThread)sender).Name + " is started.");

            ThreadTool.Sleep(0);
        }

        private static void ThreadThreadFinished(object sender, WorkerThreadEventArgs args) {
            if(IsDebugEnabled)
                log.Debug(((AbstractThread)sender).Name + " is finished.");

            ThreadTool.Sleep(0);
        }

        private static void ThreadShutDownRequested(object sender, WorkerThreadEventArgs args) {
            if(IsDebugEnabled)
                log.Debug(((AbstractThread)sender).Name + " is shutdown....");

            ThreadTool.Sleep(0);
        }

        private static void ThreadThreadProgressChanged(object sender, WorkerProgressChangedEventArgs args) {
            if(IsDebugEnabled)
                log.Debug("Progress: " + args.ProgressPercentage);
        }
    }

    public class HanoiThread : AbstractWorkerThread {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public HanoiThread(string name) : base(name) {}
        public HanoiThread(int maxStackSize) : base(maxStackSize) {}
        public HanoiThread(string name, int maxStackSize) : base(name, maxStackSize) {}

        /// <summary>
        /// AbstractWorkerThread가 기본적으로 void DoWork()를 호출하지만, 
        /// override 해서 DoWork()를 새로 정의할 수 있다.
        /// </summary>
        public override void Run() {
            try {
                int level = 0;
                while(IsShutdownRequested == false) {
                    level++;

                    if(IsDebugEnabled)
                        log.Debug("===== LEVEL : {0} =====", level);

                    DoWork(level, 'A', 'B', 'C');
                }
            }
            catch(ThreadInterruptedException) {
                if(IsDebugEnabled)
                    log.Debug("쓰레드가 중단되었습니다.");
            }
            finally {
                DoShutDown();
            }
        }

        protected override void DoWork() {}

        protected void DoWork(int level, char posA, char posB, char posC) {
            if(level > 0) {
                if(IsShutdownRequested)
                    throw new ThreadInterruptedException();

                DoWork(level - 1, posA, posC, posB);

                if(IsDebugEnabled)
                    log.Debug(posA + " -> " + posB);

                DoWork(level - 1, posC, posB, posA);
            }
        }

        protected override void DoShutDown() {
            // Clean up codes in here
            if(IsDebugEnabled)
                log.Debug("DoShutDown()");
        }
    }

    public class CountUpThread : AbstractWorkerThread {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private long _counter;
        private static readonly object _syncLock = new object();

        public CountUpThread(string name) : base(name) {}
        public CountUpThread(int maxStackSize) : base(maxStackSize) {}
        public CountUpThread(string name, int maxStackSize) : base(name, maxStackSize) {}

        protected override void DoWork() {
            _counter++;

            if(IsDebugEnabled)
                log.Debug("DoWork: counter = " + _counter);

            int progress = Convert.ToInt32(_counter);
            OnProgressChanged(progress);

            if(IsShutdownRequested)
                return;
        }

        protected override void DoShutDown() {
            if(IsDebugEnabled)
                log.Debug("DoShutdown: counter = " + _counter);

            try {
                lock(_syncLock) {
                    using(var writer = new StreamWriter("count.txt")) {
                        writer.Write((int)_counter);
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("DoShutDown() : " + ex.Message, ex);
            }
        }
    }
}