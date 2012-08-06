using System.Collections.Concurrent;
using NSoft.NFramework.Numerics.Statistics;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics {
    [TestFixture]
    public class RandomizerFixture : AbstractNumericTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const int RAND_NUMBER = 99999;

        private readonly ConcurrentDictionary<string, IRandomizer> _dblRandomizers = new ConcurrentDictionary<string, IRandomizer>();
        private readonly ConcurrentDictionary<string, IRandomizer> _intRandomizers = new ConcurrentDictionary<string, IRandomizer>();

        private double[] _numbers;
        private Histogram _dblHistogram;
        private Histogram _intHistogram;

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _dblRandomizers.GetOrAdd("Beta(2, 4)", new BetaRandomizer(2, 4));
            _dblRandomizers.GetOrAdd("Uniform(0, 1)", new UniformRandomizer(0, 1));
            _dblRandomizers.GetOrAdd("Triangular(0, 1, 0.5)", new TriangularRandomizer(0, 1, 0.5));
            _dblRandomizers.GetOrAdd("Normal(0.5, 0.15)", new NormalRandomizer(0.5, 0.15));
            _dblRandomizers.GetOrAdd("LogNormal(0.5, 1.5)", new LogNormalRandomizer(0.5, 1.5));
            _dblRandomizers.GetOrAdd("Exponential(4)", new ExponentialRandomizer(4));
            _dblRandomizers.GetOrAdd("Exponential(2)", new ExponentialRandomizer(2));
            _dblRandomizers.GetOrAdd("Exponential(1)", new ExponentialRandomizer(1));
            _dblRandomizers.GetOrAdd("Exponential(0.5)", new ExponentialRandomizer(0.5));

            for(int i = 0; i <= 20; i += 5)
                _dblRandomizers.GetOrAdd("Weibull(" + i + ")", new WeibullRandomizer(i));

            // INT 형
            //

            _intRandomizers.GetOrAdd("Binominal(50, 0.5)", new BinominalRandomizer(50, 0.5));
            _intRandomizers.GetOrAdd("Binominal(50, 0.1)", new BinominalRandomizer(50, 0.1));
            _intRandomizers.GetOrAdd("Poisson", new PoissonRandomizer(5));
            _intRandomizers.GetOrAdd("Pareto(1)", new ParetoRandomizer(1));
            _intRandomizers.GetOrAdd("Pareto(0.5)", new ParetoRandomizer(0.5));

            _numbers = new double[RAND_NUMBER];
            _dblHistogram = new Histogram(50, 0, 2);
            _intHistogram = new Histogram(50, 0, 50);
        }

        // NOTE: _numbers 를 사용하게 되면, Parallel이 불가능하기 때문에, numbers를 인자로 받고, Thread-Safe 하게 사용할 수 있도록 했다.
        //
        private static void DisplayDistribution(string name, Histogram histogram, double[] numbers) {
            log.Debug(@"-------------------------------------------");
            histogram.ResetData();
            histogram.AddData(numbers);
            log.Debug(@"{0} Histogram:\r\n{1}", name, histogram.StemLeaf(50));

            double avg, stdev;
            numbers.AverageAndStDev(out avg, out stdev);
            log.Debug(@"Avg = {0} ; StDev = {1}", avg, stdev);
            log.Debug(@"-------------------------------------------");
            log.Debug(string.Empty);
        }

        [Test]
        public void DoubleRandomNumber() {
            foreach(var method in _dblRandomizers.Keys) {
                _dblRandomizers[method].FillValues(_numbers, RAND_NUMBER);
                DisplayDistribution(method, _dblHistogram, _numbers);
            }
        }

        [Test]
        public void IntRandomNumber() {
            foreach(var method in _intRandomizers.Keys) {
                _intRandomizers[method].FillValues(_numbers, RAND_NUMBER);
                DisplayDistribution(method, _intHistogram, _numbers);
            }
        }
    }
}