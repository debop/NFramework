using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture : AbstractMathToolFixtureBase {
        #region << IsPrimes >>

        [Test]
        [TestCase(2)]
        [TestCase(7)]
        [TestCase(23)]
        [TestCase(12, Description = "Not Primes", ExpectedException = typeof(AssertionException))]
        public void IsPrimesTest(long value) {
            if(value.IsPrimes() == false)
                throw new AssertionException("Not Primes");
            // Assert.IsTrue(value.IsPrimes());
        }

        #endregion

        #region << CubeRoot >>

        [Test]
        [TestCase(8, 2)]
        [TestCase(64, 4)]
        public void CubeRootTest(double x, double cubeRoot) {
            Assert.AreEqual(cubeRoot, x.CubeRoot());
        }

        #endregion

        #region << Norm >>

        [Test]
        public void Norm_Double() {
            var list1 = new double[] { 1, 1, 1, 1, 1 };
            var list2 = new double[] { 2, 2, 2, 2, 2 };

            Console.WriteLine("{0}.Norm() is {1}", list1.CollectionToString(), list1.Norm());

            Assert.AreEqual(list1.Count() * list1[0].Square(), list1.Norm());
            Assert.AreEqual(list2.Count() * list2[0].Square(), list2.Norm());
        }

        [Test]
        public void Norm_NullableDouble() {
            var list1 = new double?[] { 1, null, 1, null, 1, null, 1, null };
            var list2 = new double?[] { 2, null, 2, null, 2, null, 2, null };

            Assert.AreEqual(list1.Count() / 2 * list1[0].Square(), list1.Norm());
            Assert.AreEqual(list2.Count() / 2 * list2[0].Square(), list2.Norm());
        }

        [Test]
        public void Norm_Int() {
            var ints1 = new[] { 1, 1, 1, 1, 1 };
            var ints2 = new[] { 2, 2, 2, 2, 2 };

            var list1 = ints1.RunEachAsync(x => (double)x).ToList();
            // var list2 = ints2.ConvertUnsafe<double>(x=>double.Parse(x.ToString())).ToList();
            var list2 = ints2.ConvertUnsafe<double>().ToList();

            Assert.AreEqual(5, list1.Count);
            Assert.AreEqual(5, list2.Count);

            Console.WriteLine("{0}.Norm() is {1}", list1.CollectionToString(), list1.Norm());
            Console.WriteLine("{0}.Norm() is {1}", list2.CollectionToString(), list2.Norm());

            Assert.AreEqual(list1.Count() * list1[0].Square(), list1.Norm());
            Assert.AreEqual(list2.Count() * list2[0].Square(), list2.Norm());
        }

        [Test]
        public void Normailized() {
            // 짝수이어야 합니다.
            const int NumberCount = 1000;

            var numbers = Enumerable.Range(1, NumberCount);

            var sum = (double)numbers.Sum();
            Assert.AreEqual((1 + NumberCount) * (NumberCount / 2), sum);

            Assert.AreEqual(1.0, numbers.Normalize().Sum());

            var normalized = numbers.Normalize().ToArray();

            for(var i = 0; i < normalized.Length; i++)
                Assert.AreEqual((i + 1) / sum, normalized[i]);
        }

        #endregion

        #region << Normalize >>

        [Test]
        [TestCase(1.0, 9999)]
        [TestCase(2.0, 9999)]
        public void Normailize_Double(double value, int count) {
            var list = ParallelEnumerable.Repeat(value, count).ToList();
            var norms = list.Normalize().ToList();

            var norm = count * value;

            // NOTE: Parallel.For 구문을 사용하려면, 내부 Action에 Enumerable 이 아닌 Collection 또는 List이어야한다.
            Parallel.For(0, count,
                         i => {
                             Assert.AreEqual(list[i], value);
                             Assert.AreEqual(list[i] / norm, norms[i]);
                         });
        }

        [Test]
        [TestCase(1, 9999)]
        [TestCase(2, 9999)]
        public void Normailize_Int(int value, int count) {
            var list = ParallelEnumerable.Repeat(value, count).ToList();
            var norms = list.Normalize().ToList();

            var norm = count * value;

            // NOTE: Parallel.For 구문을 사용하려면, 내부 Action에 Enumerable 이 아닌 Collection 또는 List이어야한다.
            Parallel.For(0, count,
                         i => {
                             Assert.AreEqual(list[i], value);
                             Assert.AreEqual((double)list[i] / norm, norms[i]);
                         });
        }

        #endregion

        #region << Hypot >>

        [Test]
        [TestCase(3.0, 4.0)]
        [TestCase(6.0, 8.0)]
        public void HypotTest(double a, double b) {
            Assert.AreEqual(a.Hypot(b), b.Hypot(a), 0.00000000001d);
            Assert.AreEqual(a.Hypot2(b), b.Hypot2(a), 0.00000000001d);

            Assert.AreEqual(a.Hypot(b), a.Hypot2(b), 0.00000000001d);
        }

        #endregion

        #region << Distance >>

        [Test]
        [TestCase(0, 0, 3.0, 4.0)]
        [TestCase(0, 0, 6.0, 8.0)]
        public void DistanceTest(double x1, double y1, double x2, double y2) {
            Assert.AreEqual(x2.Hypot2(y2), MathTool.Distance(x1, y1, x2, y2), 0.000000001d);

            var p1 = new PointF((float)x1, (float)y1);
            var p2 = new PointF((float)x2, (float)y2);

            Assert.AreEqual(x2.Hypot2(y2), p1.Distance(p2), 0.000000001d);
        }

        #endregion

        #region << Abs >>

        [Test]
        public void Abs_Double() {
            var serial = GetSerialList<double>(-10, 10, 20);
            var abs = serial.Abs();

            Assert.IsTrue(abs.All(x => x >= 0.0));
            Assert.IsFalse(abs.Any(x => x < 0.0));
        }

        #endregion

        #region << Sum (Long, Abs, Square) >>

        [TestCase(1, 99999)]
        [TestCase(2, 99999)]
        public void LongSum(int v, int count) {
            var list = Enumerable.Repeat(v, count).ToList();
            Assert.AreEqual((long)v * count, list.LongSum());
            Assert.AreEqual((long)v * count, list.LongSum());
            Assert.AreEqual((long)v * count, list.LongSum());
        }

        [TestCase(1, 99999)]
        [TestCase(2, 99999)]
        public void LongSumParallel(int v, int count) {
            var list = Enumerable.Repeat(v, count).ToList();
            Assert.AreEqual((long)v * count, list.LongSumParallel());
            Assert.AreEqual((long)v * count, list.LongSumParallel());
            Assert.AreEqual((long)v * count, list.LongSumParallel());
        }

        [TestCase(-1.0, 9999)]
        [TestCase(-2.0, 9999)]
        public void AbsSum_Double(double v, int count) {
            var list = Enumerable.Repeat(v, count);
            Assert.AreEqual(v * count, -list.AbsSum());
        }

        [Test]
        [TestCase(1.0, 9999)]
        [TestCase(2.0, 9999)]
        public void SumOfSquares_Double(double v, int count) {
            var list = Enumerable.Repeat(v, count);
            Assert.AreEqual(v.Square() * count, list.SumOfSquares());
        }

        #endregion

        #region << ApproximateEqual, Approximate >>

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.001, ExpectedException = typeof(AssertionException))]
        public void ApproximateEqual(double a, double b) {
            Assert.IsTrue(a.ApproximateEqual(b));
        }

        [Test]
        [TestCase(0.01, 0.01, 0.0001)]
        [TestCase(0.002, 0.0021, 0.0002)]
        [TestCase(0.1, 0.11, 0.0001, ExpectedException = typeof(AssertionException))]
        public void ApproximateFiltering(double value, double check, double tolerance) {
            var list = Enumerable.Repeat(value, 100);
            Assert.IsTrue(list.Approximate(check, tolerance).ItemExists());
        }

        #endregion

        #region << Clamp >>

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.001, ExpectedException = typeof(AssertionException))]
        public void Clamp(double src, double dest) {
            Assert.IsTrue(Math.Abs(src.Clamp(dest) - dest) < double.Epsilon);
        }

        [Test]
        [TestCase(0.0, 0.0, 0.000001)]
        [TestCase(0.0, 1.0, 1.0)]
        [TestCase(0.0, 0.001, 0.0000001, ExpectedException = typeof(AssertionException))]
        public void Clamp(double src, double dest, double tolerance) {
            Assert.AreEqual(src.Clamp(dest, tolerance), dest);
        }

        [TestCase(1.0, 1.1, 0.1000001)]
        [TestCase(1.0, 1.1, 0.01, ExpectedException = typeof(AssertionException))]
        public void ClampList(double value, double dest, double tolerance) {
            var list = Enumerable.Repeat(value, 100);
            var clamp = list.Clamp(dest, tolerance);

            Assert.IsTrue(clamp.All(x => Math.Abs(x - dest) < double.Epsilon));
        }

        #endregion

        #region << RangeClamp >>

        /// <summary>
        /// 값이 상하한을 벗어나면 상하한중 가까운 값으로 대체된다.
        /// </summary>
        [Test]
        [TestCase(0, -1, 1)]
        [TestCase(0, 10, 100, ExpectedException = typeof(AssertionException))]
        public void RangeClamp(double value, double min, double max) {
            Assert.AreEqual(value, value.RangeClamp(min, max));
        }

        [Test]
        [TestCase(0.0, 1.0, 100, -50.0, 50.0)]
        [TestCase(100, 100, 100, 0, 1000)]
        public void RangeClampList(double seed, double step, int count, double min, double max) {
            var serial = GetSerialList(seed, step, count);
            var clamp = serial.RangeClamp(min, max);

            Assert.AreEqual(0, clamp.Count(x => x < min));
            Assert.AreEqual(0, clamp.Count(x => x > max));

            Assert.IsTrue(clamp.ItemExists(x => x == max));
        }

        #endregion

        #region << Min , Max >>

        [Test]
        [TestCase(-100, 100)]
        [TestCase(-200, 200)]
        public void GetMinMax_Simple(double start, double end) {
            var list = GetSerialList<double>(0, 1, 100).ToList();
            list.Add(start);
            list.Add(end);

            double min, max;
            list.GetMinMax(out min, out max);

            Assert.AreEqual(start, min);
            Assert.AreEqual(end, max);
        }

        [Test]
        [TestCase(-100, 100)]
        [TestCase(-200, 200)]
        public void GetMinMax_Simple(float start, float end) {
            var list = GetSerialList<float>(0, 1, 100).ToList();
            list.Add(start);
            list.Add(end);

            float min, max;
            list.GetMinMax(out min, out max);

            Assert.AreEqual(start, min);
            Assert.AreEqual(end, max);
        }

        [Test]
        [TestCase(-100, 100)]
        [TestCase(-200, 200)]
        public void GetMinMax_Simple(decimal start, decimal end) {
            var list = GetSerialList<decimal>(0, 1, 100).ToList();
            list.Add(start);
            list.Add(end);

            decimal min, max;
            list.GetMinMax(out min, out max);

            Assert.AreEqual(start, min);
            Assert.AreEqual(end, max);
        }

        [Test]
        [TestCase(-100, 100)]
        [TestCase(-200, 200)]
        public void GetAbsMinMax_Simple(double start, double end) {
            var list = GetSerialList<double>(0, 1, 100).ToList();
            list.Add(start);
            list.Add(end);

            double min, max;
            list.GetAbsMinMax(out min, out max);

            Assert.AreEqual(0, min);
            Assert.AreEqual(end, max);
        }

        [Test]
        [TestCase(-100, 100)]
        [TestCase(-200, 200)]
        public void GetAbsMinMax_Simple(float start, float end) {
            var list = GetSerialList<float>(0, 1, 100).ToList();
            list.Add(start);
            list.Add(end);

            float min, max;
            list.GetAbsMinMax(out min, out max);

            Assert.AreEqual(0, min);
            Assert.AreEqual(end, max);
        }

        [Test]
        [TestCase(-100, 100)]
        [TestCase(-200, 200)]
        public void GetAbsMinMax_Simple(decimal start, decimal end) {
            var list = GetSerialList<decimal>(0, 1, 100).ToList();
            list.Add(start);
            list.Add(end);

            decimal min, max;
            list.GetAbsMinMax(out min, out max);

            Assert.AreEqual(0, min);
            Assert.AreEqual(end, max);
        }

        #endregion

        #region << NormalDensity >>

        [Test]
        [TestCase(0, 0, 1, 0.1)]
        public void NormalDensity(double x, double avg, double stdev, double expectedDensity) {
            Assert.Greater(x.NormalDensity(avg, stdev), expectedDensity);
        }

        [Test]
        [TestCase(0, 1, 0.1)]
        public void NormalDensity(double avg, double stdev, double expectedDensity) {
            var list = GetSerialList(-0.5, 0.5, 10);
            var desities = list.NormalDensity(avg, stdev);
            Assert.IsTrue(desities.Any(d => d > expectedDensity));
        }

        #endregion

        #region << Combination >>

        [Test]
        [TestCase(5, 1)]
        [TestCase(5, 2)]
        [TestCase(5, 3)]
        public void Combination_Test(int n, int k) {
            if(k == 0 || n == k)
                Assert.AreEqual(1, MathTool.Combinations(n, k));

            int c = MathTool.Combinations(n, k).AsInt();

            Console.WriteLine("Combination ({0},{1}) = {2}", n, k, c);
        }

        [Test]
        [TestCase(5, 1)]
        [TestCase(5, 2)]
        [TestCase(5, 3)]
        public void LongCombination_Test(int n, int k) {
            if(k == 0 || n == k)
                Assert.AreEqual(1, MathTool.Combinations(n, k));

            long c = MathTool.Combinations(n, k).AsLong();

            Console.WriteLine("Combination ({0},{1}) = {2}", n, k, c);
        }

        #endregion

        #region << Ranking >>

        [Test]
        public void Ranking_Double() {
            double[] x = { 40, 50, 100, 30, 20, 80, 70, 40 };
            var r = x.Ranking();

            Console.WriteLine("점수    순위");
            for(int i = 0; i < x.Length; i++)
                Console.WriteLine("{0}  :  {1}", x[i], r[i]);

            AssertRanking(r);
        }

        [Test]
        public void Ranking_Float() {
            float[] x = { 40, 50, 100, 30, 20, 80, 70, 40 };
            var r = x.Ranking();

            Console.WriteLine("점수    순위");
            for(int i = 0; i < x.Length; i++)
                Console.WriteLine("{0}  :  {1}", x[i], r[i]);

            AssertRanking(r);
        }

        [Test]
        public void Ranking_Int() {
            int[] x = { 40, 50, 100, 30, 20, 80, 70, 40 };
            var r = x.Ranking();

            Console.WriteLine("점수    순위");
            for(int i = 0; i < x.Length; i++)
                Console.WriteLine("{0}  :  {1}", x[i], r[i]);

            AssertRanking(r);
        }

        [Test]
        public void Ranking_Decimal() {
            decimal[] x = { 40, 50, 100, 30, 20, 80, 70, 40 };
            var r = x.Ranking();

            Console.WriteLine("점수    순위");
            for(int i = 0; i < x.Length; i++)
                Console.WriteLine("{0}  :  {1}", x[i], r[i]);

            AssertRanking(r);
        }

        private static void AssertRanking(int[] rank) {
            Assert.AreEqual(1, rank[2]);
            Assert.AreEqual(8, rank[4]);
        }

        #endregion

        #region << StDev >>

        [Test]
        [TestCase(-10, 0.1, 999)]
        [TestCase(0, 0.1, 999)]
        [TestCase(0, 0.1, 9999)]
        public void StDev_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).ToList();

            var variance = source.Variance();
            var stdev = source.StDev();

            if(IsDebugEnabled)
                log.Debug("Variance=[{0}], StDev=[{1}]", variance, stdev);

            stdev.Square().ApproximateEqual(variance, 0.01).Should().Be.True();
        }

        [Test]
        [TestCase(-10, 0.1, 999)]
        [TestCase(0, 0.1, 999)]
        [TestCase(0, 0.1, 9999)]
        public void StDev_NullableTest(double seed, double step, int count) {
            var list = GetSerialList(seed, step, count);

            var source = list.Select(x => (double?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var variance = source.Variance().GetValueOrDefault();
            var stdev = source.StDev().GetValueOrDefault();

            if(IsDebugEnabled)
                log.Debug("Variance=[{0}], StDev=[{1}]", variance, stdev);

            stdev.Square().ApproximateEqual(variance, 0.01).Should().Be.True();
        }

        #endregion

        #region << AverageAndStDev >>

        [TestCase(-10, 0.1, 5000)]
        [TestCase(0, 0.1, 5000)]
        [TestCase(0, 0.1, 50000)]
        public void AverageAndStDev_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count);

            var variance = source.Variance();

            double average, stdev;
            source.AverageAndStDev(out average, out stdev);

            if(IsDebugEnabled)
                log.Debug("Variance=[{0}], Sqrt(v)=[{3}], StDev=[{1}], Average=[{2}]", variance, stdev, average, Math.Sqrt(variance));

            Math.Sqrt(variance).ApproximateEqual(stdev, 1.0e-10).Should().Be.True();
            average.ApproximateEqual(source.Average(), 1e-10).Should().Be.True();
            //Assert.IsTrue(stdev.Square().ApproximateEqual(variance, 0.00001));
            average.ApproximateEqual(source.Average()).Should().Be.True();
        }

        [TestCase(-10, 0.1, 5000)]
        [TestCase(0, 0.1, 5000)]
        [TestCase(0, 0.1, 50000)]
        public void AverageAndStDev_NullableTest(double seed, double step, int count) {
            var list = GetSerialList(seed, step, count);

            var source = list.Select(x => (double?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var variance = source.Variance().GetValueOrDefault();
            double? average, stdev;
            source.AverageAndStDev(out average, out stdev);

            if(IsDebugEnabled)
                log.Debug("Variance=[{0}], Sqrt(v)=[{3}], StDev=[{1}], Average=[{2}]", variance, stdev, average, Math.Sqrt(variance));

            Assert.IsTrue(stdev.GetValueOrDefault().Square().ApproximateEqual(variance, 0.001));
            Assert.IsTrue(average.GetValueOrDefault().ApproximateEqual(source.Average().GetValueOrDefault()));
        }

        #endregion

        #region << Correlation Coefficient >>

        [Test]
        [TestCase(-10, 0.1, 9999, -10, 0.1, 9999)]
        [TestCase(-10, 0.1, 9999, 10, 0.1, 9999)]
        [TestCase(-10, 0.1, 9999, 10, -0.1, 9999)]
        [TestCase(-10, 0.1, 9999, 10, -0.2, 9999)]
        public void CorreleationCoefficient_Test(double seed1, double step1, int count1, double seed2, double step2, int count2) {
            var source1 = GetSerialList(seed1, step1, count1);
            var source2 = GetSerialList(seed2, step2, count2);

            var coef = source1.CorrelationCoefficient(source2);

            if(IsDebugEnabled)
                log.Debug("Correlation coefficient=[{0}]", coef);

            if(Math.Abs(step1 - step2) < double.Epsilon)
                coef.ApproximateEqual(1, 1e-10).Should().Be.True();

            if(Math.Abs(step1 - -step2) < double.Epsilon)
                coef.ApproximateEqual(-1, 1e-10).Should().Be.True();

            Assert.IsTrue(step1 * step2 * coef > 0);
        }

        #endregion

        #region << Sum in System.Linq >>

        [Test]
        [TestCase(0.0, 9999)]
        [TestCase(1.0, 9999)]
        public void Sum_Double(double value, int length) {
            var list = ParallelEnumerable.Repeat(value, length);

            Assert.AreEqual(value * length, list.Sum());
        }

        #endregion
    }
}