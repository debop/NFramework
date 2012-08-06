using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// <paramref name="source"/> 변량으로부터 평균(average), 평균편차(average deviation), 분산(variance), 기울기(skewness), 첨도(kurtosis, 뾰족한 정도)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="avg">평균</param>
        /// <param name="avgDev">평균 분산</param>
        /// <param name="variance">분산</param>
        /// <param name="skew">기울기(skewness) 양의 수이면 오른쪽으로 기울고, 음의 수이면 왼쪽으로 기울기</param>
        /// <param name="kurtosis">첨도 (Kurtosis, 뽀족한정도), 양의 수일수록 뾰족하고, 음의 수일 수록 뭉툭하다</param>
        public static void Moment(this IEnumerable<double> source, out double avg, out double avgDev, out double variance,
                                  out double skew, out double kurtosis) {
            var data = source.ToArray();
            int n = data.Length;

            n.ShouldBeGreaterOrEqual(2, "소스 항목의 수가 최소한 2개 이상이어야 합니다.");

            double sum = 0.0;
            for(var i = 0; i < n; i++)
                sum += data[i];

            avg = sum / n;
            avgDev = variance = skew = kurtosis = 0;

            for(var i = 0; i < n; i++) {
                double dx = data[i] - avg;
                avgDev += Math.Abs(dx);

                double p = dx * dx;
                variance += p;
                skew += (p *= dx);
                kurtosis += (p *= dx);
            }

            avgDev /= n;
            variance /= (n - 1);

            if(!double.IsNaN(variance) && Math.Abs(variance - 0.0) > double.Epsilon) {
                skew /= (n * variance * variance);
                kurtosis = kurtosis / (n * variance * variance) - 3.0;
            }
            else {
                skew = double.NaN;
                kurtosis = double.NaN;
            }
        }

        /// <summary>
        /// <paramref name="source"/> 변량으로부터 평균(average), 평균편차(average deviation), 분산(variance), 기울기(skewness), 첨도(kurtosis, 뾰족한 정도)
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <param name="avg">평균</param>
        /// <param name="avgDev">평균 분산</param>
        /// <param name="variance">분산</param>
        /// <param name="skew">기울기(skewness) 양의 수이면 오른쪽으로 기울고, 음의 수이면 왼쪽으로 기울기</param>
        /// <param name="kurtosis">첨도 (Kurtosis, 뽀족한정도), 양의 수일수록 뾰족하고, 음의 수일 수록 뭉툭하다</param>
        public static void Moment<T>(this IEnumerable<T> source, Func<T, double> selector, out double avg, out double avgDev,
                                     out double variance, out double skew, out double kurtosis) {
            Moment(source.Select(selector), out avg, out avgDev, out variance, out skew, out kurtosis);
        }
    }
}