using System.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.Threading.ComputerBoundAsync {
    [TestFixture]
    public class ThreadPoolCase : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private int minWorkerThreads;
        private int minPortThreads;
        private int maxWorkerThreads;
        private int maxPortThreads;

        [Test]
        public void ChangeThreadsInThreadPool() {
            GetThreadInfo();

            //! ThreadPool의 MinThread는 변경하지 않는 것이 좋다. 
            //
            ThreadPool.SetMinThreads(minWorkerThreads * 5, minPortThreads * 10);
            ThreadPool.SetMaxThreads(maxWorkerThreads * 2, maxPortThreads * 5);

            GetThreadInfo();
        }

        private void GetThreadInfo() {
            ThreadPool.GetMinThreads(out minWorkerThreads, out minPortThreads);
            log.Debug("ThreadPool Min WorkerThreads={0}, Min PortThreads={1}", minWorkerThreads, minPortThreads);


            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxPortThreads);
            log.Debug("ThreadPool Max WorkerThreads={0}, Max PortThreads={1}", maxWorkerThreads, maxPortThreads);
        }
    }
}