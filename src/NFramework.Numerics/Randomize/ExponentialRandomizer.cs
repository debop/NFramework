using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 지수분포를 가지는 Random 함수 (분포의 평균은 1 / Lambda 가 된다) 
    /// </summary>
    public sealed class ExponentialRandomizer : RandomizerBase {
        private double _lambda = 1.0;

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public ExponentialRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="lambda">분포의 평균은 1 / Lambda 가 된다</param>
        public ExponentialRandomizer(double lambda)
            : this() {
            _lambda = lambda;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public ExponentialRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// 생성자 
        /// </summary>
        /// <param name="lambda">분포의 평균은 1 / Lambda 가 된다</param>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public ExponentialRandomizer(double lambda, Func<double> randomNumberFunc)
            : base(randomNumberFunc) {
            _lambda = lambda;
        }

        /// <summary>
        /// 난수의 평균은 1 / lambda 가 된다. 기본값은 1 이다.
        /// </summary>
        public double Lambda {
            get { return _lambda; }
            set { _lambda = value; }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            double r = (2.0 * RandomNumberFunc()) - 1.0;

            return -Math.Log((r + 1.0) / 2.0) / _lambda;
        }
    }
}