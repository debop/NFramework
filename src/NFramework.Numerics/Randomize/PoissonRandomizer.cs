using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 시간당 발생하는 횟수의 평균이 lambda인 Poisson 분포를 가지는 Random 함수
    /// </summary>
    /// <remarks>
    /// <para>
    /// 단위시간당 일어난 횟수의 평균값을 가지고 실제 단위사간당 일어나는 횟수에 대한 난수를 발생시킨다.
    /// </para>
    /// <para>
    /// 단위시간에 접수창고에 오는 손님의 수, 고속도로에서 단위시간에 통과하는 차량의 수 등은 Poisson 분포에 따른다.
    /// </para>
    /// </remarks>
    public class PoissonRandomizer : RandomizerBase {
        private double _mean = 1.0;
        private double _g;
        private double _sq;
        private double _alxm;
        private double _oldm = -1;

        /// <summary>
        /// 단위시간당 발생하는 평균 횟수 (기본값은 1.0)
        /// </summary>
        public double Mean {
            get { return _mean; }
            set { _mean = value; }
        }

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public PoissonRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생기</param>
        public PoissonRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mean">단위시간당 발생하는 평균 횟수 (기본값은 1.0)</param>
        public PoissonRandomizer(double mean) {
            Mean = mean;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mean">단위시간당 발생하는 평균 횟수 (기본값은 1.0)</param>
        /// <param name="randomNumberFunc"></param>
        public PoissonRandomizer(double mean, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            Mean = mean;
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            int x;
            double s = 1.0;

            if(_mean < 12.0) {
                if(_mean.ApproximateEqual(_oldm) == false) {
                    _oldm = _mean;
                    _g = Math.Exp(-_mean);
                }

                x = -1;

                do {
                    x++;
                    s *= RandomNumberFunc();
                } while(s > _g);
            }
            else {
                if(_mean.ApproximateEqual(_oldm) == false) {
                    _oldm = _mean;
                    _sq = Math.Sqrt(2.0 * _mean);
                    _alxm = Math.Log(_mean);
                    _g = (_mean * _alxm) - (_mean + 1.0).GammaLn();
                }

                do {
                    double s2 = Math.Tan(MathTool.Pi * RandomNumberFunc());

                    x = (int)((_sq * s2) + _mean);

                    if(x < 0)
                        continue;

                    s = 0.9 * (1.0 + (s2 * s2)) * Math.Exp((x * _alxm) - (x + 1.0).GammaLn() - _g);
                } while(RandomNumberFunc() > s);
            }
            return x;
        }
    }
}