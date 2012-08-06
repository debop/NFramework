using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics {
    [TestFixture]
    public class MathExFixture : AbstractNumericTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void Ranking() {
            double[] x = { 40, 50, 100, 30, 20, 80, 70, 40 };
            var r = x.Ranking();

            log.Debug("점수    순위");
            for(int i = 0; i < x.Length; i++)
                log.Debug("{0}  :  {1}", x[i], r[i]);
        }

        [Test]
        public void DiceProbabilityTest() {
            int success = 0;

            var tryCount = 60000;

            var rnd = new UniformRandomizer(new Interval<double>(1.0, 7.0, IntervalKind.ClosedOpen));
            //(1.0, 6.9999999);

            Parallel.For(1,
                         tryCount,
                         k => {
                             var m = new int[6];
                             var check = 1;

                             for(var i = 0; i < 13; i++) {
                                 int result = (int)rnd.Next() - 1;
                                 m[result]++;
                             }

                             for(var i = 0; i < 6; i++)
                                 if(m[i] == 0) {
                                     check = 0;
                                     break;
                                 }

                             if(check == 1)
                                 Interlocked.Increment(ref success);
                         });

            var prob = (success / (double)tryCount) * 100.0;

            if(IsDebugEnabled)
                log.Debug("Success probability = {0}", prob);
        }

        [Test]
        public void CorrelationCoefficient() {
            const int N = 10000;
            var x = new double[N];
            var y = new double[N];

            var logistics = new LogisticsRandomizer();
            var weibull = new WeibullRandomizer(0.5);

            logistics.Fill(x, 5, 2);
            weibull.Fill(y, 5, 2);

            if(IsDebugEnabled)
                log.Debug("CorrelationCoefficient       = " + x.CorrelationCoefficient(y));
        }
    }
}