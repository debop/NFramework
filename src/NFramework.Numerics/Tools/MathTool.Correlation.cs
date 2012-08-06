using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Mathematical Methods
    /// </summary>
    public static partial class MathTool {
        /// <summary>
        /// 지정된 두 차원의 변량들을 가지고 상관관계 계수를 계산한다.
        /// </summary>
        /// <param name="first">첫번째 변량 시퀀스</param>
        /// <param name="second">두번째 변량 시퀀스</param>
        /// <returns>두 변량 시퀀스의 상관관계 계수</returns>
        public static double CorrelationCoefficient(this IEnumerable<double> first, IEnumerable<double> second) {
            first.ShouldNotBeNull("first");
            second.ShouldNotBeNull("second");

            double sxx = 0;
            double syy = 0;
            double sxy = 0;

            double count = first.Count();

            var sx = first.Average();
            var sy = second.Average();

            using(var enumerator1 = first.GetEnumerator())
            using(var enumerator2 = second.GetEnumerator()) {
                while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
                    double dx = enumerator1.Current - sx;
                    double dy = enumerator2.Current - sy;

                    sxx += dx * dx;
                    syy += dy * dy;
                    sxy += dx * dy;
                }
            }

            sxx = Math.Sqrt(sxx / (count - 1));
            syy = Math.Sqrt(syy / (count - 1));
            sxy /= (count - 1) * sxx * syy;

            return sxy;
        }

        /// <summary>
        /// 지정된 두 시퀀스의 선택된 변량들을 가지고, 상관관계 계수를 계산한다.
        /// </summary>
        /// <typeparam name="T1">첫번째 변량을 가진 객체의 형식</typeparam>
        /// <typeparam name="T2">두번째 변량을 가진 객체의 형식</typeparam>
        /// <param name="first">첫번째 변량을 가진 객체의 시퀀스</param>
        /// <param name="firstSelector">첫번째 객체의 시퀀스로부터 변량을 선택하는 선택자</param>
        /// <param name="second">두번째 변량을 가진 객체의 시퀀스</param>
        /// <param name="secondSelector">두번째 객체의 시퀀스로부터 변량을 선택하는 선택자</param>
        /// <returns>상관 계수</returns>
        public static double CorrelationCoefficient<T1, T2>(this IEnumerable<T1> first,
                                                            Func<T1, double> firstSelector,
                                                            IEnumerable<T2> second,
                                                            Func<T2, double> secondSelector) {
            return first.Select(firstSelector).CorrelationCoefficient(second.Select(secondSelector));
        }

        /// <summary>
        /// Computes the Pearson product-moment correlation coefficient.
        /// </summary>
        /// <param name="first">첫번째 변량 시퀀스</param>
        /// <param name="second">두번째 변량 시퀀스</param>
        /// <returns>두 변량 시퀀스의 상관관계 계수</returns>
        public static double PearsonCorrelationCoefficient(this IEnumerable<double> first,
                                                           IEnumerable<double> second) {
            var n = 0;
            var r = 0.0;

            double mean1, stdev1;
            double mean2, stdev2;
            first.AverageAndStDev(out mean1, out stdev1);
            second.AverageAndStDev(out mean2, out stdev2);

            using(var iter1 = first.GetEnumerator())
            using(var iter2 = second.GetEnumerator()) {
                while(iter1.MoveNext() && iter2.MoveNext()) {
                    n++;
                    r += (iter1.Current - mean1) * (iter2.Current - mean2) / (stdev1 * stdev2);
                }
            }

            return r / (n - 1);
        }

        /// <summary>
        /// 지정된 두 시퀀스의 선택된 변량들을 가지고, Pearson product-moment 상관관계 계수를 계산한다.
        /// </summary>
        /// <typeparam name="T1">첫번째 변량을 가진 객체의 형식</typeparam>
        /// <typeparam name="T2">두번째 변량을 가진 객체의 형식</typeparam>
        /// <param name="first">첫번째 변량을 가진 객체의 시퀀스</param>
        /// <param name="firstSelector">첫번째 객체의 시퀀스로부터 변량을 선택하는 선택자</param>
        /// <param name="second">두번째 변량을 가진 객체의 시퀀스</param>
        /// <param name="secondSelector">두번째 객체의 시퀀스로부터 변량을 선택하는 선택자</param>
        /// <returns>상관 계수</returns>
        public static double PearsonCorrelationCoefficient<T1, T2>(this IEnumerable<T1> first,
                                                                   Func<T1, double> firstSelector,
                                                                   IEnumerable<T2> second,
                                                                   Func<T2, double> secondSelector) {
            return first.Select(firstSelector).PearsonCorrelationCoefficient(second.Select(secondSelector));
        }
    }
}