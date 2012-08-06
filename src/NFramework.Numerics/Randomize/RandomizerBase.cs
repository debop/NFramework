using System;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 기본 난수 발생기
    /// </summary>
    public abstract class RandomizerBase : IRandomizer {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string ResetMethodName = @"Initialize";

        /// <summary>
        /// 난수를 발생시키는 함수 (사용자가 지정할 수 있다.)
        /// </summary>
        [CLSCompliant(false)] protected Func<double> _randomNumberFunc;

        // private static readonly double[] _cof;
        private static readonly System.Random _defaultRandomGenerator;

        /// <summary>
        /// static constructor
        /// </summary>
        static RandomizerBase() {
            // _cof = new double[] { 76.1801, -86.5053, 24.0141, -1.23174, 0.00120865, -5.39524e-006 };
            _defaultRandomGenerator = ThreadTool.CreateRandom();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        protected RandomizerBase() {
            // NOTE: System.Random이 난수를 일양분포로 제공하므로, 그것을 기본적으로 사용한다.
            _randomNumberFunc = _defaultRandomGenerator.NextDouble;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        protected RandomizerBase(Func<double> randomNumberFunc = null) {
            _randomNumberFunc = randomNumberFunc ?? _defaultRandomGenerator.NextDouble;
        }

        /// <summary>
        /// 난수 발생기
        /// </summary>
        public static Random RandomGenerator {
            get { return _defaultRandomGenerator; }
        }

        /// <summary>
        /// 난수를 발생시키는 함수
        /// </summary>
        public Func<double> RandomNumberFunc {
            get { return _randomNumberFunc; }
            set {
                if(value != null)
                    _randomNumberFunc = value;
            }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public abstract double Next();

        /// <summary>
        /// 일양분포의 난수값을 정규분포의 난수값으로 변환한다.
        /// </summary>
        /// <param name="v">Uniform 분포에서의 변수</param>
        /// <param name="mean">정규분포의 평균</param>
        /// <param name="stdev">정규분포의 표준편차</param>
        /// <returns>난수 발생 분포(밀도 함수)를 정규분포로 변환했을 때의 난수 값</returns>
        protected virtual double Normalize(double v, double mean, double stdev) {
            return v * stdev + mean;
        }

        /// <summary>
        /// 일양분포의 난수값을 정규분포의 난수값으로 변환한다.
        /// </summary>
        /// <param name="v">Uniform 분포에서의 변수</param>
        /// <param name="mean">정규분포의 평균</param>
        /// <param name="stdev">정규분포의 표준편차</param>
        /// <returns>난수 발생 분포(밀도 함수)를 정규분포로 변환했을 때의 난수 값</returns>
        protected void Normalize(double[] v, double mean, double stdev) {
            if(v == null || v.Length == 0)
                return;

            var length = v.Length;

            unsafe {
                fixed(double* vp = &v[0]) {
                    for(int i = 0; i < length; i++)
                        vp[i] = vp[i] * stdev + mean;
                }
            }
        }

        /// <summary>
        /// 난수를 발생시켜서 지정된 배열에 채운다
        /// </summary>
        /// <param name="v">난수를 담을 배열</param>
        public virtual void Fill(double[] v) {
            v.ShouldNotBeNull("v");

            if(v.Length == 0)
                return;

            var length = v.Length;

            unsafe {
                fixed(double* vp = &v[0]) {
                    for(int i = 0; i < length; i++)
                        vp[i] = Next();
                }
            }
        }

        /// <summary>
        /// 난수를 발생시켜서 <see cref="Normalize(double[],double,double)"/>를 수행한다.
        /// </summary>
        /// <param name="v">난수를 담을 배열</param>
        /// <param name="mean">정규분포의 평균</param>
        /// <param name="stdev">정규분포의 표준편차</param>
        public virtual void Fill(double[] v, double mean, double stdev) {
            v.ShouldNotBeNull("v");

            Fill(v);
            Normalize(v, mean, stdev);
        }

        /// <summary>
        /// 난수 발생 함수를 초기화 한다.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public virtual bool Reset(int? seed) {
            // 대리자를 호출하는 함수에 Initialize Method가 있다면 그 함수를 호출한다.
            var target = _randomNumberFunc.Target;

            if(target != null) {
                var t = target.GetType();
                var types = new[] { typeof(int) };
                var mi = t.GetMethod(ResetMethodName, types);

                if(mi != null) {
                    if(mi.ReturnType.Equals(typeof(void))) {
                        mi.Invoke(target, seed.HasValue ? new object[] { seed } : new object[] { });
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 초기화한다.
        /// </summary>
        /// <returns></returns>
        public virtual bool Reset() {
            return Reset(null);
        }
    }
}