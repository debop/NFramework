using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 정규분포를 가지는 난수 발생기
    /// </summary>
    public class NormalRandomizer : RandomizerBase {
        private double _mean;
        private double _variance = 1.0;
        private double _stdev = 1.0;
        private double _rndNumber;
        private bool _haveOne; // 난수 발생 속도를 높히기 위해

        /// <summary>
        /// N(0,1) 의 정규분포를 가지는 난수발생기 생성
        /// </summary>
        public NormalRandomizer() {}

        /// <summary>
        /// N(0,1) 의 정규분포를 가지는 난수발생기 생성
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public NormalRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// N(mean,stdev) 의 정규분포를 가지는 난수발생기 생성
        /// </summary>
        /// <param name="mean">정규분포의 평균 값</param>
        /// <param name="stdev">정규분포의 표준편차</param>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public NormalRandomizer(double mean, double stdev, Func<double> randomNumberFunc = null)
            : this(mean, stdev) {
            RandomNumberFunc = randomNumberFunc;
        }

        /// <summary>
        /// N(mean,stdev) 의 정규분포를 가지는 난수발생기 생성
        /// </summary>
        /// <param name="mean">정규분포의 평균 값</param>
        /// <param name="stdev">정규분포의 표준편차</param>
        public NormalRandomizer(double mean, double stdev)
            : this() {
            stdev.ShouldBePositiveOrZero("stdev");

            _mean = mean;
            _stdev = stdev;
            _variance = stdev * stdev;
        }

        /// <summary>
        /// 평균
        /// </summary>
        public double Mean {
            get { return _mean; }
            set { _mean = value; }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return _stdev; }
            set {
                value.ShouldBePositiveOrZero("StDev");

                _stdev = value;
                _variance = value * value;
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return _variance; }
            set {
                value.ShouldBePositiveOrZero("Variance");

                _variance = value;
                _stdev = Math.Sqrt(_variance);
            }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            if(_haveOne) {
                //if (IsDebugEnabled)
                //    log.Debug(@"기존에 발급받은 Random 변수가 있으므로, 그 값을 반환합니다. return=" + _rndNumber);

                _haveOne = !_haveOne;
                return _rndNumber;
            }

            double r1, r2;
            double s;
            do {
                r1 = 2.0 * RandomNumberFunc() - 1.0;
                r2 = 2.0 * RandomNumberFunc() - 1.0;
                s = MathTool.Norm(r1, r2);
            } while(s > 1.0 || Math.Abs(s - 0.0) < double.Epsilon);

            s = Math.Sqrt(-2.0 * Math.Log(s) / s);

            r1 *= s;
            r2 *= s;

            _rndNumber = _mean + r1 * _stdev;
            _haveOne = true;

            return _mean + r2 * _stdev;
        }
    }
}