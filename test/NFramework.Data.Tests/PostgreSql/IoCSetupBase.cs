using System;
using System.Diagnostics;
using System.Threading;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.Data.PostgreSql {
    public abstract class IoCSetupBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [TestFixtureSetUp]
        public void ClassSetUp() {
            IoC.Initialize();
        }

        [TestFixtureTearDown]
        public void ClassTearDown() {
            if(IoC.IsInitialized)
                IoC.Reset();
        }

        private static void Prepare() {
            CollectGarbage();
            IncreaseThreadAndProcessPriority();
        }

        private static void CollectGarbage() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForFullGCComplete();
        }

        private static void IncreaseThreadAndProcessPriority() {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
        }
    }
}