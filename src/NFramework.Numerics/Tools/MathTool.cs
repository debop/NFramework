using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 수학 함수를 제공합니다.
    /// </summary>
    public static partial class MathTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();

        /// <summary>
        /// Double에서의 Epsilon (1.0E-10)
        /// </summary>
        public const double Epsilon = double.Epsilon; //1.0E-20;

        public const float FloatEpsilon = float.Epsilon; // 1.0E-10f;

        /// <summary>The number e</summary>
        public const double E = 2.7182818284590452353602874713526624977572470937000d;

        /// <summary>The number pi</summary>
        public const double Pi = 3.1415926535897932384626433832795028841971693993751d;

        /// <summary>The number 2*pi</summary>
        public const double Pi2 = 6.2831853071795864769252867665590057683943387987502d;

        /// <summary>
        /// PI / 2.0
        /// </summary>
        public const double PiOver2 = Pi / 2.0d;

        /// <summary>
        /// PI 제곱 (pi^2)
        /// </summary>
        public const double SquarePI = Pi * Pi;

        /// <summary>
        /// PI 제곱근 sqrt(pi)
        /// </summary>
        public static readonly double SqrtPi = Math.Sqrt(Pi);

        /// <summary>
        /// sqrt(2*pi)
        /// </summary>
        public static readonly double SqrtPi2 = Math.Sqrt(Pi * 2.0);

        /// <summary>
        /// sqrt(2*pi*e)
        /// </summary>
        public static readonly double SqrtPi2E = Math.Sqrt(Pi * 2.0 * E);

        /// <summary>
        /// log[e](sqrt(2*pi))
        /// </summary>
        public static readonly double LnSqrtPi2 = Math.Log(SqrtPi2);

        /// <summary>
        /// log[e](sqrt(2*pi*e))
        /// </summary>
        public static readonly double LnSqrtPi2E = Math.Log(SqrtPi2E);

        /// <summary>
        /// log[e](2 * sqrt(e/pi))
        /// </summary>
        public static readonly double Ln2SqrtEOverPi = Math.Log(2 * Math.Sqrt(E / Pi));

        /// <summary>
        /// 1 / sqrt(pi)
        /// </summary>
        public static readonly double InvSqrtPi = 1.0 / SqrtPi;

        /// <summary>
        /// 1 / sqrt(2*pi)
        /// </summary>
        public static readonly double InvSqrtPi2 = 1.0 / SqrtPi2;

        /// <summary>
        /// 2 * sqrt(e/pi))
        /// </summary>
        public static readonly double TwoSqrtEOverPi = 2.0 * Math.Sqrt(E / Pi);

        /// <summary>
        /// log[2](e)
        /// </summary>
        public static readonly double Log2E = Math.Log(E, 2.0);

        /// <summary>
        /// log[10](e)
        /// </summary>
        public static readonly double Log10E = Math.Log10(E);

        /// <summary>
        /// log[e](2)
        /// </summary>
        public static readonly double Ln2 = Math.Log(2);

        /// <summary>
        /// log[e](10)
        /// </summary>
        public static readonly double Ln10 = Math.Log(10);

        /// <summary>
        /// 2PI의 Log값 log[e](2*pi)
        /// </summary>
        public static readonly double LnPI2 = Math.Log(Pi2);

        /// <summary>
        /// log[e](pi)
        /// </summary>
        public static readonly double LnPI = Math.Log(Pi);

        /// <summary>
        /// 1/e
        /// </summary>
        public static readonly double InvE = 1.0d / E;

        /// <summary>
        /// sqrt(e)
        /// </summary>
        public static readonly double SqrtE = Math.Sqrt(E);

        /// <summary>
        /// sqrt(2)
        /// </summary>
        public static readonly double Sqrt2 = Math.Sqrt(2.0);

        /// <summary>
        /// 각도 (degree 를 radian으로 변환하기 위한 factor) (Pi / 180)
        /// </summary>
        public const double Degree = Pi / 180.0;

        /// <summary>
        /// ln(10) / 20 - Power Decibel (dB) 를 Neper (Np) 로 변환할 때의 factor
        /// Use this version when the Decibel represent a power gain but the compared values are not powers (e.g. amplitude, current, voltage).
        /// </summary>
        public static double PowerDecibel = Math.Log(10) / 20.0;

        /// <summary>
        /// ln(10) / 10 - Neutral Decibel (dB)를 Neper (Np)로 변환할 때의 factor
        /// Use this version when the Decibel represent a power gain but the compared values are not powers (e.g. amplitude, current, voltage).
        /// </summary>
        public static double NeutralDecibel = Math.Log(10) / 10.0;

        /// <summary>황금비 (Golden Ratio) (1+sqrt(5))/2</summary>
        public static readonly double GoldenRatio = (1.0 + Math.Sqrt(5.0)) / 2.0;

        /// <summary>The Catalan constant</summary>
        /// <remarks>Sum(k=0 -> inf){ (-1)^k/(2*k + 1)2 }</remarks>
        public const double Catalan = 0.9159655941772190150546035149323841107741493742816721342664981196217630197762547694794d;

        /// <summary>The Euler-Mascheroni constant</summary>
        /// <remarks>lim(n -> inf){ Sum(k=1 -> n) { 1/k - log(n) } }</remarks>
        public const double EulerMascheroni = 0.5772156649015328606065120900824024310421593359399235988057672348849d;

        /// <summary>The Glaisher constant</summary>
        /// <remarks>e^(1/12 - Zeta(-1))</remarks>
        public const double Glaisher = 1.2824271291006226368753425688697917277676889273250011920637400217404063088588264611297d;

        /// <summary>The Khinchin constant</summary>
        /// <remarks>prod(k=1 -> inf){1+1/(k*(k+2))^log(k,2)}</remarks>
        public const double Khinchin = 2.6854520010653064453097148354817956938203822939944629530511523455572188595371520028011d;

        /// <summary>
        /// 해를 찾기 위한 기본 시되 횟수 (100회)
        /// </summary>
        public const int DefaultTryCount = 10000;

        /// <summary>
        /// 블록의 기본 크기 (구간을 이용한 이동평균이나 합을 구할때 사용)
        /// </summary>
        public const int BlockSize = 4;

        /// <summary>
        /// 2의 제곱의 배열
        /// </summary>
        internal static readonly int[] pow2 = new[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768 };

        public static int Pow2(int index) {
            return pow2[index];
        }
    }
}