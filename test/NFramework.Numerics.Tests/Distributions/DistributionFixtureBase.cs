using System;
using NSoft.NFramework.Numerics.Statistics;

namespace NSoft.NFramework.Numerics.Distributions {
    public abstract class DistributionFixtureBase : AbstractNumericTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        // NOTE: _numbers 를 사용하게 되면, Parallel이 불가능하기 때문에, numbers를 인자로 받고, Thread-Safe 하게 사용할 수 있도록 했다.
        //
        protected static void DisplayDistribution(string name, Histogram histogram) {
            if(log.IsDebugEnabled) {
                log.Debug(@"-------------------------------------------");
                log.Debug(@"{0} Histogram:{1}{2}", name, Environment.NewLine, histogram.StemLeaf(50));

                log.Debug(@"-------------------------------------------");
                log.Debug(string.Empty);
            }
        }
    }
}