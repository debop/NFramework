using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 지정된 시퀀스의 항목 중 최대/최소 값을 구한다
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<double> source, out double min, out double max) {
            source.ShouldNotBeNull("source");

            min = double.MaxValue;
            max = double.MinValue;

            foreach(var v in source.Where(x => !double.IsNaN(x))) {
                if(v < min)
                    min = v;

                if(v > max)
                    max = v;
            }
        }

        /// <summary>
        /// 지정된 시퀀스의 항목 중 최대/최소 값을 구한다
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<double?> source, out double min, out double max) {
            source.Where(x => x.HasValue).Select(x => x.Value).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 지정된 시퀀스의 최대/최소 값을 구하다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<float> source, out float min, out float max) {
            source.ShouldNotBeNull("source");

            min = float.MaxValue;
            max = float.MinValue;

            foreach(var v in source.Where(x => !float.IsNaN(x))) {
                if(v < min)
                    min = v;

                if(v > max)
                    max = v;
            }
        }

        /// <summary>
        /// 지정된 시퀀스의 최대/최소 값을 구하다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<float?> source, out float min, out float max) {
            source.Where(x => x.HasValue).Select(x => x.Value).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 지정된 시퀀스의 최대/최소 값을 구하다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<decimal> source, out decimal min, out decimal max) {
            source.ShouldNotBeNull("source");

            min = decimal.MaxValue;
            max = decimal.MinValue;

            foreach(var v in source) {
                if(v < min)
                    min = v;

                if(v > max)
                    max = v;
            }
        }

        /// <summary>
        /// 지정된 시퀀스의 최대/최소 값을 구하다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<decimal?> source, out decimal min, out decimal max) {
            source.Where(x => x.HasValue).Select(x => x.Value).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 지정된 시퀀스의 최대/최소 값을 구하다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<long> source, out long min, out long max) {
            source.ShouldNotBeNull("source");

            min = long.MaxValue;
            max = long.MinValue;

            foreach(var v in source) {
                if(v < min)
                    min = v;

                if(v > max)
                    max = v;
            }
        }

        /// <summary>
        /// 지정된 시퀀스의 최대/최소 값을 구하다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<long?> source, out long min, out long max) {
            source.Where(x => x.HasValue).Select(x => x.Value).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 지정된 시퀀스의 최대/최소 값을 구하다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<int> source, out int min, out int max) {
            source.ShouldNotBeNull("source");

            min = int.MaxValue;
            max = int.MinValue;

            foreach(var v in source) {
                if(v < min)
                    min = v;

                if(v > max)
                    max = v;
            }
        }

        /// <summary>
        /// 지정된 시퀀스의 최대/최소 값을 구하다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax(this IEnumerable<int?> source, out int min, out int max) {
            source.Where(x => x.HasValue).Select(x => x.Value).GetMinMax(out min, out max);
        }

        //! --------------------------------------------------

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, double> selector, out double min, out double max) {
            selector.ShouldNotBeNull("selector");
            source.Select(selector).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, double?> selector, out double min, out double max) {
            selector.ShouldNotBeNull("selector");
            source.Select(selector).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, float> selector, out float min, out float max) {
            selector.ShouldNotBeNull("selector");
            source.Select(selector).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, float?> selector, out float min, out float max) {
            selector.ShouldNotBeNull("selector");
            source.Select(selector).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, decimal> selector, out decimal min, out decimal max) {
            selector.ShouldNotBeNull("selector");
            source.Select(x => selector(x)).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, decimal?> selector, out decimal min, out decimal max) {
            selector.ShouldNotBeNull("selector");
            source.Select(x => selector(x)).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, long> selector, out long min, out long max) {
            selector.ShouldNotBeNull("selector");
            source.Select(x => selector(x)).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, long?> selector, out long min, out long max) {
            selector.ShouldNotBeNull("selector");
            source.Select(x => selector(x)).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, int> selector, out int min, out int max) {
            selector.ShouldNotBeNull("selector");
            source.Select(x => selector(x)).GetMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetMinMax<T>(this IEnumerable<T> source, Func<T, int?> selector, out int min, out int max) {
            selector.ShouldNotBeNull("selector");
            source.Select(x => selector(x)).GetMinMax(out min, out max);
        }

        //! --------------------------------------------------

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<double> source, out double min, out double max) {
            source.ShouldNotBeNull("source");

            min = double.MaxValue;
            max = double.MinValue;

            foreach(var v in source.Where(x => !double.IsNaN(x)).Abs()) {
                if(min > v)
                    min = v;
                if(max < v)
                    max = v;
            }
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<double?> source, out double min, out double max) {
            source.ShouldNotBeNull("source");
            source.Where(x => x.HasValue).Select(x => x.Value).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<float> source, out float min, out float max) {
            source.ShouldNotBeNull("source");

            min = float.MaxValue;
            max = float.MinValue;

            foreach(var v in source.Where(x => !float.IsNaN(x)).Abs()) {
                if(min > v)
                    min = v;
                if(max < v)
                    max = v;
            }
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<float?> source, out float min, out float max) {
            source.ShouldNotBeNull("source");
            source.Where(x => x.HasValue).Select(x => x.Value).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<decimal> source, out decimal min, out decimal max) {
            source.ShouldNotBeNull("source");

            min = decimal.MaxValue;
            max = decimal.MinValue;

            foreach(var v in source.Abs()) {
                if(min > v)
                    min = v;
                if(max < v)
                    max = v;
            }
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<decimal?> source, out decimal min, out decimal max) {
            source.ShouldNotBeNull("source");
            source.Where(x => x.HasValue).Select(x => x.Value).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<long> source, out long min, out long max) {
            source.ShouldNotBeNull("source");

            min = long.MaxValue;
            max = long.MinValue;

            foreach(var v in source.Abs()) {
                if(min > v)
                    min = v;
                if(max < v)
                    max = v;
            }
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<long?> source, out long min, out long max) {
            source.ShouldNotBeNull("source");
            source.Where(x => x.HasValue).Select(x => x.Value).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<int> source, out int min, out int max) {
            source.ShouldNotBeNull("source");

            min = int.MaxValue;
            max = int.MinValue;

            foreach(var v in source.Abs()) {
                if(min > v)
                    min = v;
                if(max < v)
                    max = v;
            }
        }

        /// <summary>
        /// 시퀀스 요소의 절대값의 최대/최소값을 구한다.
        /// </summary>
        /// <param name="source">Sequece of variable</param>
        /// <param name="min">변량의 절대값의 최소값</param>
        /// <param name="max">변량의 절대값의 최대값</param>
        public static void GetAbsMinMax(this IEnumerable<int?> source, out int min, out int max) {
            source.ShouldNotBeNull("source");
            source.Where(x => x.HasValue).Select(x => x.Value).GetAbsMinMax(out min, out max);
        }

        //! --------------------------------------------------

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, double> selector, out double min, out double max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, double?> selector, out double min, out double max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, float> selector, out float min, out float max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, float?> selector, out float min, out float max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, decimal> selector, out decimal min, out decimal max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, decimal?> selector, out decimal min, out decimal max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, long> selector, out long min, out long max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, long?> selector, out long min, out long max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, int> selector, out int min, out int max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        /// <summary>
        /// 시퀀스의 변량의 절대값의 최대, 최소 값을 구한다.
        /// </summary>
        /// <typeparam name="T">변량을 가진 객체의 형식</typeparam>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택하는 선택자</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public static void GetAbsMinMax<T>(this IEnumerable<T> source, Func<T, int?> selector, out int min, out int max) {
            source.Select(selector).GetAbsMinMax(out min, out max);
        }

        //! --------------------------------------------------

        /// <summary>
        /// 지정된 시퀀스의 값 중 가장 큰 값을 가진 요소의 인덱스를 구한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int IndexOfMax<T>(this IEnumerable<T> source) {
            var index = -1;

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext() == false)
                    return index;

                var highest = iter.Current;
                var count = 0;
                index = count;

                while(iter.MoveNext()) {
                    var current = iter.Current;
                    count++;
                    if(LinqTool.Operators<T>.GreaterThan(current, highest)) {
                        highest = current;
                        index = count;
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// 지정된 시퀀스의 값 중 가장 작은 값을 가진 요소의 인덱스를 구한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int IndexOfMin<T>(this IEnumerable<T> source) {
            var index = -1;

            using(var iter = source.GetEnumerator()) {
                if(iter.MoveNext() == false)
                    return index;

                var highest = iter.Current;
                var count = 0;
                index = count;

                while(iter.MoveNext()) {
                    var current = iter.Current;
                    count++;
                    if(LinqTool.Operators<T>.LessThan(current, highest)) {
                        highest = current;
                        index = count;
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// 지정된 시퀀스의 값 중 가장 큰 값을 가진 요소의 인덱스를 구한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int IndexOfAbsMax<T>(this IEnumerable<T> source) {
            var index = -1;

            using(var iter = source.Abs().GetEnumerator()) {
                if(iter.MoveNext() == false)
                    return index;

                var highest = iter.Current;
                var count = 0;
                index = count;

                while(iter.MoveNext()) {
                    var current = iter.Current;
                    count++;
                    if(LinqTool.Operators<T>.GreaterThan(current, highest)) {
                        highest = current;
                        index = count;
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// 지정된 시퀀스의 값 중 가장 작은 값을 가진 요소의 인덱스를 구한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int IndexOfAbsMin<T>(this IEnumerable<T> source) {
            var index = -1;

            using(var iter = source.Abs().GetEnumerator()) {
                if(iter.MoveNext() == false)
                    return index;

                var highest = iter.Current;
                var count = 0;
                index = count;

                while(iter.MoveNext()) {
                    var current = iter.Current;
                    count++;
                    if(LinqTool.Operators<T>.LessThan(current, highest)) {
                        highest = current;
                        index = count;
                    }
                }
            }
            return index;
        }

        //! --------------------------------------------------
    }
}