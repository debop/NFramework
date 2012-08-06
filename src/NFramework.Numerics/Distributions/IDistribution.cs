using System;

namespace NSoft.NFramework.Numerics.Distributions {
    /// <summary>
    /// 분포 인터페이스
    /// </summary>
    public interface IDistribution {
        /// <summary>
        /// 난수 발생기
        /// TODO: Randomizer로 명칭 변경 
        /// </summary>
        Random RandomSource { get; set; }

        /// <summary>
        /// 평균
        /// </summary>
        double Mean { get; }

        /// <summary>
        /// 분산
        /// </summary>
        double Variance { get; }

        /// <summary>
        /// 표준편차
        /// </summary>
        double StDev { get; }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        double Entropy { get; }

        /// <summary>
        /// 기울기
        /// </summary>
        double Skewness { get; }

        /// <summary>
        /// 첨도 (뽀족한 정도) (+) 값이면 뾰족하고, (-) 값이면 뭉툭하다
        /// </summary>
        double Kurtosis { get; }

        /// <summary>
        /// 확률 분포 계산을 위한 누적분포함수
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative distribution function.</param>
        /// <returns>the cumulative distribution at location <paramref name="x"/>.</returns>
        double CumulativeDistribution(double x);
    }
}