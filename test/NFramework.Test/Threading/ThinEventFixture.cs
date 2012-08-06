using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NSoft.NFramework.Threading {
    [TestFixture]
    public class ThinEventFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        [Test]
        public void WaitTest() {
            var thin = new ThinEvent();

            Task.Factory.StartNew(() => {
                                      Thread.Sleep(1000);
                                      thin.Set();

                                      if(IsDebugEnabled)
                                          log.Debug("Set event...");
                                  });

            if(IsDebugEnabled)
                log.Debug("Wait event set...");

            thin.Wait();

            if(IsDebugEnabled)
                log.Debug("set event is completed...");
        }
    }
}