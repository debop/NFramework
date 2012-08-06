using System;
using System.Collections.Generic;
using NSoft.NFramework.Numerics.Integration;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics {
    [TestFixture]
    public class IntegrationFixture : AbstractNumericTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string DashLine = "---------------------------------------------";
        private readonly Dictionary<string, IIntegrator> _integrators = new Dictionary<string, IIntegrator>();

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _integrators.Add("Romberg", new RombergIntegrator());
            _integrators.Add("Simpson", new SimpsonIntegrator());
            _integrators.Add("Trapizoidal", new TrapizoidalIntegrator());
            _integrators.Add("MidValue", new MidValueIntegrator());
        }

        public static double F(double x) {
            return -x * x + 9.0d;
        }

        public static double F2(double x) {
            return Math.Sinh(x) / Math.Log(x * 100);
        }

        [Test]
        public void IntegrationMethodsTest() {
            for(var i = 0; i < 10; i++) {
                double a = i;
                double b = i + 2 * a + 2;

                if(IsDebugEnabled) {
                    log.Debug(DashLine);
                    log.Debug("Integration Test f(x) [{0},{1}]", a, b);
                    log.Debug("f(x) = sqrt(9-x*x)");
                    log.Debug(DashLine);
                }
                foreach(string method in _integrators.Keys) {
                    var result = _integrators[method].Integrate(F, a, b);
                    if(IsDebugEnabled)
                        log.Debug("{0} Integrator = {1}", method, result);

                    Assert.AreNotEqual(double.NaN, result);
                }

                if(IsDebugEnabled)
                    log.Debug(DashLine);
            }
        }

        [Test]
        public void IntegrationMethodsTest2() {
            for(var i = 0; i < 10; i++) {
                double a = i;
                double b = i + 2 * a + 2;

                if(IsDebugEnabled) {
                    log.Debug(DashLine);
                    log.Debug("Integration Test f(x) [{0},{1}]", a, b);
                    log.Debug("f(x) = sqrt(9-x*x)");
                    log.Debug(DashLine);
                }
                foreach(string method in _integrators.Keys) {
                    var result = _integrators[method].Integrate(F2, a, b);
                    if(IsDebugEnabled)
                        log.Debug("{0} Integrator = {1}", method, result);

                    Assert.AreNotEqual(double.NaN, result);
                }

                if(IsDebugEnabled)
                    log.Debug(DashLine);
            }
        }

        [Test]
        public void IntegrationMethodsTest3() {
            Func<double, double> f1 = x => x + 1.0;
            Func<double, double> f2 = x => x + 2.0 * f1(x) + 2.0;

            for(var i = 0; i < 10; i++) {
                //double a = (double)i + 1.0;
                //double b = (double)i + 2.0 * a + 2.0;
                double a = f1(i);
                double b = f2(i);

                if(IsDebugEnabled) {
                    log.Debug(DashLine);
                    log.Debug("Integration Test f(x)[{0},{1}]", a, b);
                    log.Debug("f(x) = Math.Sinh(x) / Math.Log(x * 100)");
                    log.Debug(DashLine);
                }

                foreach(string method in _integrators.Keys) {
                    var result = _integrators[method].Integrate(F2, a, b);

                    if(IsDebugEnabled)
                        log.Debug("{0} Integrator=[{1}]", method, result);

                    Assert.AreNotEqual(double.NaN, result);
                }

                if(IsDebugEnabled)
                    log.Debug(DashLine);
            }
        }
    }
}