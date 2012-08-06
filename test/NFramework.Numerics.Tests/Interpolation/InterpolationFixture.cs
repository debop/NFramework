using System.Collections.Generic;
using NSoft.NFramework.Collections;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics {
    [TestFixture]
    public class InterpolationFixture : AbstractNumericTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public const string DashLine = "---------------------------------------------";
        private readonly Dictionary<string, IInterpolator> _interpolators = new Dictionary<string, IInterpolator>();

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _interpolators.Add("Lagrange", new LagrangeInterpolator());
            _interpolators.Add("Linear", new LinearInterpolator());
            _interpolators.Add("Neville", new NevilleInterpolator());
            _interpolators.Add("Newton", new NewtonInterpolator());
            _interpolators.Add("Spline", new SplineInterpolator());
        }

        [Test]
        public void InterpolationTest() {
            double[] x = { 0, 1, 3, 6, 7 };
            double[] y = { 0.8, 3.1, 4.5, 3.9, 2.8 };

            var t = new double[15];

            for(var i = 0; i < 15; i++)
                t[i] = i / 2.0;

            var values = new MultiMap<string, double>();

            foreach(var method in _interpolators.Keys)
                log.Debug("{0} : {1}", method, _interpolators[method].Interpolate(x, y, t).CollectionToString());
        }
    }
}