using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics {
    [TestFixture]
    public class MinimumFinderFixture : AbstractNumericTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly Dictionary<string, IMinimumFinder> _minimumFinders = new Dictionary<string, IMinimumFinder>();

        private readonly List<Func<double, double>> _funcs = new List<Func<double, double>>();

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _minimumFinders.Add("GoldenSection", new GoldenSectionMinimumFinder());

            _funcs.Add(F);
            _funcs.Add(F2);
        }

        private static double F(double x) {
            return (x - 0.2) * (x + 500.0);
        }

        private static double F2(double x) {
            return (x - 6.0) * (x + 100.0) * (x - 255.9);
        }

        private static Func<double, double> F3 = x => x;

        [Test]
        public void FindMiminumTest() {
            const double lower = -10.0;
            const double upper = 10.0;

            foreach(var f in _funcs)
                foreach(var method in _minimumFinders.Keys) {
                    // Console.WriteLine();
                    var x = _minimumFinders[method].FindMiminum(f, lower, upper);

                    if(IsDebugEnabled)
                        log.Debug(@"{0} : {1}", method, x);

                    Assert.AreNotEqual(double.NaN, x);
                }
        }

        [Test]
        public void GoldenSectionMinimumFinderTest() {
            const double lower = -500;
            const double upper = 0;

            Func<double, double> f = F;

            var golden = new GoldenSectionMinimumFinder();

            var x = golden.FindMiminum(f, lower, upper);

            if(IsDebugEnabled)
                log.Debug(@"{0} : {1}", golden, x);

            Assert.AreNotEqual(double.NaN, x);

            if(IsDebugEnabled)
                log.Debug("func({0}) = {1}", x, f(x));
        }
    }
}