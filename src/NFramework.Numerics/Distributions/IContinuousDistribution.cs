using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions {
    /// <summary>
    /// 연속 분포 (Continuous Distribution)의 인터페이스
    /// </summary>
    public interface IContinuousDistribution : IDistribution {
        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        double Mode { get; }

        /// <summary>
        /// 중앙값
        /// </summary>
        double Median { get; }

        /// <summary>
        /// 최소값
        /// </summary>
        double Minumum { get; }

        /// <summary>
        /// 최대값
        /// </summary>
        double Maximum { get; }

        /// <summary>
        /// 분포의 확률 밀도
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        double Density(double x);

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        double DensityLn(double x);

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        double Sample();

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        IEnumerable<double> Samples();
    }
}