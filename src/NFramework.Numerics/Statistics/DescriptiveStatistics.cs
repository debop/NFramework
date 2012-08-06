using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Statistics {
    /// <summary>
    /// 변량에 대한 기본 통계 정보를 한번에 계산합니다.
    /// 계산하는 정보로는 평균, 분산, 표준편차, 분포의 기울기, 중앙값, 최빈값, 첨예도, 최대/최소값 등을 한꺼번에 계산합니다.
    /// </summary>
    public sealed class DescriptiveStatistics {
        public DescriptiveStatistics(IEnumerable<double> source) : this(source, false) {}
        public DescriptiveStatistics(IEnumerable<double?> source) : this(source, false) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptiveStatistics"/> class. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="highAccuracy"></param>
        public DescriptiveStatistics(IEnumerable<double> source, bool highAccuracy) {
            if(highAccuracy)
                ComputeHighAccuracy(source);
            else
                Compute(source);

            Median = source.Median();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptiveStatistics"/> class. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="highAccuracy"></param>
        public DescriptiveStatistics(IEnumerable<double?> source, bool highAccuracy) {
            if(highAccuracy)
                ComputeHighAccuracy(source);
            else
                Compute(source);

            Median = source.Median();
        }

        /// <summary>
        /// Gets the size of the sample.
        /// </summary>
        /// <value>The size of the sample.</value>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the sample mean.
        /// </summary>
        /// <value>The sample mean.</value>
        public double Mean { get; private set; }

        /// <summary>
        /// Gets the sample variance.
        /// </summary>
        /// <value>The sample variance.</value>
        public double Variance { get; private set; }

        /// <summary>
        /// Gets the sample standard deviation.
        /// </summary>
        /// <value>The sample standard deviation.</value>
        public double StandardDeviation { get; private set; }

        /// <summary>
        /// Gets the sample skewness.
        /// </summary>
        /// <value>The sample skewness.</value>
        /// <remarks>Returns zero if <see cref="Count"/> is less than three. </remarks>
        public double Skewness { get; private set; }

        /// <summary>
        /// Gets the sample median.
        /// </summary>
        /// <value>The sample median.</value>
        public double Median { get; private set; }

        /// <summary>
        /// Gets the sample kurtosis.
        /// </summary>
        /// <value>The sample kurtosis.</value>
        /// <remarks>Returns zero if <see cref="Count"/> is less than four. </remarks>
        public double Kurtosis { get; private set; }

        /// <summary>
        /// Gets the maximum sample value.
        /// </summary>
        /// <value>The maximum sample value.</value>
        public double Maximum { get; private set; }

        /// <summary>
        /// Gets the minimum sample value.
        /// </summary>
        /// <value>The minimum sample value.</value>
        public double Minimum { get; private set; }

        /// <summary>
        /// 변량으로부터 대표적 통계 값들을 계산합니다.
        /// </summary>
        /// <param name="source"></param>
        private void Compute(IEnumerable<double> source) {
            Mean = source.Mean();
            double variance = 0;
            double correction = 0;
            double skewness = 0;
            double kurtosis = 0;
            double minimum = Double.PositiveInfinity;
            double maximum = Double.NegativeInfinity;
            int n = 0;
            foreach(var xi in source) {
                double diff = xi - Mean;
                correction += diff;
                double tmp = diff * diff;
                variance += tmp;
                tmp *= diff;
                skewness += tmp;
                tmp *= diff;
                kurtosis += tmp;
                if(minimum > xi) {
                    minimum = xi;
                }
                if(maximum < xi) {
                    maximum = xi;
                }
                n++;
            }

            Count = n;
            Minimum = minimum;
            Maximum = maximum;
            Variance = (variance - (correction * correction / n)) / (n - 1);
            StandardDeviation = System.Math.Sqrt(Variance);

            if(Variance.ApproximateEqual(0.0) == false) {
                if(n > 2) {
                    Skewness = (double)n / ((n - 1) * (n - 2)) * (skewness / (Variance * StandardDeviation));
                }

                if(n > 3) {
                    Kurtosis = (((double)n * (n + 1))
                                / ((n - 1) * (n - 2) * (n - 3))
                                * (kurtosis / (Variance * Variance)))
                               - ((3.0 * (n - 1) * (n - 1)) / ((n - 2) * (n - 3)));
                }
            }
        }

        /// <summary>
        /// 변량으로부터 대표적 통계 값들을 계산합니다.
        /// </summary>
        /// <param name="source"></param>
        private void Compute(IEnumerable<double?> source) {
            Mean = source.Mean();
            double variance = 0;
            double correction = 0;
            double skewness = 0;
            double kurtosis = 0;
            double minimum = Double.PositiveInfinity;
            double maximum = Double.NegativeInfinity;
            int n = 0;
            foreach(var xi in source) {
                if(xi.HasValue) {
                    double diff = xi.Value - Mean;
                    double tmp = diff * diff;
                    correction += diff;
                    variance += tmp;
                    tmp *= diff;
                    skewness += tmp;
                    tmp *= diff;
                    kurtosis += tmp;
                    if(minimum > xi) {
                        minimum = xi.Value;
                    }
                    if(maximum < xi) {
                        maximum = xi.Value;
                    }
                    n++;
                }
            }

            Count = n;
            if(n > 0) {
                Minimum = minimum;
                Maximum = maximum;
                Variance = (variance - (correction * correction / n)) / (n - 1);
                StandardDeviation = System.Math.Sqrt(Variance);

                if(Variance.ApproximateEqual(0.0) == false) {
                    if(n > 2) {
                        Skewness = (double)n / ((n - 1) * (n - 2)) * (skewness / (Variance * StandardDeviation));
                    }

                    if(n > 3) {
                        Kurtosis = (((double)n * (n + 1))
                                    / ((n - 1) * (n - 2) * (n - 3))
                                    * (kurtosis / (Variance * Variance)))
                                   - ((3.0 * (n - 1) * (n - 1)) / ((n - 2) * (n - 3)));
                    }
                }
            }
        }

        /// <summary>
        /// 변량으로부터 대표적 통계 값들을 계산합니다.
        /// </summary>
        /// <param name="source"></param>
        private void ComputeHighAccuracy(IEnumerable<double> source) {
            Mean = source.Mean();
            decimal mean = (decimal)Mean;
            decimal variance = 0;
            decimal correction = 0;
            decimal skewness = 0;
            decimal kurtosis = 0;
            decimal minimum = Decimal.MaxValue;
            decimal maximum = Decimal.MinValue;
            int n = 0;
            foreach(decimal xi in source) {
                decimal diff = xi - mean;
                decimal tmp = diff * diff;
                correction += diff;
                variance += tmp;
                tmp *= diff;
                skewness += tmp;
                tmp *= diff;
                kurtosis += tmp;
                if(minimum > xi) {
                    minimum = xi;
                }
                if(maximum < xi) {
                    maximum = xi;
                }
                n++;
            }

            Count = n;
            Minimum = (double)minimum;
            Maximum = (double)maximum;
            Variance = (double)(variance - (correction * correction / n)) / (n - 1);
            StandardDeviation = Math.Sqrt(Variance);

            if(Variance.ApproximateEqual(0.0) == false) {
                if(n > 2) {
                    Skewness = (double)n / ((n - 1) * (n - 2)) * ((double)skewness / (Variance * StandardDeviation));
                }

                if(n > 3) {
                    Kurtosis = (((double)n * (n + 1))
                                / ((n - 1) * (n - 2) * (n - 3))
                                * ((double)kurtosis / (Variance * Variance)))
                               - ((3.0 * (n - 1) * (n - 1)) / ((n - 2) * (n - 3)));
                }
            }
        }

        /// <summary>
        /// 변량으로부터 대표적 통계 값들을 계산합니다.
        /// </summary>
        /// <param name="source"></param>
        private void ComputeHighAccuracy(IEnumerable<double?> source) {
            Mean = source.Mean();

            decimal mean = (decimal)Mean;
            decimal variance = 0;
            decimal correction = 0;
            decimal skewness = 0;
            decimal kurtosis = 0;
            decimal minimum = Decimal.MaxValue;
            decimal maximum = Decimal.MinValue;
            int n = 0;
            foreach(decimal? xi in source) {
                if(xi.HasValue) {
                    decimal diff = xi.Value - mean;
                    decimal tmp = diff * diff;
                    correction += diff;
                    variance += tmp;
                    tmp *= diff;
                    skewness += tmp;
                    tmp *= diff;
                    kurtosis += tmp;
                    if(minimum > xi) {
                        minimum = xi.Value;
                    }
                    if(maximum < xi) {
                        maximum = xi.Value;
                    }
                    n++;
                }
            }

            Count = n;
            if(n > 0) {
                Minimum = (double)minimum;
                Maximum = (double)maximum;
                Variance = (double)(variance - (correction * correction / n)) / (n - 1);
                StandardDeviation = System.Math.Sqrt(Variance);

                if(Variance.ApproximateEqual(0.0) == false) {
                    if(n > 2) {
                        Skewness = (double)n / ((n - 1) * (n - 2)) * ((double)skewness / (Variance * StandardDeviation));
                    }

                    if(n > 3) {
                        Kurtosis = (((double)n * (n + 1))
                                    / ((n - 1) * (n - 2) * (n - 3))
                                    * ((double)kurtosis / (Variance * Variance)))
                                   - ((3.0 * (n - 1) * (n - 1)) / ((n - 2) * (n - 3)));
                    }
                }
            }
        }
    }
}