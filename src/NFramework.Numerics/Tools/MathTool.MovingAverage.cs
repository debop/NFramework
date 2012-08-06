using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        #region << 표준 이동평균 (Standard Moving Average) >>

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> StandardMovingAverage(this IEnumerable<double> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var sum = 0.0d;
            var block = blockSize;
            var nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                double value;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(nans > 0)
                            yield return double.NaN;
                        else
                            yield return sum / (blockSize - block - 1);
                        yield break;
                    }

                    value = right.Current;

                    if(double.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                }

                while(right.MoveNext()) {
                    value = right.Current;

                    if(double.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                    if(nans > 0)
                        yield return double.NaN;
                    else
                        yield return sum / blockSize;

                    left.MoveNext();

                    value = left.Current;
                    if(double.IsNaN(value) == false)
                        sum -= value;
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> StandardMovingAverage(this IEnumerable<double?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var elements = 0;
            var nans = -1;
            double sum = 0;
            double? curr;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(nans > 0)
                            yield return double.NaN;
                        else if(elements > 0)
                            yield return (double)sum / elements;
                        else
                            yield return null;
                        yield break;
                    }
                    curr = right.Current;

                    if(curr.HasValue) {
                        if(double.IsNaN(curr.Value))
                            nans = blockSize;
                        else {
                            sum += curr.Value;
                            nans--;
                        }
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current;

                    if(curr.HasValue) {
                        if(double.IsNaN(curr.Value))
                            nans = blockSize;
                        else {
                            sum += curr.Value;
                            nans--;
                        }
                        elements++;
                    }

                    if(nans > 0)
                        yield return double.NaN;
                    else if(elements > 0)
                        yield return (double)sum / elements;
                    else
                        yield return null;

                    left.MoveNext();
                    curr = left.Current;
                    if(curr.HasValue) {
                        if(!double.IsNaN(curr.Value))
                            sum -= curr.Value;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<float> StandardMovingAverage(this IEnumerable<float> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var sum = 0.0f;
            var block = blockSize;
            var nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                float value;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(nans > 0)
                            yield return float.NaN;
                        else
                            yield return sum / (blockSize - block - 1);
                        yield break;
                    }

                    value = right.Current;

                    if(float.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                }

                while(right.MoveNext()) {
                    value = right.Current;

                    if(float.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                    if(nans > 0)
                        yield return float.NaN;
                    else
                        yield return sum / blockSize;

                    left.MoveNext();

                    value = left.Current;
                    if(float.IsNaN(value) == false)
                        sum -= value;
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<float?> StandardMovingAverage(this IEnumerable<float?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var elements = 0;
            var nans = -1;
            float sum = 0;
            float? curr;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(nans > 0)
                            yield return float.NaN;
                        else if(elements > 0)
                            yield return (float)sum / elements;
                        else
                            yield return null;
                        yield break;
                    }
                    curr = right.Current;
                    if(curr.HasValue) {
                        if(float.IsNaN(curr.Value))
                            nans = blockSize;
                        else {
                            sum += curr.Value;
                            nans--;
                        }
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    if(curr.HasValue) {
                        if(float.IsNaN(curr.Value))
                            nans = blockSize;
                        else {
                            sum += curr.Value;
                            nans--;
                        }
                        elements++;
                    }

                    if(nans > 0)
                        yield return float.NaN;
                    else if(elements > 0)
                        yield return (float)sum / elements;
                    else
                        yield return null;
                    left.MoveNext();
                    curr = left.Current;
                    if(curr.HasValue) {
                        if(!float.IsNaN(curr.Value))
                            sum -= curr.Value;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<decimal> StandardMovingAverage(this IEnumerable<decimal> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var sum = 0m;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum / (blockSize - block - 1);
                        yield break;
                    }
                    sum += right.Current;
                }

                while(right.MoveNext()) {
                    sum += right.Current;
                    yield return sum / blockSize;
                    left.MoveNext();
                    sum -= left.Current;
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<decimal?> StandardMovingAverage(this IEnumerable<decimal?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var elements = 0;
            var sum = 0m;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                decimal? curr;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(elements > 0)
                            yield return sum / elements;
                        else
                            yield return null;
                        yield break;
                    }
                    curr = right.Current;
                    if(curr.HasValue) {
                        sum += curr.Value;
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    if(curr.HasValue) {
                        sum += curr.Value;
                        elements++;
                    }

                    if(elements > 0)
                        yield return sum / elements;
                    else
                        yield return null;
                    left.MoveNext();
                    curr = left.Current;
                    if(curr.HasValue) {
                        sum -= curr.Value;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> StandardMovingAverage(this IEnumerable<long> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var sum = 0d;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum / (blockSize - block - 1);
                        yield break;
                    }
                    sum += right.Current;
                }

                while(right.MoveNext()) {
                    sum += right.Current;
                    yield return sum / blockSize;
                    left.MoveNext();
                    sum -= left.Current;
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> StandardMovingAverage(this IEnumerable<long?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var elements = 0;
            var sum = 0d;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                long? curr;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(elements > 0)
                            yield return sum / elements;
                        else
                            yield return null;
                        yield break;
                    }
                    curr = right.Current;
                    if(curr.HasValue) {
                        sum += curr.Value;
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    if(curr.HasValue) {
                        sum += curr.Value;
                        elements++;
                    }

                    if(elements > 0)
                        yield return sum / elements;
                    else
                        yield return null;
                    left.MoveNext();
                    curr = left.Current;
                    if(curr.HasValue) {
                        sum -= curr.Value;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> StandardMovingAverage(this IEnumerable<int> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var sum = 0d;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum / (blockSize - block - 1);
                        yield break;
                    }
                    sum += right.Current;
                }

                while(right.MoveNext()) {
                    sum += right.Current;
                    yield return sum / blockSize;
                    left.MoveNext();
                    sum -= left.Current;
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> StandardMovingAverage(this IEnumerable<int?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var elements = 0;
            var sum = 0d;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                int? curr;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(elements > 0)
                            yield return sum / elements;
                        else
                            yield return null;
                        yield break;
                    }
                    curr = right.Current;
                    if(curr.HasValue) {
                        sum += curr.Value;
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current;
                    if(curr.HasValue) {
                        sum += curr.Value;
                        elements++;
                    }

                    if(elements > 0)
                        yield return sum / elements;
                    else
                        yield return null;
                    left.MoveNext();
                    curr = left.Current;
                    if(curr.HasValue) {
                        sum -= curr.Value;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, double> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, double?> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<float> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, float> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<float?> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, float?> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<decimal> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, decimal> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<decimal?> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                     Func<T, decimal?> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, long> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, long?> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, int> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        /// <summary>
        /// 표준 방식으로 이동평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 포함한 시퀀스</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> StandardMovingAverage<T>(this IEnumerable<T> source, int blockSize, Func<T, int?> selector) {
            return source.Select(x => selector(x)).StandardMovingAverage(blockSize);
        }

        #endregion

        #region << 지수 이동평균 (Exponential Moving Average >>

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> ExponentialMovingAverage(this IEnumerable<double> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var sum = 0.0d;
            var block = blockSize;
            var nans = -1;

            double factor = 2.0 / (blockSize + 1);
            double prevAvg = 0.0;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                double value;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(nans > 0)
                            yield return double.NaN;
                        else {
                            prevAvg = sum / (blockSize - block - 1);
                            yield return prevAvg;
                        }
                        yield break;
                    }

                    value = right.Current;

                    if(double.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                }

                while(right.MoveNext()) {
                    value = right.Current;

                    if(double.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                    if(nans > 0)
                        yield return double.NaN;
                    else {
                        var result = factor * (value - prevAvg) + prevAvg;
                        yield return result;
                        prevAvg = result;
                    }

                    left.MoveNext();

                    value = left.Current;
                    if(double.IsNaN(value) == false)
                        sum -= value;
                }
            }
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> ExponentialMovingAverage(this IEnumerable<double?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var elements = 0;
            var nans = -1;
            double sum = 0;
            double? curr;

            double factor = 2.0 / (blockSize + 1);
            double? prevAvg = 0.0;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(nans > 0)
                            yield return double.NaN;
                        else if(elements > 0) {
                            prevAvg = sum / elements;
                            yield return prevAvg;
                        }
                        else
                            yield return null;
                        yield break;
                    }
                    curr = right.Current;

                    if(curr.HasValue) {
                        if(double.IsNaN(curr.Value))
                            nans = blockSize;
                        else {
                            sum += curr.Value;
                            nans--;
                        }
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current;

                    if(curr.HasValue) {
                        if(double.IsNaN(curr.Value))
                            nans = blockSize;
                        else {
                            sum += curr.Value;
                            nans--;
                        }
                        elements++;
                    }

                    if(nans > 0)
                        yield return double.NaN;
                    else if(elements > 0) {
                        double? result = factor * (curr - prevAvg) + prevAvg;
                        yield return result;
                        prevAvg = result;
                    }
                    else
                        yield return null;

                    left.MoveNext();
                    curr = left.Current;
                    if(curr.HasValue) {
                        if(!double.IsNaN(curr.Value))
                            sum -= curr.Value;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<float> ExponentialMovingAverage(this IEnumerable<float> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            float sum = 0.0f;
            int block = blockSize;
            int nans = -1;

            float factor = 2.0f / (blockSize + 1);
            float prevAvg = 0.0f;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                float value;

                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(nans > 0)
                            yield return float.NaN;
                        else {
                            prevAvg = sum / (blockSize - block - 1);
                            yield return prevAvg;
                        }
                        yield break;
                    }

                    value = right.Current;

                    if(float.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                }

                while(right.MoveNext()) {
                    value = right.Current;

                    if(float.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                    if(nans > 0)
                        yield return float.NaN;
                    else {
                        var result = factor * (value - prevAvg) + prevAvg;
                        yield return result;
                        prevAvg = result;
                    }

                    left.MoveNext();

                    value = left.Current;
                    if(double.IsNaN(value) == false)
                        sum -= value;
                }
            }
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<float?> ExponentialMovingAverage(this IEnumerable<float?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var block = blockSize;
            var elements = 0;
            var nans = -1;
            float sum = 0;
            float? curr;

            float factor = 2.0f / (blockSize + 1);
            float? prevAvg = 0.0f;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        if(nans > 0)
                            yield return float.NaN;
                        else if(elements > 0) {
                            prevAvg = sum / elements;
                            yield return prevAvg;
                        }
                        else
                            yield return null;
                        yield break;
                    }
                    curr = right.Current;

                    if(curr.HasValue) {
                        if(float.IsNaN(curr.Value))
                            nans = blockSize;
                        else {
                            sum += curr.Value;
                            nans--;
                        }
                        elements++;
                    }
                }

                while(right.MoveNext()) {
                    curr = right.Current;

                    if(curr.HasValue) {
                        if(float.IsNaN(curr.Value))
                            nans = blockSize;
                        else {
                            sum += curr.Value;
                            nans--;
                        }
                        elements++;
                    }

                    if(nans > 0)
                        yield return float.NaN;
                    else if(elements > 0) {
                        float? result = factor * (curr - prevAvg) + prevAvg;
                        yield return result;
                        prevAvg = result;
                    }
                    else
                        yield return null;

                    left.MoveNext();
                    curr = left.Current;
                    if(curr.HasValue) {
                        if(!float.IsNaN(curr.Value))
                            sum -= curr.Value;
                        elements--;
                    }
                }
            }
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<decimal> ExponentialMovingAverage(this IEnumerable<decimal> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            return
                source
                    .Select(x => (double)x)
                    .ExponentialMovingAverage(blockSize)
                    .Select(d => double.IsNaN(d) ? decimal.MinValue : (decimal)d);
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<decimal?> ExponentialMovingAverage(this IEnumerable<decimal?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            return
                source
                    .Select(x => (double?)x)
                    .ExponentialMovingAverage(blockSize)
                    .Select(d => d.HasValue ? (double.IsNaN(d.Value) ? decimal.MinValue : (decimal?)d) : null);
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> ExponentialMovingAverage(this IEnumerable<long> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            return
                source
                    .Select(x => (double)x)
                    .ExponentialMovingAverage(blockSize);
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> ExponentialMovingAverage(this IEnumerable<long?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            return
                source
                    .Select(x => (double?)x)
                    .ExponentialMovingAverage(blockSize);
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double> ExponentialMovingAverage(this IEnumerable<int> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            return
                source
                    .Select(x => (double)x)
                    .ExponentialMovingAverage(blockSize);
        }

        /// <summary>
        /// 지수 방식으로 이동평균을 구합니다. (표준방식보다 부드러운 곡선을 만듭니다)
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동 평균을 계산하기 위한 항목 수 (최소 2)</param>
        /// <returns>이동 평균</returns>
        public static IEnumerable<double?> ExponentialMovingAverage(this IEnumerable<int?> source, int blockSize = BlockSize) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            return
                source
                    .Select(x => (double?)x)
                    .ExponentialMovingAverage(blockSize);
        }

        #endregion

        #region << 누적 이동평균 (Cumulative Moving Average) >>

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double> CumulativeMovingAverage(this IEnumerable<double> source) {
            var sum = 0d;
            var idx = 0;

            return source.Select(x => (sum += x) / ++idx);
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double?> CumulativeMovingAverage(this IEnumerable<double?> source) {
            var sum = 0d;
            var idx = 0;

            return
                source
                    .Where(x => x.HasValue)
                    .Select(x => (double?)((sum += x.Value) / ++idx));
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<float> CumulativeMovingAverage(this IEnumerable<float> source) {
            var sum = 0f;
            var idx = 0;

            return source.Select(x => (sum += x) / ++idx);
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<float?> CumulativeMovingAverage(this IEnumerable<float?> source) {
            var sum = 0f;
            var idx = 0;

            return
                source
                    .Where(x => x.HasValue)
                    .Select(x => (float?)((sum += x.Value) / ++idx));
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<decimal> CumulativeMovingAverage(this IEnumerable<decimal> source) {
            var sum = 0m;
            var idx = 0;

            return source.Select(x => (sum += x) / ++idx);
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<decimal?> CumulativeMovingAverage(this IEnumerable<decimal?> source) {
            var sum = 0m;
            var idx = 0;

            return
                source
                    .Where(x => x.HasValue)
                    .Select(x => (decimal?)((sum += x.Value) / ++idx));
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double> CumulativeMovingAverage(this IEnumerable<long> source) {
            var sum = 0d;
            var idx = 0;

            return source.Select(x => (sum += x) / ++idx);
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double?> CumulativeMovingAverage(this IEnumerable<long?> source) {
            var sum = 0d;
            var idx = 0;

            return
                source
                    .Where(x => x.HasValue)
                    .Select(x => (double?)((sum += x.Value) / ++idx));
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double> CumulativeMovingAverage(this IEnumerable<int> source) {
            var sum = 0d;
            var idx = 0;

            return source.Select(x => (sum += x) / ++idx);
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double?> CumulativeMovingAverage(this IEnumerable<int?> source) {
            var sum = 0d;
            var idx = 0;

            return
                source
                    .Where(x => x.HasValue)
                    .Select(x => (double?)((sum += x.Value) / ++idx));
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double?> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, double?> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<float> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<float?> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, float?> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<decimal> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<decimal?> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, decimal?> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, long> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double?> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, long?> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, int> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        /// <summary>
        /// 누적 이동 평균을 구합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="selector">항목에서 변량을 선택하는 선택자</param>
        /// <returns>누적 이동평균</returns>
        public static IEnumerable<double?> CumulativeMovingAverage<T>(this IEnumerable<T> source, Func<T, int?> selector) {
            return source.Select(x => selector(x)).CumulativeMovingAverage();
        }

        #endregion

        #region << 가중 이동평균 (Weighted Moving Average) >>

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage(this IEnumerable<double> source, int blockSize,
                                                                Func<int, double> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<double>(blockSize);
            var factors = new double[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }

                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);
                    yield return queue.Multiply(factors).Sum() / factorSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage(this IEnumerable<double?> source, int blockSize,
                                                                 Func<int, double> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<double?>();
            var factors = new double?[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }
                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorFullSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);

                    double? tempSum;

                    if(queue.All(x => x.HasValue == false)) {
                        tempSum = null;
                    }
                    else if(queue.Any(x => x.HasValue == false)) {
                        var map = queue.Divide(queue);
                        tempSum = map.Multiply(factors).Sum();
                    }
                    else {
                        tempSum = factorFullSum;
                    }

                    yield return queue.Multiply(factors).Sum() / tempSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<float> WeightedMovingAverage(this IEnumerable<float> source, int blockSize,
                                                               Func<int, float> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<float>(blockSize);
            var factors = new float[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }

                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);
                    yield return queue.Multiply(factors).Sum() / factorSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<float?> WeightedMovingAverage(this IEnumerable<float?> source, int blockSize,
                                                                Func<int, float> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<float?>();
            var factors = new float?[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }

                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorFullSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);

                    float? tempSum;
                    if(queue.All(x => x.HasValue == false)) {
                        tempSum = null;
                    }
                    else if(queue.Any(x => x.HasValue == false)) {
                        var map = queue.Divide(queue);
                        tempSum = map.Multiply(factors).Sum();
                    }
                    else {
                        tempSum = factorFullSum;
                    }

                    yield return queue.Multiply(factors).Sum() / tempSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<decimal> WeightedMovingAverage(this IEnumerable<decimal> source, int blockSize,
                                                                 Func<int, decimal> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<decimal>(blockSize);
            var factors = new decimal[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }

                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);
                    yield return queue.Multiply(factors).Sum() / factorSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<decimal?> WeightedMovingAverage(this IEnumerable<decimal?> source, int blockSize,
                                                                  Func<int, decimal> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<decimal?>();
            var factors = new decimal?[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }

                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorFullSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);

                    decimal? tempSum;
                    if(queue.All(x => x.HasValue == false)) {
                        tempSum = null;
                    }
                    else if(queue.Any(x => x.HasValue == false)) {
                        var map = queue.Divide(queue);
                        tempSum = map.Multiply(factors).Sum();
                    }
                    else {
                        tempSum = factorFullSum;
                    }

                    yield return queue.Multiply(factors).Sum() / tempSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage(this IEnumerable<long> source, int blockSize,
                                                                Func<int, double> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<double>(blockSize);
            var factors = new double[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }

                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);
                    yield return queue.Multiply(factors).Sum() / factorSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage(this IEnumerable<long?> source, int blockSize,
                                                                 Func<int, double> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<double?>();
            var factors = new double?[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }
                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorFullSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);

                    double? tempSum;

                    if(queue.All(x => x.HasValue == false)) {
                        tempSum = null;
                    }
                    else if(queue.Any(x => x.HasValue == false)) {
                        var map = queue.Divide(queue);
                        tempSum = map.Multiply(factors).Sum();
                    }
                    else {
                        tempSum = factorFullSum;
                    }

                    yield return queue.Multiply(factors).Sum() / tempSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage(this IEnumerable<int> source, int blockSize,
                                                                Func<int, double> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<double>(blockSize);
            var factors = new double[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }

                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);
                    yield return queue.Multiply(factors).Sum() / factorSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage(this IEnumerable<int?> source, int blockSize,
                                                                 Func<int, double> weightingFunc) {
            blockSize.ShouldBeGreaterOrEqual(2, "blockSize");

            var queue = new Queue<double?>();
            var factors = new double?[blockSize];

            using(var iter = source.GetEnumerator()) {
                for(var i = 0; i < blockSize - 1; i++) {
                    Guard.Assert(iter.MoveNext(), "시퀀스의 항목 수가 blockSize[{0}] 보다 커야합니다.", blockSize);
                    queue.Enqueue(iter.Current);
                    factors[i] = weightingFunc(i + 1);
                }
                factors[blockSize - 1] = weightingFunc(blockSize);
                var factorFullSum = factors.Sum();

                while(iter.MoveNext()) {
                    queue.Enqueue(iter.Current);

                    double? tempSum;

                    if(queue.All(x => x.HasValue == false)) {
                        tempSum = null;
                    }
                    else if(queue.Any(x => x.HasValue == false)) {
                        var map = queue.Divide(queue);
                        tempSum = map.Multiply(factors).Sum();
                    }
                    else {
                        tempSum = factorFullSum;
                    }

                    yield return queue.Multiply(factors).Sum() / tempSum;
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage(this IEnumerable<double> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (double)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage(this IEnumerable<double?> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (double)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<float> WeightedMovingAverage(this IEnumerable<float> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (float)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<float?> WeightedMovingAverage(this IEnumerable<float?> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (float)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<decimal> WeightedMovingAverage(this IEnumerable<decimal> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (decimal)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<decimal?> WeightedMovingAverage(this IEnumerable<decimal?> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (decimal)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage(this IEnumerable<long> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (double)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage(this IEnumerable<long?> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (double)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage(this IEnumerable<int> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (double)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage(this IEnumerable<int?> source, int blockSize = BlockSize) {
            return source.WeightedMovingAverage(blockSize, x => (double)x);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                   Func<int, double> weightingFunc, Func<T, double> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                    Func<int, double> weightingFunc, Func<T, double?> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<float> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                  Func<int, float> weightingFunc, Func<T, float> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<float?> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                   Func<int, float> weightingFunc, Func<T, float?> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<decimal> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                    Func<int, decimal> weightingFunc, Func<T, decimal> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<decimal?> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                     Func<int, decimal> weightingFunc, Func<T, decimal?> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                   Func<int, double> weightingFunc, Func<T, long> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                    Func<int, double> weightingFunc, Func<T, long?> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                   Func<int, double> weightingFunc, Func<T, int> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        /// <summary>
        /// 지정한 시퀀스의 항목에 가중치를 준 이동평균을 계산합니다.
        /// </summary>
        /// <param name="source">변량을 가진 항목의 시퀀스</param>
        /// <param name="blockSize">이동평균 계산시 변량 수</param>
        /// <param name="weightingFunc">가중치 함수</param>
        /// <param name="selector">항목에서 변량을 선택하는 함수</param>
        /// <returns>가중치가 적용된 이동평균</returns>
        public static IEnumerable<double?> WeightedMovingAverage<T>(this IEnumerable<T> source, int blockSize,
                                                                    Func<int, double> weightingFunc, Func<T, int?> selector) {
            return source.Select(x => selector(x)).WeightedMovingAverage(blockSize, weightingFunc);
        }

        #endregion
    }
}