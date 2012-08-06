using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<double> MovingSum(this IEnumerable<double> source, int blockSize = BlockSize) {
            var sum = 0d;
            var block = blockSize;
            var nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                double value;
                while(block > 1) {
                    block--;

                    if(right.MoveNext() == false) {
                        if(nans > 0)
                            yield return double.NaN;
                        else
                            yield return sum;

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
                        yield return sum;

                    left.MoveNext();
                    value = left.Current;

                    if(double.IsNaN(value) == false)
                        sum -= value;
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<double?> MovingSum(this IEnumerable<double?> source, int blockSize = BlockSize) {
            var sum = 0d;
            var block = blockSize;
            var nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                double value;
                while(block > 1) {
                    block--;

                    if(right.MoveNext() == false) {
                        if(nans > 0)
                            yield return double.NaN;
                        else
                            yield return sum;

                        yield break;
                    }
                    value = right.Current.GetValueOrDefault();

                    if(double.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                }
                while(right.MoveNext()) {
                    value = right.Current.GetValueOrDefault();

                    if(double.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }

                    if(nans > 0)
                        yield return double.NaN;
                    else
                        yield return sum;

                    left.MoveNext();
                    value = left.Current.GetValueOrDefault();

                    if(double.IsNaN(value) == false)
                        sum -= value;
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<float> MovingSum(this IEnumerable<float> source, int blockSize = BlockSize) {
            var sum = 0f;
            var block = blockSize;
            var nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                float value;
                while(block > 1) {
                    block--;

                    if(right.MoveNext() == false) {
                        if(nans > 0)
                            yield return float.NaN;
                        else
                            yield return sum;

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
                        yield return sum;

                    left.MoveNext();
                    value = left.Current;

                    if(float.IsNaN(value) == false)
                        sum -= value;
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<float?> MovingSum(this IEnumerable<float?> source, int blockSize = BlockSize) {
            var sum = 0f;
            var block = blockSize;
            var nans = -1;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                float value;
                while(block > 1) {
                    block--;

                    if(right.MoveNext() == false) {
                        if(nans > 0)
                            yield return float.NaN;
                        else
                            yield return sum;

                        yield break;
                    }
                    value = right.Current.GetValueOrDefault();

                    if(float.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }
                }
                while(right.MoveNext()) {
                    value = right.Current.GetValueOrDefault();

                    if(float.IsNaN(value))
                        nans = blockSize;
                    else {
                        sum += value;
                        nans--;
                    }

                    if(nans > 0)
                        yield return float.NaN;
                    else
                        yield return sum;

                    left.MoveNext();
                    value = left.Current.GetValueOrDefault();

                    if(float.IsNaN(value) == false)
                        sum -= value;
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<decimal> MovingSum(this IEnumerable<decimal> source, int blockSize = BlockSize) {
            var sum = 0m;
            var block = blockSize;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum;
                        yield break;
                    }
                    sum += right.Current;
                }

                while(right.MoveNext()) {
                    sum += right.Current;
                    yield return sum;
                    left.MoveNext();
                    sum -= left.Current;
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<decimal?> MovingSum(this IEnumerable<decimal?> source, int blockSize = BlockSize) {
            var sum = 0m;
            var block = blockSize;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum;
                        yield break;
                    }
                    sum += right.Current.GetValueOrDefault();
                }

                while(right.MoveNext()) {
                    sum += right.Current.GetValueOrDefault();
                    yield return sum;
                    left.MoveNext();
                    sum -= left.Current.GetValueOrDefault();
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<long> MovingSum(this IEnumerable<long> source, int blockSize = BlockSize) {
            var sum = 0L;
            var block = blockSize;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum;
                        yield break;
                    }
                    sum += right.Current;
                }

                while(right.MoveNext()) {
                    sum += right.Current;
                    yield return sum;
                    left.MoveNext();
                    sum -= left.Current;
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<long?> MovingSum(this IEnumerable<long?> source, int blockSize = BlockSize) {
            var sum = 0L;
            var block = blockSize;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum;
                        yield break;
                    }
                    sum += right.Current.GetValueOrDefault();
                }

                while(right.MoveNext()) {
                    sum += right.Current.GetValueOrDefault();
                    yield return sum;
                    left.MoveNext();
                    sum -= left.Current.GetValueOrDefault();
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<int> MovingSum(this IEnumerable<int> source, int blockSize = BlockSize) {
            var sum = 0;
            var block = blockSize;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum;
                        yield break;
                    }
                    sum += right.Current;
                }

                while(right.MoveNext()) {
                    sum += right.Current;
                    yield return sum;
                    left.MoveNext();
                    sum -= left.Current;
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">변량</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<int?> MovingSum(this IEnumerable<int?> source, int blockSize = BlockSize) {
            var sum = 0;
            var block = blockSize;

            using(var left = source.GetEnumerator())
            using(var right = source.GetEnumerator()) {
                while(block > 1) {
                    block--;
                    if(!right.MoveNext()) {
                        yield return sum;
                        yield break;
                    }
                    sum += right.Current.GetValueOrDefault();
                }

                while(right.MoveNext()) {
                    sum += right.Current.GetValueOrDefault();
                    yield return sum;
                    left.MoveNext();
                    sum -= left.Current.GetValueOrDefault();
                }
            }
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<double> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, double> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<double?> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, double?> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<float> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, float> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<float?> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, float?> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<decimal> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, decimal> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<decimal?> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, decimal?> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<long> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, long> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<long?> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, long?> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<int> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, int> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }

        /// <summary>
        /// 변량(<paramref name="source"/>)로부터 이동합계을 계산합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="blockSize">합계를 구하는 변량의 갯수</param>
        /// /// <param name="selector">시퀀스 항목에서 변량 선택자</param>
        /// <returns>이동 합계 시퀀스</returns>
        public static IEnumerable<int?> MovingSum<T>(this IEnumerable<T> source, int blockSize, Func<T, int?> selector) {
            return source.Select(selector).MovingSum(blockSize);
        }
    }
}