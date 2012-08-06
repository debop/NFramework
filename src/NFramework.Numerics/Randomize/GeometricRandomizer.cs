using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 기하 분포를 따르는 난수 발생기
    /// </summary>
    /// <remarks>
    /// 확률 p로 당첨되는 현상금에 n회째의 응모로 처음 당첨될 확률은 Pn = p(1-p)^n-1 이다. 이 분포를 기하분포라 한다.
    /// </remarks>
    public sealed class GeometricRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public GeometricRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="p">확률</param>
        public GeometricRandomizer(double p) {
            Probability = p;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="p">확률</param>
        /// <param name="randumNumberFunc">사용자 정의 난수 발생 함수</param>
        public GeometricRandomizer(double p, Func<double> randumNumberFunc)
            : base(randumNumberFunc) {
            Probability = p;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randumNumberFunc">사용자 정의 난수 발생 함수</param>
        public GeometricRandomizer(Func<double> randumNumberFunc) : base(randumNumberFunc) {}

        private double _probability = 0.5;

        /// <summary>
        /// 확률 (기본값은 0.5)
        /// </summary>
        ///<exception cref="ArgumentException">확률값이 0~1 사이가 아니면 예외가 발생한다.</exception>
        public double Probability {
            get { return _probability; }
            set {
                if(Math.Abs(_probability - value) > double.Epsilon) {
                    value.ShouldBeInRange(0.0d, 1.0d, "Probability");
                    _probability = value;
                }
            }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            if(_probability < 0.3)
                return Math.Ceiling(Math.Log(1.0 - RandomNumberFunc()) / Math.Log(1.0 - _probability));

            var n = 1;
            while(RandomNumberFunc() > _probability) n++;
            return n;
        }
    }
}