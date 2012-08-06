using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// [LOW, HIGH] 범위의 Uniform(일양) 분포를 가지는 난수 발생기 (기본은 [0, 1] 범위이다)
    /// </summary>
    public sealed class UniformRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public UniformRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="min">하한값</param>
        /// <param name="max">상한값</param>
        public UniformRandomizer(double min, double max) {
            _range = new Interval<double>(min, max, IntervalKind.Closed);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="min">하한값</param>
        /// <param name="max">상한값</param>
        /// <param name="randomNumberFunc">난수 발생을 담당하는 함수</param>
        public UniformRandomizer(double min, double max, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            _range = new Interval<double>(min, max, IntervalKind.Closed);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="interval">상하한 구간</param>
        public UniformRandomizer(Interval<double> interval) {
            _range = interval;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="interval">상하한 구간</param>
        /// <param name="randomNumberFunc">난수 발생을 담당하는 함수</param>
        public UniformRandomizer(Interval<double> interval, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            _range = interval;
        }

        private Interval<double> _range = new Interval<double>(0.0, 1.0, IntervalKind.Closed);

        /// <summary>
        /// Uniform 분포의 상하한 구간을 나타낸다.
        /// </summary>
        public Interval<double> Range {
            get { return _range; }
            set { _range = value; }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            var length = _range.GetLength();
            var result = (Math.Abs(length - 1.0) < double.Epsilon)
                             ? RandomNumberFunc() + Range.Min
                             : length * RandomNumberFunc() + Range.Min;

            return result.RangeClamp(Range.Min, Range.Max);
        }
    }
}