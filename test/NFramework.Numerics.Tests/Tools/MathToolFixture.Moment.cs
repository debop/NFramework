using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_Moment : AbstractMathToolFixtureBase {
        [Test]
        public void Moment_Test() {
            var data = LinqTool.Generate(5, x => Generator.GenerateDouble(x, 10));
            var source = data.Select(x => x.Item2);
            const double expected = 2.5;

            double avg, avgDev, variance, skew, kurtosis;
            source.Moment(out avg, out avgDev, out variance, out skew, out kurtosis);

            var actual = variance;

            if(IsDebugEnabled)
                log.Debug("avg=[{0}], avgDev=[{1}], variance=[{2}], skew=[{3}], kurtosis=[{4}]", avg, avgDev, variance, skew, kurtosis);

            Assert.AreEqual(expected, actual, "variance");

            Assert.AreEqual(source.Average(), avg);
            Assert.AreEqual(0, skew);
        }
    }
}