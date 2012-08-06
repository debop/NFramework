using System;
using System.Threading;

namespace NSoft.NFramework.Threading {
    /// <summary>
    /// Thread 관련 Utility Class 입니다.
    /// </summary>
    public static class ThreadTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// <see cref="System.Threading.Monitor"/>.Wait 를 이용하여 Sleep 함수를 구현한 것이다.
        /// </summary>
        /// <param name="millisecondsTimeout">timeout (msec)</param>
        public static void Sleep(int millisecondsTimeout = 1) {
            var obj = new object();

            lock(obj) {
                Monitor.Wait(obj, millisecondsTimeout);
            }
        }

        /// <summary>
        /// Seed 값이 호출할 때마다 다른 Random 인스턴스를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public static Random CreateRandom() {
            return new ThreadSafeRandom();
        }
    }
}