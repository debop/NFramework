using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 적분을 수행하는 인터페이스
    /// </summary>
    public interface IIntegrator {
        /// <summary>
        /// 함수의 [a,b] 구간을 적분합니다.
        /// </summary>
        /// <param name="func">적분할 함수</param>
        /// <param name="a">적분 시작 위치</param>
        /// <param name="b">적분 끝 위치</param>
        /// <returns>적분 값</returns>
        double Integrate(Func<double, double> func, double a, double b);
    }
}