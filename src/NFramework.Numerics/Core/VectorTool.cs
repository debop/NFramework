using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Utility class for <see cref="Vector"/>
    /// </summary>
    public static class VectorTool {
        /// <summary>
        /// 난수발생기
        /// </summary>
        public static readonly Random Rnd = ThreadTool.CreateRandom();

        /// <summary>
        /// 두 Vector의 창원이 같은지 검사한다.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public static void CheckDimension(Vector u, Vector v) {
            Guard.Assert(IsSameDimension(u, v), "Vector dimension is not equal.");
        }

        /// <summary>
        /// 두 Vector가 같은 Dimension을 가졌는지 검사한다.
        /// </summary>
        public static bool IsSameDimension(Vector u, Vector v) {
            if(u == null || v == null)
                return false;

            return u.Length == v.Length;
        }

        /// <summary>
        /// 무작위 벡터를 만듭니다.
        /// </summary>
        public static Vector RandomVector(int length) {
            length.ShouldBePositive("length of vector");

            var vector = new Vector(length);
            for(int i = 0; i < length; i++)
                vector[i] = Rnd.NextDouble();

            return vector;
        }

        /// <summary>
        /// 무작위 벡터를 <paramref name="count"/> 갯수만큼 생성합니다.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<Vector> RandomVector(int count, int length) {
            length.ShouldBePositive("length of vector");

            for(int c = 0; c < count; c++)
                yield return RandomVector(length);
        }

        /// <summary>
        /// Vector의 절대값을 계산한다.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>Vector의 절대값</returns>
        public static double AbsSum(this Vector v) {
            return v.Data.AbsSum();
        }

        /// <summary>
        /// 두 벡터의 Dot 를 구한다. (요소 순서별로 곱하기의 합)
        /// </summary>
        /// <param name="u">First Vector</param>
        /// <param name="v">Second Vector</param>
        /// <returns>Dot of vector</returns>
        public static double Dot(this Vector u, Vector v) {
            CheckDimension(u, v);

            double sum = 0.0;

            for(int i = 0; i < u.Length; i++)
                sum += u[i] * v[i];

            return sum;
        }

        /// <summary>
        /// 절대값이 가장 큰 요소를 반환한다.
        /// </summary>
        public static double Norm1(this Vector v) {
            return v.Data.Max(x => Math.Abs(x));
        }

        /// <summary>
        /// 두 벡터의 Dot의 제곱근
        /// </summary>
        /// <param name="u">First Vector</param>
        /// <param name="v">Second Vector</param>
        /// <returns></returns>
        public static double Norm2(this Vector u, Vector v) {
            return Math.Sqrt(Dot(u, v));
        }

        /// <summary>
        /// 벡터의 요소 중 가장 큰 값 반환한다.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns></returns>
        public static double NormInf(this Vector v) {
            return v.Norm1();
        }

        /// <summary>
        /// 벡터의 길이를 구한다.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>벡터의 길이</returns>
        public static double NormF(this Vector v) {
            return v.Aggregate<double, double>(0, (sum, t) => sum.Hypot(t));
        }
    }
}