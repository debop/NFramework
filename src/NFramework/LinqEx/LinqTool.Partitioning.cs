using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.LinqEx {
    // NOTE: PMS의 자원 분배 분포에 대한 예제용으로 만든 것입니다.

    public static partial class LinqTool {
        /// <summary>
        /// 기본 PartitionCount (=10)
        /// </summary>
        public const int DefaultPartitionCount = 10;

        public const double DefaultPartitionAmount = 100;

        /// <summary>
        /// 기본 Paritioning Data
        /// </summary>
        public static readonly IDictionary<PartitioningMethod, double[]> DefaultPartitioningArray
            = new Dictionary<PartitioningMethod, double[]>()
              {
                  { PartitioningMethod.HorizontaolUniform, new double[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 } },
                  { PartitioningMethod.VerticalBegin, new double[] { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                  { PartitioningMethod.VerticalEnd, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 } },
                  { PartitioningMethod.TriangleBegin, new double[] { 18, 17, 14, 13, 11, 9, 7, 6, 3, 2 } },
                  { PartitioningMethod.TriangleMiddle, new double[] { 2, 6, 10, 14, 18, 18, 14, 10, 6, 2 } },
                  { PartitioningMethod.TriangleEnd, new double[] { 2, 3, 6, 7, 9, 11, 13, 14, 17, 18 } },
                  { PartitioningMethod.TwoStepBegin, new double[] { 7, 7, 7, 7, 7, 13, 13, 13, 13, 13 } },
                  { PartitioningMethod.TwoStepEnd, new double[] { 13, 13, 13, 13, 13, 7, 7, 7, 7, 7 } },
                  { PartitioningMethod.TrapezoidMiddle, new double[] { 2, 7, 11, 15, 15, 15, 11, 7, 2 } },
                  { PartitioningMethod.Normal, new double[] { 1, 3, 8, 15, 23, 23, 15, 8, 3, 1 } },
                  { PartitioningMethod.ThreeMiddle, new double[] { 8, 8, 8, 13, 13, 13, 13, 8, 8, 8 } },
              };

        /// <summary>
        /// 분할 방법에 따라 총액(<paramref name="totalAmount"/>)을 분할 갯수(<paramref name="partitionCount"/>)만큼 분할합니다.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="partitionCount"></param>
        /// <param name="totalAmount"></param>
        /// <returns></returns>
        public static double[] Partitioning(this PartitioningMethod method, int partitionCount = DefaultPartitionCount,
                                            double totalAmount = DefaultPartitionAmount) {
            partitionCount.ShouldBePositive("partitionCouunt");
            totalAmount.ShouldBePositive("totalAmount");

            if(IsDebugEnabled)
                log.Debug("총액을 분배합니다. PartitioningMethod=[{0}], partitionCount=[{1}], totalAmount=[{2}]", method, partitionCount,
                          totalAmount);

            if(partitionCount == DefaultPartitionCount && Math.Abs(totalAmount - DefaultPartitionAmount) < double.Epsilon)
                return DefaultPartitioningArray[method];

            Guard.Assert(() => partitionCount % 2 == 0, "PartitionCount는 짝수여야 합니다. PartitionCount=[{0}]", partitionCount);

            double[] result;
            double a, b;

            var half = partitionCount / 2;
            var uniformParition = totalAmount / partitionCount;

            switch(method) {
                case PartitioningMethod.HorizontaolUniform:
                    result = Generate(partitionCount, x => uniformParition).ToArray<double>();
                    break;

                case PartitioningMethod.VerticalBegin:
                    result = new double[partitionCount];
                    result[0] = totalAmount;
                    break;

                case PartitioningMethod.VerticalEnd:
                    result = new double[partitionCount];
                    result[partitionCount - 1] = totalAmount;
                    break;

                case PartitioningMethod.TriangleBegin:
                    a = -16.0 / 9.0;
                    b = 18.0 * totalAmount / DefaultPartitionAmount;
                    result = Generate<double>(partitionCount, x => a * x + b).Select(y => Math.Round(y, 0)).ToArray<double>();
                    break;

                case PartitioningMethod.TriangleMiddle:
                    result = new double[partitionCount];
                    b = 2.0 * totalAmount / DefaultPartitionAmount;
                    var triangleMiddle = Generate<double>(half, x => 4 * x + b).Select(y => Math.Round(y, 0)).ToArray<double>();

                    Array.Copy(triangleMiddle, result, half);
                    Array.Copy(triangleMiddle.Reverse().ToArray<double>(), 0, result, half, half);

                    break;

                case PartitioningMethod.TriangleEnd:
                    a = 16.0 / 9.0;
                    b = 2.0 * totalAmount / DefaultPartitionAmount;
                    result = Generate<double>(partitionCount, x => a * x + b).Select(y => Math.Round(y, 0)).ToArray<double>();
                    break;

                case PartitioningMethod.TwoStepBegin:

                    Func<int, double> @twostepBegin = x => (x < half) ? 1.3 * uniformParition : 0.7 * uniformParition;
                    result = Generate<double>(partitionCount, x => @twostepBegin(x)).Select(y => Math.Round(y, 0)).ToArray<double>();
                    break;

                case PartitioningMethod.TwoStepEnd:
                    Func<int, double> @twostepEnd = x => (x < half) ? 0.7 * uniformParition : 1.3 * uniformParition;
                    result = Generate<double>(partitionCount, x => @twostepEnd(x)).Select(y => Math.Round(y, 0)).ToArray<double>();
                    break;

                case PartitioningMethod.TrapezoidMiddle:
                    result = new double[partitionCount];
                    b = 2.0 * totalAmount / DefaultPartitionAmount;
                    var trapezoid = Generate<double>(half - 1, x => 13.0 / 3.0 * x + b).Select(y => Math.Round(y, 0)).ToArray<double>();

                    Array.Copy(trapezoid, result, half - 1);
                    Array.Copy(trapezoid.Reverse().ToArray(), 0, result, half + 1, half - 1);
                    result[half] = result[half - 1];
                    break;

                case PartitioningMethod.Normal:

                    result =
                        Generate<double>(partitionCount, x => totalAmount * NormalDistribution(4.0, half, x + 1)).Select(
                            y => Math.Round(y, 0)).ToArray<double>();
                    break;

                case PartitioningMethod.ThreeMiddle:

                    var countByThree = 0.3 * partitionCount;
                    var countBySeven = 0.7 * partitionCount;
                    Func<int, double> @treeMiddle =
                        x => (x <= countByThree || x >= countBySeven) ? 0.8 * uniformParition : 1.3 * uniformParition;

                    result = Generate<double>(partitionCount, x => @treeMiddle(x)).Select(y => Math.Round(y, 0)).ToArray<double>();
                    break;

                default:
                    throw new NotSupportedException(string.Format("지원하지 않는 분할 방식입니다. method=[{0}]", method));
            }

            return result;
        }

        private static double NormalDistribution(double stdev, double mean, double x) {
            var a = 1.0 / Math.Sqrt(2.0 * Math.PI * stdev * stdev);
            var b = -(x - mean) * (x - mean) / (2.0 * stdev);

            return a * Math.Exp(b);
        }
    }
}