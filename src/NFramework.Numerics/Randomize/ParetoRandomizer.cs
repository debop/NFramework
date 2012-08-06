using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Pareto 분포를 가지는 난수발생기
    /// </summary>
    public sealed class ParetoRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public ParetoRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public ParetoRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="c">Pareto Shape Parameter (default: 1.0) (양수여야 합니다.)</param>
        public ParetoRandomizer(double c) {
            C = c;
        }

        /// <summary>
        /// 생성자 
        /// </summary>
        /// <param name="c">Pareto Shape Parameter (default: 1.0) (양수여야 합니다.)</param>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public ParetoRandomizer(double c, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            C = c;
        }

        private double _c = 1.0;

        /// <summary>
        /// Pareto Shape Parameter (default: 1.0) (양수여야 합니다.)
        /// </summary>
        public double C {
            get { return _c; }
            set {
                value.ShouldBePositive("C");
                _c = value;
            }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            return Math.Pow(RandomNumberFunc(), -1.0 / C);
        }
    }
}