namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 특수 함수 모음 (Beta, Gamma 등)
    /// </summary>
    public static partial class SpecialFunctions {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        static SpecialFunctions() {
            IntializeFactorial();
        }

        private const int FactorialMaxArgument = 170;
        private static double[] _factorialCache;

        private static void IntializeFactorial() {
            if(IsDebugEnabled)
                log.Debug(@"Factorial 값을 캐시하도록 합니다...");

            _factorialCache = new double[FactorialMaxArgument + 1];
            _factorialCache[0] = 1.0d;

            for(var i = 1; i < _factorialCache.Length; i++)
                _factorialCache[i] = _factorialCache[i - 1] * i;
        }
    }
}