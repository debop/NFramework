using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Weibull 분포를 따르는 Random Generator
    /// </summary>
    /// <remarks>
    /// <para>
    /// 기계 구입 후 시간에 따라 발생하는 고장 확률에 대한 분포를 표현한다.
    /// </para>
    /// <para>
    /// alpha &lt; 1 이면 초기 고장형, 
    /// alpha = 1 이면 지수분포이며, 우발 고장형,
    /// alpha &gt; 1 이면 마모 고장형이라 한다.
    /// </para>
    /// </remarks>
    public sealed class WeibullRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public WeibullRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="alpha"></param>
        public WeibullRandomizer(double alpha) {
            _alpha = alpha;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc"></param>
        public WeibullRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="randomNumberFunc"></param>
        public WeibullRandomizer(double alpha, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            _alpha = alpha;
        }

        private double _alpha = 1.0;

        /// <summary>
        /// alpha &lt; 1 이면 초기 고장형, 
        /// alpha = 1 이면 지수분포이며, 우발 고장형, 
        /// alpha &gt; 1 이면 마모 고장형이라 한다.
        /// 기본값은 1.0 이다.
        /// </summary>
        public double Alpha {
            get { return _alpha; }
            set { _alpha = value; }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            return Math.Pow(-Math.Log(1.0 - RandomNumberFunc()), 1.0 / Alpha);
        }
    }
}