using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics {
    [TestFixture]
    public class RootFinderFixture : AbstractNumericTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string DashLine = @"---------------------------------------------";
        private readonly Dictionary<string, IRootFinder> _rootFinders = new Dictionary<string, IRootFinder>();

        private readonly List<Func<double, double>> _funcs = new List<Func<double, double>>();

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _rootFinders.Add("Bisection", new BisectionRootFinder());
            _rootFinders.Add("NewtonRapson", new NewtonRapsonRootFinder());
            _rootFinders.Add("Secant", new SecantRootFinder());

            _funcs.Add(F);
            _funcs.Add(F2);
        }

        private static double F(double x) {
            return (x - 0.2) * (x + 500.0);
        }

        private static double F2(double x) {
            return (x - 6.0) * (x + 100.0) * (x - 255.9);
        }

        [Test]
        public void RootFind() {
            const double lower = -10.0;
            const double upper = 10.0;

            foreach(var f in _funcs)
                foreach(var method in _rootFinders.Keys) {
                    // Console.WriteLine();
                    var x = _rootFinders[method].FindRoot(f, lower, upper, 999999);

                    if(IsDebugEnabled)
                        log.Debug(@"{0} : {1}", method, x);

                    if(Equals(x, double.NaN) == false) {
                        if(IsDebugEnabled)
                            log.Debug("func({0}) = {1}", x, f(x));

                        Assert.AreEqual(0.0, f(x).Clamp(0.0, 1.0e-5));
                    }
                }
        }

        [Test]
        public void BisectionRootFindTest() {
            const double lower = 0;
            const double upper = 50.0;

            Func<double, double> f = F2;

            var bisection = new BisectionRootFinder();

            var x = bisection.FindRoot(f, lower, upper, 999999);


            if(IsDebugEnabled)
                log.Debug(@"{0} : {1}", bisection, x);

            if(Equals(x, double.NaN) == false) {
                if(IsDebugEnabled)
                    log.Debug("func({0}) = {1}", x, f(x));

                Assert.AreEqual(0.0, f(x).Clamp(0.0, 1.0e-5));
            }
        }
    }
}