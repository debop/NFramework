using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Log Normal 분포
    /// </summary>
    public class LogNormalRandomizer : RandomizerBase {
        private const double DefaultMean = 1.0;
        private const double DefaultVariance = 0.1;

        private double _mean;
        private double _variance;
        private NormalRandomizer _normalRnd;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mean">평균</param>
        /// <param name="variance">분산</param>
        /// <param name="rndFunc">사용자 정의 난수 발생 함수</param>
        public LogNormalRandomizer(double mean, double variance, Func<double> rndFunc = null)
            : base(rndFunc) {
            SetParameters(mean, variance);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mean">평균</param>
        /// <param name="variance">분산</param>
        public LogNormalRandomizer(double mean, double variance) {
            SetParameters(mean, variance);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="rndFunc">사용자 정의 난수 발생 함수</param>
        public LogNormalRandomizer(Func<double> rndFunc)
            : base(rndFunc) {
            SetParameters(DefaultMean, DefaultVariance);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public LogNormalRandomizer() : this(DefaultMean, DefaultVariance) {}

        private void SetParameters(double mean, double variance) {
            variance.ShouldBePositiveOrZero("variance");

            _normalRnd = new NormalRandomizer();
            Mean = mean;
            Variance = variance;
        }

        /// <summary>
        /// 평균
        /// </summary>
        public double Mean {
            get { return _mean; }
            set {
                value.ShouldNotBeZero("Mean");

                var v2 = value * value;
                var sv = StDev / value;

                Normal.Mean = Math.Log(v2 / Math.Sqrt(v2 + _variance));
                Normal.Variance = Math.Log((sv * sv) + 1.0);
                Recalculate();
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return _variance; }
            set {
                double m2 = _mean * _mean;
                double nv = (Math.Sqrt(value) / _mean);

                Normal.Mean = Math.Log(m2 / Math.Sqrt(m2 + value));
                Normal.Variance = Math.Log((nv * nv) + 1.0);
                Recalculate();
            }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return Math.Sqrt(Variance); }
            set {
                value.ShouldBePositiveOrZero("StDev");
                Variance = value * value;
            }
        }

        /// <summary>
        /// Instance of <see cref="NormalRandomizer"/> : N(Mean, StDev)
        /// </summary>
        protected NormalRandomizer Normal {
            get {
                if(_normalRnd == null)
                    _normalRnd = new NormalRandomizer();

                return _normalRnd;
            }
            set {
                _normalRnd = value;
                Recalculate();
            }
        }

        /// <summary>
        /// 정규분포 상의 평균 값
        /// </summary>
        public double NormalMean {
            get { return Normal.Mean; }
            set {
                if(Math.Abs(Normal.Mean - value) > double.Epsilon) {
                    Normal.Mean = value;
                    Recalculate();
                }
            }
        }

        /// <summary>
        /// 정규분포상의 분산
        /// </summary>
        public double NormalVariance {
            get { return Normal.Variance; }
            set {
                if(Math.Abs(Normal.Variance - value) > double.Epsilon) {
                    Normal.Variance = value;
                    Recalculate();
                }
            }
        }

        /// <summary>
        /// 평균, 분산을 재계산한다.
        /// </summary>
        private void Recalculate() {
            _mean = Math.Exp(Normal.Mean + (Normal.Variance / 2.0));
            _variance = Math.Exp((2.0 * Normal.Mean) + Normal.Variance) * (Math.Exp(Normal.Variance) - 1.0);

            if(IsDebugEnabled)
                log.Debug("Recalculated!!! mean=[{0}], variance=[{1}]", _mean, _variance);
        }

        /// <summary>
        /// 난수 발생 함수를 초기화 한다.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public override bool Reset(int? seed) {
            return _normalRnd.Reset(seed);
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            return Math.Exp(_normalRnd.Next());
        }
    }
}