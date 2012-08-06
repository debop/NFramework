using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions {
    /// <summary>
    /// 이산 분포 (Discrete Distribution)의 인터페이스
    /// </summary>
    public interface IDiscreteDistribution : IDistribution {
        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        int Mode { get; }

        /// <summary>
        /// 중앙값
        /// </summary>
        int Median { get; }

        /// <summary>
        /// 최소값
        /// </summary>
        int Minumum { get; }

        /// <summary>
        /// 최대값
        /// </summary>
        int Maximum { get; }

        /// <summary>
        /// 분포의 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        double Probability(int k);

        /// <summary>
        /// 분포의 Log 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        double ProbabilityLn(int k);

        /// <summary>
        /// 분포의 데이타를 반환합니다.
        /// </summary>
        /// <returns></returns>
        int Sample();

        /// <summary>
        /// 분포의 무작위 데이타를 열거합니다.
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> Samples();
    }
}