using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 삼각 분포의 난수발생기
    /// </summary>
    public sealed class TriangularRandomizer : RandomizerBase {
        private double _lowerBound;
        private double _upperBound = 1.0;
        private double _mode = 0.5;
        private double _modeStd = 0.5;
        private double _range = 1.0;

        /// <summary>
        /// 생성자
        /// </summary>
        public TriangularRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public TriangularRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="lower">하한</param>
        /// <param name="upper">상한</param>
        /// <param name="m">중심 (삼각형의 꼭지점)</param>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public TriangularRandomizer(double lower, double upper, double m, Func<double> randomNumberFunc = null)
            : this(lower, upper, m) {
            RandomNumberFunc = randomNumberFunc;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="interval">상하한 구간</param>
        /// <param name="m">중심 (삼각형의 꼭지점)</param>
        public TriangularRandomizer(Interval<double> interval, double m) : this(interval.Min, interval.Max, m) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="lower">하한</param>
        /// <param name="upper">상한</param>
        /// <param name="m">중심 (삼각형의 꼭지점)</param>
        public TriangularRandomizer(double lower, double upper, double m)
            : this() {
            SetParameters(lower, upper, m);
        }

        /// <summary>
        /// 하한 값 (기본값은 0.0)
        /// </summary>
        public double LowerBound {
            get { return _lowerBound; }
            set {
                if(Math.Abs(value - _lowerBound) > double.Epsilon) {
                    Guard.Assert(Math.Abs(value - Mode) > double.Epsilon, "LowerBound equal Mode");

                    _lowerBound = value;
                    if(_lowerBound > _upperBound)
                        MathTool.Swap(ref _lowerBound, ref _upperBound);

                    SetParameters(_lowerBound, _upperBound, _mode);
                }
            }
        }

        /// <summary>
        /// 상한 값 (기본값은 1.0)
        /// </summary>
        public double UpperBound {
            get { return _upperBound; }
            set {
                if(Math.Abs(value - _upperBound) > double.Epsilon) {
                    Guard.Assert(Math.Abs(value - Mode) > double.Epsilon, "UpperBound equal Mode");

                    _upperBound = value;
                    if(_upperBound < _lowerBound)
                        MathTool.Swap(ref _lowerBound, ref _upperBound);

                    SetParameters(_lowerBound, _upperBound, _mode);
                }
            }
        }

        /// <summary>
        /// 삼각형의 꼭지점의 위치 (기본값은 0.5)
        /// </summary>
        public double Mode {
            get { return _mode; }
            set {
                if(Math.Abs(_mode - value) > double.Epsilon) {
                    Guard.Assert(value > _lowerBound && value < _upperBound,
                                 "Mode must in range ({0}, {1}) - 경계포함 안함(Opened)", _lowerBound, _upperBound);

                    _mode = value;
                    SetParameters(_lowerBound, _upperBound, _mode);
                }
            }
        }

        /// <summary>
        /// 상하한 구간 설정
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <param name="m"></param>
        public void SetParameters(double lower, double upper, double m) {
            if(IsDebugEnabled)
                log.Debug(@"Set parameters... lower=[{0}], upper=[{1}], m=[{2}]", lower, upper, m);

            Guard.Assert(Math.Abs(lower - upper) > double.Epsilon &&
                         Math.Abs(upper - m) > double.Epsilon &&
                         Math.Abs(m - lower) > double.Epsilon,
                         "Parameter must not be equals. lower=[{0}], upper=[{1}], m=[{2}]", lower, upper, m);

            if(lower > upper)
                MathTool.Swap(ref lower, ref upper);

            Guard.Assert(m > lower && m < upper,
                         "Mode must in range (lower, upper) - 경계포함 안함(Opened). lower=[{0}], upper=[{1}], m=[{2}]",
                         lower, upper, m);

            _lowerBound = lower;
            _upperBound = upper;
            _mode = m;
            _range = _upperBound - _lowerBound;
            _modeStd = (_mode - _lowerBound) / _range;
        }

        /// <summary>
        /// 상하한 구간 
        /// </summary>
        /// <returns></returns>
        public double GetIntervalLength() {
            return _upperBound - _lowerBound;
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            double result;
            double x = RandomNumberFunc();

            if(x <= _modeStd)
                result = Math.Sqrt(_modeStd * x);
            else
                result = 1.0 - Math.Sqrt((1.0 - _modeStd) * (1.0 - x));

            return _lowerBound + (_range * result);
        }
    }
}