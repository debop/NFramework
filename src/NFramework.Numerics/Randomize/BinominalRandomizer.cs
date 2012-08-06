using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 시도 횟수와 확률이 주어지면 그 확률을 만족하는 횟수를 Random하게 반환한다.
    /// 주사위의 한면이 나올 확률이 1/6 인데, 시도 횟수가 증가함에 따라 1/6에 수렴하게 된다.
    /// </summary>
    public sealed class BinominalRandomizer : RandomizerBase {
        private readonly int _trials;
        private readonly double _probability;

        private static double POld;
        private static double Pc;
        private static double PLog;
        private static double PcLog;
        private static double En;
        private static double OldG;
        private static int NOld;

        static BinominalRandomizer() {
            POld = -1.0;
            NOld = -1;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="trials">시도 횟수</param>
        /// <param name="probability">확률</param>
        public BinominalRandomizer(int trials, double probability) {
            _trials = trials;
            _probability = probability;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="trials">시도 횟수</param>
        /// <param name="probability">확률</param>
        /// <param name="rndFunc"></param>
        public BinominalRandomizer(int trials, double probability, Func<double> rndFunc = null)
            : base(rndFunc) {
            _trials = trials;
            _probability = probability;
        }

        /// <summary>
        /// 확률
        /// </summary>
        public double Probability {
            get { return _probability; }
        }

        /// <summary>
        /// 시도 횟수
        /// </summary>
        public double Trials {
            get { return _trials; }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            double r;
            double v, v1, v2, v3, v4, v5, v6, v7, v8;

            v4 = (_probability < 0.5) ? _probability : (1.0 - _probability);
            v = _trials * v4;
            v6 = 1.0;

            if(_trials < 25) {
                r = 0;
                for(int i = 0; i < _trials; i++)
                    if(RandomNumberFunc() < v4)
                        r++;
            }
            else if(v < 1.0) {
                v2 = Math.Exp(-1.0 * v);
                v6 = 1.0;

                int i;
                for(i = 0; i <= _trials; i++) {
                    v6 *= RandomNumberFunc();

                    if(v6 < v2)
                        break;
                }
                r = (i <= _trials) ? i : _trials;
            }
            else {
                if(_trials != NOld) {
                    En = _trials;
                    OldG = (En + 1.0).GammaLn();
                    NOld = _trials;
                }

                if(v4 != BinominalRandomizer.POld) {
                    Pc = 1.0 - v4;
                    PLog = Math.Log(v4);
                    PcLog = Math.Log(Pc);
                    POld = v4;
                }

                v5 = Math.Sqrt(2.0 * v * Pc);
                v8 = En + 1.0;

                do {
                    v3 = MathTool.Pi * RandomNumberFunc();
                    v7 = Math.Tan(v3);

                    v1 = (v5 + v7) + v;

                    if(v1 < 0.0) continue;
                    if(v1 >= v8) continue;

                    v1 = Math.Floor(v1);
                    v6 = 1.2 * v5 * (1.0 + v7 * v7)
                         * Math.Exp((OldG
                                     - (v1 + 1.0).GammaLn()
                                     - ((En - v1) + 1.0).GammaLn()
                                    )
                                    + (v1 * PLog)
                                    + ((En - v1) * PcLog));
                } while(RandomNumberFunc() > v6);

                r = (int)v1;
            }

            if(v4 != _probability)
                r = _trials - r;

            return r;
        }
    }
}