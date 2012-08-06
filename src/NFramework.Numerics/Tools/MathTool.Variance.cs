using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        #region << Variance (분산) >>

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Variance(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            double avg = 0;
            double variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    var x = iter.Current;
                    n++;
                    x -= avg;
                    avg += x / n;
                    variance += (n - 1) * x * x / n;
                }

            return variance / (n - 1);
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다 (요소중 null 값은 제외합니다)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Variance(this IEnumerable<double?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            double avg = 0;
            double variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    if(iter.Current.HasValue) {
                        var x = iter.Current.Value;
                        n++;
                        x -= avg;
                        avg += x / n;
                        variance += (n - 1) * x * x / n;
                    }
                }
            return (n > 1) ? (double?)(variance / (n - 1)) : null;
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float Variance(this IEnumerable<float> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            float avg = 0;
            float variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    float x = iter.Current;
                    n++;
                    x -= avg;
                    avg += x / n;
                    variance += (n - 1) * x * x / n;
                }

            return variance / (n - 1);
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다 (요소중 null 값은 제외합니다)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float? Variance(this IEnumerable<float?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            float avg = 0;
            float variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    if(iter.Current.HasValue) {
                        float x = iter.Current.Value;
                        n++;
                        x -= avg;
                        avg += x / n;
                        variance += (n - 1) * x * x / n;
                    }
                }
            return (n > 1) ? (float?)(variance / (n - 1)) : null;
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal Variance(this IEnumerable<decimal> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            decimal avg = 0;
            decimal variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    var x = iter.Current;
                    n++;
                    x -= avg;
                    avg += x / n;
                    variance += (n - 1) * x * x / n;
                }

            return variance / (n - 1);
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다 (요소중 null 값은 제외합니다)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal? Variance(this IEnumerable<decimal?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            decimal avg = 0;
            decimal variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    if(iter.Current.HasValue) {
                        decimal x = iter.Current.Value;
                        n++;
                        x -= avg;
                        avg += x / n;
                        variance += (n - 1) * x * x / n;
                    }
                }
            return (n > 1) ? (decimal?)(variance / (n - 1)) : null;
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Variance(this IEnumerable<long> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            double avg = 0;
            double variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    double x = iter.Current;
                    n++;
                    x -= avg;
                    avg += x / n;
                    variance += (n - 1) * x * x / n;
                }

            return variance / (n - 1);
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다 (요소중 null 값은 제외합니다)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Variance(this IEnumerable<long?> source) {
            source.ShouldNotBeNull("source");
            source.Where(x => x.HasValue).Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            double avg = 0;
            double variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    if(iter.Current.HasValue) {
                        double x = iter.Current.Value;
                        n++;
                        x -= avg;
                        avg += x / n;
                        variance += (n - 1) * x * x / n;
                    }
                }
            return (n > 1) ? (double?)(variance / (n - 1)) : null;
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Variance(this IEnumerable<int> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            double avg = 0;
            double variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    double x = iter.Current;
                    n++;
                    x -= avg;
                    avg += x / n;
                    variance += (n - 1) * x * x / n;
                }

            return variance / (n - 1);
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다 (요소중 null 값은 제외합니다)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Variance(this IEnumerable<int?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            double avg = 0;
            double variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    if(iter.Current.HasValue) {
                        double x = iter.Current.Value;
                        n++;
                        x -= avg;
                        avg += x / n;
                        variance += (n - 1) * x * x / n;
                    }
                }
            return (n > 1) ? (double?)(variance / (n - 1)) : null;
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static double Variance<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static double? Variance<T>(this IEnumerable<T> source, Func<T, double?> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static float Variance<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static float? Variance<T>(this IEnumerable<T> source, Func<T, float?> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static decimal Variance<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static decimal? Variance<T>(this IEnumerable<T> source, Func<T, decimal?> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static double Variance<T>(this IEnumerable<T> source, Func<T, long> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static double? Variance<T>(this IEnumerable<T> source, Func<T, long?> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static double Variance<T>(this IEnumerable<T> source, Func<T, int> selector) {
            return source.Select(selector).Variance();
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>분산</returns>
        public static double? Variance<T>(this IEnumerable<T> source, Func<T, int?> selector) {
            return source.Select(selector).Variance();
        }

        #endregion

        #region << Variance ( Cumulative ) - 누적 분산 >>

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double> CumulativeVariance(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator())
                if(iter.MoveNext()) {
                    int n = 1;
                    double sum = iter.Current;
                    double sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        double curr = iter.Current;
                        n++;
                        sum += curr;
                        sumSqrt += curr * curr;

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double?> CumulativeVariance(this IEnumerable<double?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    int n = iter.Current.HasValue ? 1 : 0;
                    double sum = iter.Current.GetValueOrDefault();
                    double sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        if(iter.Current.HasValue) {
                            double curr = iter.Current.Value;
                            n++;
                            sum += curr;
                            sumSqrt += curr * curr;
                        }

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<float> CumulativeVariance(this IEnumerable<float> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    int n = 1;
                    float sum = iter.Current;
                    float sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        float curr = iter.Current;
                        n++;
                        sum += curr;
                        sumSqrt += curr * curr;

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<float?> CumulativeVariance(this IEnumerable<float?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    int n = iter.Current.HasValue ? 1 : 0;
                    float sum = iter.Current.GetValueOrDefault();
                    float sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        if(iter.Current.HasValue) {
                            float curr = iter.Current.Value;
                            n++;
                            sum += curr;
                            sumSqrt += curr * curr;
                        }

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<decimal> CumulativeVariance(this IEnumerable<decimal> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    int n = 1;
                    decimal sum = iter.Current;
                    decimal sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        decimal curr = iter.Current;
                        n++;
                        sum += curr;
                        sumSqrt += curr * curr;

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<decimal?> CumulativeVariance(this IEnumerable<decimal?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    int n = iter.Current.HasValue ? 1 : 0;
                    decimal sum = iter.Current.GetValueOrDefault();
                    decimal sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        if(iter.Current.HasValue) {
                            decimal curr = iter.Current.Value;
                            n++;
                            sum += curr;
                            sumSqrt += curr * curr;
                        }

                        if(n <= 1)
                            throw new DivideByZeroException();

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double> CumulativeVariance(this IEnumerable<long> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    int n = 1;
                    double sum = iter.Current;
                    double sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        double curr = iter.Current;
                        n++;
                        sum += curr;
                        sumSqrt += curr * curr;

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double?> CumulativeVariance(this IEnumerable<long?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    int n = iter.Current.HasValue ? 1 : 0;
                    double sum = iter.Current.GetValueOrDefault();
                    double sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        if(iter.Current.HasValue) {
                            double curr = iter.Current.Value;
                            n++;
                            sum += curr;
                            sumSqrt += curr * curr;
                        }

                        if(n <= 1)
                            throw new DivideByZeroException();

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double> CumulativeVariance(this IEnumerable<int> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    var n = 1;
                    double sum = iter.Current;
                    double sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        double curr = iter.Current;
                        n++;
                        sum += curr;
                        sumSqrt += curr * curr;

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double?> CumulativeVariance(this IEnumerable<int?> source) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext()) {
                    int n = iter.Current.HasValue ? 1 : 0;
                    double sum = iter.Current.GetValueOrDefault();
                    double sumSqrt = sum * sum;

                    while(iter.MoveNext()) {
                        if(iter.Current.HasValue) {
                            double curr = iter.Current.Value;
                            n++;
                            sum += curr;
                            sumSqrt += curr * curr;
                        }
                        if(n <= 1)
                            throw new DivideByZeroException();

                        yield return (sumSqrt - sum * sum / n) / (n - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return CumulativeVariance(source.Select(selector));
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double?> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, double?> selector) {
            return CumulativeVariance(source.Select(selector));
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<float> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(selector).CumulativeVariance();
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<float?> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, float?> selector) {
            return source.Select(selector).CumulativeVariance();
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<decimal> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(selector).CumulativeVariance();
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<decimal?> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, decimal?> selector) {
            return source.Select(selector).CumulativeVariance();
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, long> selector) {
            return source.Select(selector).CumulativeVariance();
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double?> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, long?> selector) {
            return source.Select(selector).CumulativeVariance();
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, int> selector) {
            return source.Select(selector).CumulativeVariance();
        }

        /// <summary>
        /// 시퀀스의 분산을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">시퀀스 항목으로부터 요소를 선택하는 선택자</param>
        /// <returns>누적 분산</returns>
        public static IEnumerable<double?> CumulativeVariance<T>(this IEnumerable<T> source, Func<T, int?> selector) {
            return source.Select(selector).CumulativeVariance();
        }

        #endregion

        #region << Variance (Block) - 몇개의 블럭단위로 분산을 계산합니다. >>

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double> Variance(this IEnumerable<double> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var blockSize2 = blockSize - 1;
            double sum = 0;
            double sumSqrt = 0;
            int nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                double curr;
                var elements = 0;

                while(block > 1) {
                    block--;
                    if(right.MoveNext() == false) {
                        Guard.Assert(elements >= 2, "요소수가 최소한 2개 이상이어야 합니다.");
                        if(nans > 0)
                            yield return double.NaN;
                        else
                            yield return (sumSqrt - sum * (sum / elements)) / (elements - 1);
                        yield break;
                    }
                    curr = right.Current;
                    if(double.IsNaN(curr))
                        nans = blockSize;
                    else {
                        sum += curr;
                        sumSqrt += curr * curr;
                        nans--;
                    }
                    elements++;
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    if(double.IsNaN(curr)) {
                        nans = blockSize;
                    }
                    else {
                        sum += curr;
                        sumSqrt += curr * curr;
                        nans--;
                    }
                    if(nans > 0)
                        yield return double.NaN;
                    else
                        yield return (sumSqrt - sum * (sum / blockSize)) / blockSize2;

                    left.MoveNext();
                    curr = left.Current;

                    if(double.IsNaN(curr) == false) {
                        sum -= curr;
                        sumSqrt -= curr * curr;
                    }
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double?> Variance(this IEnumerable<double?> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            double sum = 0;
            double sumSqrt = 0;
            int nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                double curr;
                int elements = 0;

                while(block > 1) {
                    block--;
                    if(right.MoveNext() == false) {
                        Guard.Assert(elements >= 2, "요소수가 최소한 2개 이상이어야 합니다.");

                        if(nans > 0)
                            yield return double.NaN;
                        else
                            yield return (sumSqrt - sum * (sum / elements)) / (elements - 1);

                        yield break;
                    }
                    curr = right.Current.GetValueOrDefault();
                    nans--;

                    if(right.Current.HasValue) {
                        if(double.IsNaN(curr))
                            nans = blockSize;
                        else {
                            sum += curr;
                            sumSqrt += curr * curr;
                        }
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current.GetValueOrDefault();
                    nans--;
                    if(right.Current.HasValue) {
                        if(double.IsNaN(curr))
                            nans = blockSize;
                        else {
                            sum += curr;
                            sumSqrt += curr * curr;
                        }
                        elements++;
                    }

                    if(elements <= 1)
                        yield return null;
                    else if(nans > 0)
                        yield return double.NaN;
                    else
                        yield return (sumSqrt - sum * (sum / elements)) / (elements - 1);

                    left.MoveNext();
                    curr = left.Current.GetValueOrDefault();

                    if(left.Current.HasValue) {
                        if(double.IsNaN(curr) == false) {
                            sum -= curr;
                            sumSqrt -= curr * curr;
                        }
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<float> Variance(this IEnumerable<float> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var blockSize2 = blockSize - 1;
            float sum = 0;
            float sumSqrt = 0;
            int nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                float curr;
                var elements = 0;

                while(block > 1) {
                    block--;
                    if(right.MoveNext() == false) {
                        Guard.Assert(elements >= 2, "요소수가 최소한 2개 이상이어야 합니다.");
                        if(nans > 0)
                            yield return float.NaN;
                        else
                            yield return (sumSqrt - sum * (sum / elements)) / (elements - 1);
                        yield break;
                    }
                    curr = right.Current;
                    if(float.IsNaN(curr))
                        nans = blockSize;
                    else {
                        sum += curr;
                        sumSqrt += curr * curr;
                        nans--;
                    }
                    elements++;
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    if(float.IsNaN(curr)) {
                        nans = blockSize;
                    }
                    else {
                        sum += curr;
                        sumSqrt += curr * curr;
                        nans--;
                    }
                    if(nans > 0)
                        yield return float.NaN;
                    else
                        yield return (sumSqrt - sum * (sum / blockSize)) / blockSize2;

                    left.MoveNext();
                    curr = left.Current;

                    if(float.IsNaN(curr) == false) {
                        sum -= curr;
                        sumSqrt -= curr * curr;
                    }
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<float?> Variance(this IEnumerable<float?> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            float sum = 0;
            float sumSqrt = 0;
            int nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                float curr;
                int elements = 0;

                while(block > 1) {
                    block--;
                    if(right.MoveNext() == false) {
                        Guard.Assert(elements >= 2, "요소수가 최소한 2개 이상이어야 합니다.");

                        if(nans > 0)
                            yield return float.NaN;
                        else
                            yield return (sumSqrt - sum * (sum / elements)) / (elements - 1);

                        yield break;
                    }
                    curr = right.Current.GetValueOrDefault();
                    nans--;

                    if(right.Current.HasValue) {
                        if(float.IsNaN(curr))
                            nans = blockSize;
                        else {
                            sum += curr;
                            sumSqrt += curr * curr;
                        }
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current.GetValueOrDefault();
                    nans--;
                    if(right.Current.HasValue) {
                        if(float.IsNaN(curr))
                            nans = blockSize;
                        else {
                            sum += curr;
                            sumSqrt += curr * curr;
                        }
                        elements++;
                    }

                    if(elements <= 1)
                        yield return null;
                    else if(nans > 0)
                        yield return float.NaN;
                    else
                        yield return (sumSqrt - sum * (sum / elements)) / (elements - 1);

                    left.MoveNext();
                    curr = left.Current.GetValueOrDefault();

                    if(left.Current.HasValue) {
                        if(float.IsNaN(curr) == false) {
                            sum -= curr;
                            sumSqrt -= curr * curr;
                        }
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<decimal> Variance(this IEnumerable<decimal> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            int block = blockSize;
            int blockSize2 = blockSize - 1;
            decimal sum = 0;
            decimal sumSqr = 0;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                int elements = 0;
                decimal curr;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        Guard.Assert(elements >= 2, "시퀀스는 최소한 2개 이상의 항목을 가져야 합니다.");

                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);
                        yield break;
                    }
                    curr = right.Current;
                    sum += curr;
                    sumSqr += curr * curr;
                    elements++;
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    sum += curr;
                    sumSqr += curr * curr;
                    yield return (sumSqr - sum * (sum / blockSize)) / blockSize2;
                    left.MoveNext();
                    curr = left.Current;
                    sum -= curr;
                    sumSqr -= curr * curr;
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<decimal?> Variance(this IEnumerable<decimal?> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            int block = blockSize;
            decimal sum = 0;
            decimal sumSqr = 0;
            int nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                int elements = 0;
                decimal curr;
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        Guard.Assert(elements >= 2, "시퀀스는 최소한 2개 이상의 항목을 가져야 합니다.");

                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);
                        yield break;
                    }
                    curr = right.Current.GetValueOrDefault();
                    nans--;
                    if(right.Current.HasValue) {
                        sum += curr;
                        sumSqr += curr * curr;
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current.GetValueOrDefault();
                    nans--;
                    if(right.Current.HasValue) {
                        sum += curr;
                        sumSqr += curr * curr;
                        elements++;
                    }
                    if(elements <= 1)
                        yield return null;
                    else
                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);

                    left.MoveNext();
                    curr = left.Current.GetValueOrDefault();
                    if(left.Current.HasValue) {
                        sum -= curr;
                        sumSqr -= curr * curr;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double> Variance(this IEnumerable<long> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            int block = blockSize;
            int blockSize2 = blockSize - 1;
            double sum = 0;
            double sumSqr = 0;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                int elements = 0;
                long curr;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        Guard.Assert(elements >= 2, "시퀀스는 최소한 2개 이상의 항목을 가져야 합니다.");

                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);
                        yield break;
                    }
                    curr = right.Current;
                    sum += curr;
                    sumSqr += curr * curr;
                    elements++;
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    sum += curr;
                    sumSqr += curr * curr;
                    yield return (sumSqr - sum * (sum / blockSize)) / blockSize2;
                    left.MoveNext();
                    curr = left.Current;
                    sum -= curr;
                    sumSqr -= curr * curr;
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double?> Variance(this IEnumerable<long?> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            int block = blockSize;
            double sum = 0;
            double sumSqr = 0;
            int nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                int elements = 0;
                long curr;
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        Guard.Assert(elements >= 2, "시퀀스는 최소한 2개 이상의 항목을 가져야 합니다.");

                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);
                        yield break;
                    }
                    curr = right.Current.GetValueOrDefault();
                    nans--;
                    if(right.Current.HasValue) {
                        sum += curr;
                        sumSqr += curr * curr;
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current.GetValueOrDefault();
                    nans--;
                    if(right.Current.HasValue) {
                        sum += curr;
                        sumSqr += curr * curr;
                        elements++;
                    }
                    if(elements <= 1)
                        yield return null;
                    else
                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);

                    left.MoveNext();
                    curr = left.Current.GetValueOrDefault();
                    if(left.Current.HasValue) {
                        sum -= curr;
                        sumSqr -= curr * curr;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double> Variance(this IEnumerable<int> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            int block = blockSize;
            int blockSize2 = blockSize - 1;
            double sum = 0;
            double sumSqr = 0;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                int elements = 0;
                int curr;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        Guard.Assert(elements >= 2, "시퀀스는 최소한 2개 이상의 항목을 가져야 합니다.");

                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);
                        yield break;
                    }
                    curr = right.Current;
                    sum += curr;
                    sumSqr += curr * curr;
                    elements++;
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    sum += curr;
                    sumSqr += curr * curr;
                    yield return (sumSqr - sum * (sum / blockSize)) / blockSize2;
                    left.MoveNext();
                    curr = left.Current;
                    sum -= curr;
                    sumSqr -= curr * curr;
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double?> Variance(this IEnumerable<int?> source, int blockSize = BlockSize) {
            source.ShouldNotBeNull("source");
            source.Take(2).Count().ShouldBeGreaterOrEqual(2, "count of source");
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            int block = blockSize;
            double sum = 0;
            double sumSqr = 0;
            int nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                int elements = 0;
                int curr;
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        Guard.Assert(elements >= 2, "시퀀스는 최소한 2개 이상의 항목을 가져야 합니다.");

                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);
                        yield break;
                    }
                    curr = right.Current.GetValueOrDefault();
                    nans--;
                    if(right.Current.HasValue) {
                        sum += curr;
                        sumSqr += curr * curr;
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current.GetValueOrDefault();
                    nans--;
                    if(right.Current.HasValue) {
                        sum += curr;
                        sumSqr += curr * curr;
                        elements++;
                    }
                    if(elements <= 1)
                        yield return null;
                    else
                        yield return (sumSqr - sum * (sum / elements)) / (elements - 1);

                    left.MoveNext();
                    curr = left.Current.GetValueOrDefault();
                    if(left.Current.HasValue) {
                        sum -= curr;
                        sumSqr -= curr * curr;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, double> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double?> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, double?> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<float> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, float> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<float?> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, float?> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<decimal> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, decimal> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<decimal?> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, decimal?> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, long> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double?> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, long?> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, int> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 분산을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">분산 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 분산</returns>
        public static IEnumerable<double?> Variance<T>(this IEnumerable<T> source, int blockSize, Func<T, int?> selector) {
            return source.Select(selector).Variance(blockSize);
        }

        #endregion
    }
}