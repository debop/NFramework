using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// F 함수의 분포를 가지는 난수 발생기
    /// </summary>
    public sealed class FRandomizer : RandomizerBase {
        private double _n1 = 1.0;
        private double _n2 = 1.0;
        private ChiSquareRandomizer _chiSquare1;
        private ChiSquareRandomizer _chiSquare2;

        /// <summary>
        /// 생성자
        /// </summary>
        public FRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n1">첫번째 <see cref="ChiSquareRandomizer.N"/> 값 </param>
        /// <param name="n2">두번째 <see cref="ChiSquareRandomizer.N"/> 값 </param>
        public FRandomizer(double n1, double n2) {
            N1 = n1;
            N2 = n2;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="c">모든<see cref="ChiSquareRandomizer.N"/> 값 </param>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생기</param>
        public FRandomizer(double c, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            N1 = c;
            N2 = c;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생기</param>
        public FRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// 첫번째 <see cref="ChiSquareRandomizer.N"/> 값 
        /// </summary>
        public double N1 {
            get { return _n1; }
            set {
                if(Math.Abs(_n1 - value) > double.Epsilon) {
                    _n1 = value;
                    _chiSquare1 = null;
                }
            }
        }

        /// <summary>
        /// 두번째 <see cref="ChiSquareRandomizer.N"/> 값 
        /// </summary>
        public double N2 {
            get { return _n2; }
            set {
                if(Math.Abs(_n2 - value) > double.Epsilon) {
                    _n2 = value;
                    _chiSquare2 = null;
                }
            }
        }

        /// <summary>
        /// 첫번째 <see cref="ChiSquareRandomizer"/>
        /// </summary>
        private ChiSquareRandomizer ChiSquare1 {
            get { return _chiSquare1 ?? (_chiSquare1 = new ChiSquareRandomizer(_n1)); }
        }

        /// <summary>
        /// 두번째 <see cref="ChiSquareRandomizer"/>
        /// </summary>
        private ChiSquareRandomizer ChiSquare2 {
            get { return _chiSquare2 ?? (_chiSquare2 = new ChiSquareRandomizer(_n2)); }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            return (ChiSquare1.NextGamma() * _n2) / (ChiSquare2.NextGamma() * _n1);
        }
    }
}