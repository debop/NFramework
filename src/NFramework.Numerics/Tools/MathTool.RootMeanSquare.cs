using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double RootMeanSquare(this IEnumerable<double> source) {
            double rms = 0.0;
            long n = 0;

            foreach(var x in source) {
                rms += x * x;
                n++;
            }

            return Math.Sqrt(rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double RootMeanSquare(this IEnumerable<double?> source) {
            double rms = 0.0;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                rms += x.Value * x.Value;
                n++;
            }

            return Math.Sqrt(rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float RootMeanSquare(this IEnumerable<float> source) {
            float rms = 0.0f;
            long n = 0;

            foreach(var x in source) {
                rms += x * x;
                n++;
            }

            return (float)Math.Sqrt(rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float RootMeanSquare(this IEnumerable<float?> source) {
            float rms = 0.0f;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                rms += x.Value * x.Value;
                n++;
            }

            return (float)Math.Sqrt(rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal RootMeanSquare(this IEnumerable<decimal> source) {
            decimal rms = 0.0m;
            long n = 0;

            foreach(var x in source) {
                rms += x * x;
                n++;
            }

            return (decimal)Math.Sqrt((double)rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal RootMeanSquare(this IEnumerable<decimal?> source) {
            decimal rms = 0.0m;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                rms += x.Value * x.Value;
                n++;
            }

            return (decimal)Math.Sqrt((double)rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double RootMeanSquare(this IEnumerable<long> source) {
            double rms = 0.0;
            long n = 0;

            foreach(var x in source) {
                rms += x * x;
                n++;
            }

            return Math.Sqrt(rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double RootMeanSquare(this IEnumerable<long?> source) {
            double rms = 0.0;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                rms += x.Value * x.Value;
                n++;
            }

            return Math.Sqrt(rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double RootMeanSquare(this IEnumerable<int> source) {
            double rms = 0.0;
            long n = 0;

            foreach(var x in source) {
                rms += x * x;
                n++;
            }

            return Math.Sqrt(rms / (n - 1));
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double RootMeanSquare(this IEnumerable<int?> source) {
            double rms = 0.0;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                rms += x.Value * x.Value;
                n++;
            }

            return Math.Sqrt(rms / (n - 1));
        }

        //!+ ----------------------------------------------------------------------------------------

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double RootMeanSquareError(this IEnumerable<double> expected, IEnumerable<double> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            double rmse = 0.0;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                rmse += (expecter.Current - actualer.Current).Square();
                n++;
            }

            return Math.Sqrt(rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double RootMeanSquareError(this IEnumerable<double?> expected, IEnumerable<double?> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            double rmse = 0.0;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                if(expecter.Current.HasValue && actualer.Current.HasValue) {
                    rmse += (expecter.Current.Value - actualer.Current.Value).Square();
                    n++;
                }
            }

            return Math.Sqrt(rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static float RootMeanSquareError(this IEnumerable<float> expected, IEnumerable<float> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            float rmse = 0.0f;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                rmse += (expecter.Current - actualer.Current).Square();
                n++;
            }

            return (float)Math.Sqrt((double)rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static float RootMeanSquareError(this IEnumerable<float?> expected, IEnumerable<float?> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            float rmse = 0.0f;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                if(expecter.Current.HasValue && actualer.Current.HasValue) {
                    rmse += (expecter.Current.Value - actualer.Current.Value).Square();
                    n++;
                }
            }

            return (float)Math.Sqrt((double)rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static decimal RootMeanSquareError(this IEnumerable<decimal> expected, IEnumerable<decimal> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            decimal rmse = 0.0m;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                rmse += (expecter.Current - actualer.Current).Square();
                n++;
            }

            return (decimal)Math.Sqrt((double)rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static decimal RootMeanSquareError(this IEnumerable<decimal?> expected, IEnumerable<decimal?> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            decimal rmse = 0.0m;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                if(expecter.Current.HasValue && actualer.Current.HasValue) {
                    rmse += (expecter.Current.Value - actualer.Current.Value).Square();
                    n++;
                }
            }

            return (decimal)Math.Sqrt((double)rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double RootMeanSquareError(this IEnumerable<long> expected, IEnumerable<long> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            double rmse = 0.0;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                rmse += (expecter.Current - actualer.Current).Square();
                n++;
            }

            return Math.Sqrt(rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double RootMeanSquareError(this IEnumerable<long?> expected, IEnumerable<long?> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            double rmse = 0.0;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                if(expecter.Current.HasValue && actualer.Current.HasValue) {
                    rmse += (expecter.Current.Value - actualer.Current.Value).Square();
                    n++;
                }
            }

            return Math.Sqrt(rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double RootMeanSquareError(this IEnumerable<int> expected, IEnumerable<int> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            double rmse = 0.0;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                rmse += (expecter.Current - actualer.Current).Square();
                n++;
            }

            return Math.Sqrt(rmse / (n - 1));
        }

        /// <summary>
        /// 제곱 평균(root-mean-square) error (RMSE) : 예측치와 실제값과의 오차를 제곱 평균으로 계산합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double RootMeanSquareError(this IEnumerable<int?> expected, IEnumerable<int?> actual) {
            expected.ShouldNotBeNull("expected");
            actual.ShouldNotBeNull("actual");

            Guard.Assert(expected.Count() == actual.Count(), "두 시퀀스의 항목 수가 틀립니다.");

            double rmse = 0.0;
            long n = 0;

            using(var expecter = expected.GetEnumerator())
            using(var actualer = actual.GetEnumerator()) {
                if(expecter.Current.HasValue && actualer.Current.HasValue) {
                    rmse += (expecter.Current.Value - actualer.Current.Value).Square();
                    n++;
                }
            }

            return Math.Sqrt(rmse / (n - 1));
        }

        //!+ ----------------------------------------------------------------------------------------

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double NormalizedRootMeanSquareError(this IEnumerable<double> expected, IEnumerable<double> actual) {
            double rmse = RootMeanSquareError(expected, actual);

            double min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double NormalizedRootMeanSquareError(this IEnumerable<double?> expected, IEnumerable<double?> actual) {
            double rmse = RootMeanSquareError(expected, actual);

            double min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static float NormalizedRootMeanSquareError(this IEnumerable<float> expected, IEnumerable<float> actual) {
            float rmse = RootMeanSquareError(expected, actual);

            float min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static float NormalizedRootMeanSquareError(this IEnumerable<float?> expected, IEnumerable<float?> actual) {
            float rmse = RootMeanSquareError(expected, actual);

            float min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static decimal NormalizedRootMeanSquareError(this IEnumerable<decimal> expected, IEnumerable<decimal> actual) {
            decimal rmse = RootMeanSquareError(expected, actual);

            decimal min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static decimal NormalizedRootMeanSquareError(this IEnumerable<decimal?> expected, IEnumerable<decimal?> actual) {
            decimal rmse = RootMeanSquareError(expected, actual);

            decimal min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double NormalizedRootMeanSquareError(this IEnumerable<long> expected, IEnumerable<long> actual) {
            double rmse = RootMeanSquareError(expected, actual);

            long min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double NormalizedRootMeanSquareError(this IEnumerable<long?> expected, IEnumerable<long?> actual) {
            double rmse = RootMeanSquareError(expected, actual);

            long min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double NormalizedRootMeanSquareError(this IEnumerable<int> expected, IEnumerable<int> actual) {
            double rmse = RootMeanSquareError(expected, actual);

            int min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }

        /// <summary>
        /// 정규화된 제곱 평균 - Normalized root-mean-square error (RMSE) : 예측치와 실제값과의 오차를 제곱평균으로 계산하고, 정규화합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Root_mean_square_error
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double NormalizedRootMeanSquareError(this IEnumerable<int?> expected, IEnumerable<int?> actual) {
            double rmse = RootMeanSquareError(expected, actual);

            int min, max;
            actual.GetMinMax(out min, out max);

            return rmse / (max - min);
        }
    }
}