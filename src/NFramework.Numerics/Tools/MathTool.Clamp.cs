using System;
using System.Collections.Generic;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 지정된 값이 원하는 값과의 차이가 오차범위 내에 있다면 원하는 값으로 대체한다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="destValue">원하는 값</param>
        /// <param name="tolerance">오차</param>
        /// <returns>value와 destValue의 오차가 허용범위에 있다면 destValue, 아니면 value를 반환한다.</returns>
        public static double Clamp(this double value, double destValue, double tolerance = Epsilon) {
            return (Math.Abs(value - destValue) <= tolerance) ? destValue : value;
        }

        /// <summary>
        /// 지정된 값이 원하는 값과의 차이가 오차범위 내에 있다면 원하는 값으로 대체한다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="destValue">원하는 값</param>
        /// <param name="tolerance">오차</param>
        /// <returns>value와 destValue의 오차가 허용범위에 있다면 destValue, 아니면 value를 반환한다.</returns>
        public static float Clamp(this float value, float destValue, float tolerance = FloatEpsilon) {
            return (Math.Abs(value - destValue) <= tolerance) ? destValue : value;
        }

        /// <summary>
        /// 시퀀스 항목 값이 원하는 값과의 차이가 오차범위 내에 있다면 원하는 값으로 대체한다.
        /// </summary>
        /// <param name="source">검사할 값을 가진 시퀀스</param>
        /// <param name="destValue">원하는 값</param>
        /// <param name="tolerance">오차</param>
        /// <returns>value와 destValue의 오차가 허용범위에 있다면 destValue, 아니면 value를 반환한다.</returns>
        public static IEnumerable<T> Clamp<T>(this IEnumerable<T> source, T destValue, T tolerance) where T : IComparable<T> {
            source.ShouldNotBeNull("source");

            foreach(var v in source) {
                var abs = LinqTool.Operators<T>.Subtract(v, destValue).Abs();
                yield return LinqTool.Operators<T>.LessThanOrEqual(abs, tolerance) ? destValue : v;
            }
        }

        /// <summary>
        /// 지정된 값이 상하한을 벗어나면 벗어난 값에 가까운 한계값으로 대체된다.
        /// </summary>
        /// <param name="value">실제값</param>
        /// <param name="minValue">하한 값</param>
        /// <param name="maxValue">상한 값</param>
        /// <returns>실제값이 상한을 넘어서면 상한값을, 하한값을 벗어나면 하한값을 반환한다. 상하한 영역에 있다면 실제값을 반환한다.</returns>
        public static double RangeClamp(this double value, double minValue, double maxValue) {
            if(minValue > maxValue)
                Swap(ref minValue, ref maxValue);

            if(value < minValue)
                return minValue;

            if(value > maxValue)
                return maxValue;

            return value;
        }

        /// <summary>
        /// 지정된 값이 상하한을 벗어나면 벗어난 값에 가까운 한계값으로 대체된다.
        /// </summary>
        /// <param name="value">실제값</param>
        /// <param name="minValue">하한 값</param>
        /// <param name="maxValue">상한 값</param>
        /// <returns>실제값이 상한을 넘어서면 상한값을, 하한값을 벗어나면 하한값을 반환한다. 상하한 영역에 있다면 실제값을 반환한다.</returns>
        public static float RangeClamp(this float value, float minValue, float maxValue) {
            if(minValue > maxValue)
                Swap(ref minValue, ref maxValue);

            if(value < minValue)
                return minValue;

            if(value > maxValue)
                return maxValue;

            return value;
        }

        /// <summary>
        /// 시퀀스 항목 값이 상하한을 벗어나면 벗어난 값에 가까운 한계값으로 대체된다.
        /// </summary>
        /// <param name="source">실제값을 가진 시퀀스</param>
        /// <param name="minValue">하한 값</param>
        /// <param name="maxValue">상한 값</param>
        /// <returns>실제값이 상한을 넘어서면 상한값을, 하한값을 벗어나면 하한값을 반환한다. 상하한 영역에 있다면 실제값을 반환한다.</returns>
        public static IEnumerable<T> RangeClamp<T>(this IEnumerable<T> source, T minValue, T maxValue)
            where T : struct, IComparable<T> {
            source.ShouldNotBeNull("source");


            Guard.Assert(LinqTool.Operators<T>.GreaterThanOrEqual(maxValue, minValue),
                         @"maxValue[{0}]가 minValue[{1}]보다 커야합니다.", maxValue, minValue);

            foreach(var x in source) {
                if(LinqTool.Operators<T>.LessThan(x, minValue))
                    yield return minValue;
                else if(LinqTool.Operators<T>.GreaterThan(x, maxValue))
                    yield return maxValue;
                else
                    yield return x;
            }
        }
    }
}