using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Power distribution을 가지는 난수 발생기
    /// </summary>
    /// <remarks>
    /// <para>
    /// [0,1) 의 Uniform Distribution을 n+1개 만들면 그 최대값은 밀도 함수 f(x) = (n+1)*X^n ( 0 &lt;= x &lt; 1 )을 따른다.
    /// </para>
    /// <para>
    /// 거듭제곱 분포 함수 F(x) = x^(n+1) 의 역함수인 F-1(x) = x^(1/(n+1)) 로 난수를 발생시킨다.
    /// </para>
    /// </remarks>
    public class PowerRandomizer : RandomizerBase {
        /// <summary>
        /// 기본 생성자
        /// </summary>
        public PowerRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n">Power Parameter (default: 1) (양수만 가능)</param>
        public PowerRandomizer(int n) {
            N = n;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n">Power Parameter (default: 1) (양수만 가능)</param>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public PowerRandomizer(int n, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            N = n;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public PowerRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        private int _n = 1;

        /// <summary>
        /// Power Parameter (default: 1) (양수만 가능)
        /// </summary>
        public int N {
            get { return _n; }
            set {
                value.ShouldBePositive("N");
                _n = value;
            }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            return Math.Pow(RandomNumberFunc(), 1.0 / (N + 1.0));
        }
    }
}