using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 복소수 연산을 위한 Helper Class입니다.
    /// </summary>
    public static class ComplexTool {
        /// <summary>
        /// 제공된 Complex 시퀀스의 요소들의 modulus(length)를 지정된 최소, 최대값으로 제한한다.
        /// </summary>
        public static IEnumerable<Complex> ClampLength(this IEnumerable<Complex> source, double minValue, double maxValue) {
            source.ShouldNotBeNull("source");
            if(minValue > maxValue)
                throw new InvalidOperationException("minValue greater than maxValue");

            //foreach (Complex complex in source)
            //    yield return Complex.FromModulusAndArgument(Math.Max(minValue, Math.Min(maxValue, complex.GetModulus())),
            //                                                Math.Max(minValue, Math.Min(maxValue, complex.GetArgument())));

            return
                source.Select(
                    complex => Complex.FromModulusAndArgument(Math.Max(minValue, Math.Min(maxValue, complex.GetModulus())),
                                                              Math.Max(minValue, Math.Min(maxValue, complex.GetArgument()))));
        }

        /// <summary>
        /// 지정된 Complex 시퀀스의 요소들의 값을 최대, 최소값 범위안에 위치 시킨다.
        /// </summary>
        public static IEnumerable<Complex> Clamp(this IEnumerable<Complex> source, Complex min, Complex max) {
            source.ShouldNotBeNull("source");

            //foreach (Complex complex in source)
            //    yield return new Complex(Math.Min(Math.Max(complex.Re, min.Re), max.Re),
            //                             Math.Min(Math.Max(complex.Im, min.Im), max.Im));

            return
                source.Select(complex => new Complex(Math.Min(Math.Max(complex.Re, min.Re), max.Re),
                                                     Math.Min(Math.Max(complex.Im, min.Im), max.Im)));
        }

        /// <summary>
        /// 지정된 Complex 시퀀스의 요소들의 Real Part를 0~1 값을 가지게 하고, Image part는 0을 가지게 한다.
        /// </summary>
        public static IEnumerable<Complex> ClampToRealUnit(this IEnumerable<Complex> source) {
            source.ShouldNotBeNull("source");

            return source.Select(
                complex => new Complex(Math.Min(Math.Max(complex.Re, 0), 1), 0));
        }

        /// <summary>
        /// 지정된 Offset 값 만큼 Complex 를 Shift 시킨다.
        /// </summary>
        public static void Shift(this Complex[] source, int offset) {
            if(offset == 0)
                return;

            int count = source.Length;
            source.ShouldNotBeNull("source");
            Guard.Assert(offset >= 0 && offset < count, "offset value ranged (0 ~ [{0}]). but offset=[{1}]", count, offset);

            var temp = new Complex[count];
            for(var i = 0; i < count; i++)
                temp[(i + offset) % count] = source[i];

            for(var i = 0; i < count; i++)
                source[i] = temp[i];
        }

        /// <summary>
        /// 지정된 Complex 시퀀스에서 최대/최소 절대값(Modulus/Length)를 구한다.
        /// </summary>
        public static void GetLengthRange(this IEnumerable<Complex> source, out double min, out double max) {
            source.ShouldNotBeNull("source");

            min = double.MaxValue;
            max = double.MinValue;

            foreach(Complex complex in source) {
                double m = complex.GetModulus();
                if(m > max)
                    max = m;
                if(m < min)
                    min = m;
            }
        }

        /// <summary>
        /// 지정된 두개의 Complex 시퀀스의 요소들이 오차범위에서 일치하는지 검사한다.
        /// </summary>
        public static bool SequentialApproximateEqual(this IEnumerable<Complex> first, IEnumerable<Complex> second,
                                                      double epsilon = MathTool.Epsilon) {
            first.ShouldNotBeNull("first");
            second.ShouldNotBeNull("second");

            using(var enumerator = first.GetEnumerator())
            using(var enumerator2 = second.GetEnumerator()) {
                while(enumerator.MoveNext()) {
                    if(!enumerator2.MoveNext() || Complex.ApproximateEquals(enumerator.Current, enumerator2.Current, epsilon) == false)
                        return false;
                }
                if(enumerator2.MoveNext())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 지정된 offset 만큼 Complex 시퀀스의 real part 값을 증가시킨다.
        /// </summary>
        public static IEnumerable<Complex> Offset(this IEnumerable<Complex> source, double offset) {
            source.ShouldNotBeNull("source");
            return source.Select(complex => complex + offset);
        }

        /// <summary>
        /// 지정된 offset 만큼 Complex 시퀀스의 real part 값을 증가시킨다.
        /// </summary>
        public static IEnumerable<Complex> Offset(this IEnumerable<Complex> source, Complex offset) {
            source.ShouldNotBeNull("source");
            return source.Select(complex => complex + offset);
        }

        /// <summary>
        /// 시퀀스의 Complex에 scale 값을 곱한다.
        /// </summary>
        public static IEnumerable<Complex> Scale(this IEnumerable<Complex> source, double scale) {
            source.ShouldNotBeNull("source");
            return source.Select(complex => complex * scale);
        }

        /// <summary>
        /// 시퀀스의 Complex에 sclae 값을 곱한다.
        /// </summary>
        public static IEnumerable<Complex> Scale(this IEnumerable<Complex> source, Complex scale) {
            source.ShouldNotBeNull("source");
            return source.Select(complex => complex * scale);
        }

        /// <summary>
        /// 두 시퀀스를 곱합니다.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> first, IEnumerable<Complex> second) {
            first.ShouldNotBeNull("first");
            second.ShouldNotBeNull("second");

            Guard.Assert(first.Count() == second.Count(), "first and second sequence length should be same.");

            using(var enumerator1 = first.GetEnumerator())
            using(var enumerator2 = second.GetEnumerator()) {
                while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
                    yield return enumerator1.Current * enumerator2.Current;
                }
            }
        }

        /// <summary>
        /// ㅅ
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<Complex> Divide(this IEnumerable<Complex> first, IEnumerable<Complex> second) {
            first.ShouldNotBeNull("first");
            second.ShouldNotBeNull("second");

            Guard.Assert(first.Count() == second.Count(), "first and second sequence length should be same.");

            using(var enumerator1 = first.GetEnumerator())
            using(var enumerator2 = second.GetEnumerator()) {
                while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
                    if(enumerator2.Current != Complex.Zero)
                        yield return enumerator1.Current / enumerator2.Current;
                    else
                        yield return Complex.Zero;
                }
            }
        }

        /// <summary>
        /// <paramref name="src"/> 요소를 <paramref name="dest"/>로 복사합니다. 대상 배열의 크기가 원본의 크기보다 크거나 같아야합니다.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void Copy(this Complex[] src, Complex[] dest) {
            src.ShouldNotBeNull("src");
            dest.ShouldNotBeNull("dest");
            Guard.Assert(src.Length <= dest.Length, "원본 배열의 크기가 대상 배열의 크기와 같거나 작아야 합니다.");

            Array.Copy(src, 0, dest, 0, dest.Length);
        }

        /// <summary>
        /// 지정된 배열을 역순 정렬을 합니다.
        /// </summary>
        /// <param name="source"></param>
        public static void Reverse(this Complex[] source) {
            var length = source.Length;

            for(var i = 0; i < length / 2; i++) {
                Complex temp = source[i];
                source[i] = source[length - 1 - i];
                source[length - 1 - i] = temp;
            }
        }

        /// <summary>
        /// 시퀀스 요소의 Length들을 0~1의 값을 갖도록 정규화를 수행한다.
        /// </summary>
        public static IEnumerable<Complex> Normalize(this IEnumerable<Complex> source) {
            source.ShouldNotBeNull("source");

            double min, max;
            source.GetLengthRange(out min, out max);

            var range = max - min;
            return source.Scale(1 / range).Offset(-min / range);
        }

        /// <summary>
        /// Invert  (1 / c)
        /// </summary>
        public static IEnumerable<Complex> Invert(this IEnumerable<Complex> source) {
            source.ShouldNotBeNull("source");
            return source.Select(complex => (Complex)1 / complex);
        }

        /// <summary>
        /// Complex 시퀀스의 합을 구한다.
        /// </summary>
        public static Complex Sum(this IEnumerable<Complex> source) {
            source.ShouldNotBeNull("source");
            return source.Aggregate(Complex.Zero, (result, next) => result + next);
        }

        /// <summary>
        /// 시퀀스의 startIndex 부터 count 갯수 만큼의 요소만을 합산한다.
        /// </summary>
        public static Complex SumRecursive(this IEnumerable<Complex> source, int startIndex, int count) {
            source.ShouldNotBeNull("source");

            return source.Skip(startIndex).Take(count).Sum();
        }

        /// <summary>
        /// 지정된 시퀀스의 Complex 요소의 제곱의 합을 구합니다.
        /// </summary>
        public static Complex SumOfSquares(this IEnumerable<Complex> source) {
            source.ShouldNotBeNull("source");
            return source.Aggregate(Complex.Zero, (result, next) => result + next * next);
        }

        /// <summary>
        /// 지정된 시퀀스의 범위에 있는 Complex 요소의 제곱의 합을 구합니다.
        /// </summary>
        public static Complex SumOfSquares(this IEnumerable<Complex> source, int startIndex, int count) {
            source.ShouldNotBeNull("source");
            return source.Skip(startIndex).Take(count).SumOfSquares();
        }

        /// <summary>
        /// Returns a Norm of a value of this type, which is appropriate for measuring how
        /// close this value is to zero.
        /// </summary>
        /// <param name="complex">The <see cref="Complex"/> number to perfom this operation on.</param>
        /// <returns>A norm of this value.</returns>
        public static double Norm(this Complex complex) {
            return complex.GetModulusSquared();
        }

        /// <summary>
        /// Returns a Norm of the difference of two values of this type, which is
        /// appropriate for measuring how close together these two values are.
        /// </summary>
        /// <param name="complex">The <see cref="Complex"/> number to perfom this operation on.</param>
        /// <param name="other">The value to compare with.</param>
        /// <returns>A norm of the difference between this and the other value.</returns>
        public static double NormOfDifference(this Complex complex, Complex other) {
            return (complex - other).GetModulusSquared();
        }

        /// <summary>
        /// 평균, 중간값
        /// </summary>
        public static Complex Mean(this IEnumerable<Complex> source) {
            source.ShouldNotBeNull("source");

            int count = source.Count();
            count.ShouldBePositive("count of source.");

            return source.Sum() / (double)count;
        }

        /// <summary>
        /// 분산
        /// </summary>
        public static Complex Variance(this IEnumerable<Complex> source) {
            source.ShouldNotBeNull("source");

            int count = source.Count();
            count.ShouldBePositive("count of source.");

            return source.SumOfSquares() / (double)count - source.Sum();
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public static Complex StdDev(this IEnumerable<Complex> source) {
            source.ShouldNotBeNull("source");

            return Variance(source).Sqrt();
        }

        /// <summary>
        /// 평균 제곱합 제곱근 오차(Root Mean Square Error: RMS 오차)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public static double RMSError(IEnumerable<Complex> alpha, IEnumerable<Complex> beta) {
            return Math.Sqrt(SumOfSquaredError(alpha, beta));
        }

        /// <summary>
        /// 두 시퀀스의 차이에 대한 제곱 값의 총합 
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public static double SumOfSquaredError(IEnumerable<Complex> alpha, IEnumerable<Complex> beta) {
            double rms = 0.0;

            using(var enumerator1 = alpha.GetEnumerator())
            using(var enumerator2 = beta.GetEnumerator()) {
                while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
                    var delta = enumerator2.Current - enumerator1.Current;
                    rms += delta.GetModulusSquared();
                }
            }
            return rms;
        }

        /// <summary>
        /// 지정된 배열의 지정된 위치의 요소를 교환합니다.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void Swap(this Complex[] array, int left, int right) {
            var temp = array[left];
            array[left] = array[right];
            array[right] = temp;
        }

        /// <summary>
        /// 두 변수를 교환합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap(ref Complex a, ref Complex b) {
            var temp = a;
            a = b;
            b = temp;
        }

        private static readonly double HalfOfRoot2 = 0.5 * Math.Sqrt(2.0);

        /// <summary>
        /// 제곱근
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Complex Sqrt(this Complex c) {
            double re = c.Re;
            double im = c.Im;

            double modulus = c.GetModulus();
            int sign = (im < 0) ? -1 : 1;

            c.Re = HalfOfRoot2 * Math.Sqrt(modulus + re);
            c.Im = HalfOfRoot2 * sign * Math.Sqrt(modulus - im);

            return c;
        }

        /// <summary>
        /// 지수
        /// </summary>
        /// <param name="c"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Complex Pow(this Complex c, double e) {
            double re = c.Re;
            double im = c.Im;

            double modulus = Math.Pow(c.GetModulus(), e * 0.5);
            double argument = Math.Atan2(im, re) * e;

            c.Re = modulus * Math.Cos(argument);
            c.Im = modulus * Math.Sin(argument);

            return c;
        }
    }
}