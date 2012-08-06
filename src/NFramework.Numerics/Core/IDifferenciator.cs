using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 함수의 미분값을 제공하는 Class의 Interface
    /// </summary>
    public interface IDifferenciator {
        /// <summary>
        /// 지정된 함수의 x 지점의 미분값을 구한다.
        /// </summary>
        /// <param name="func">미분할 함수</param>
        /// <param name="x">미분값을 구할 위치</param>
        /// <returns></returns>
        double Differenticate(Func<double, double> func, double x);
    }
}