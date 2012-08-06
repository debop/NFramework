using System;
using System.Diagnostics;

namespace NSoft.NFramework {
    public static partial class With {
        /// <summary>
        /// 지정된 함수를 수행하는데 걸리는 시간 (msec)을 검사한다.
        /// </summary>
        /// <param name="gabageCollect">시간측정을 더 정밀하게 하기 위해 GabageCollect()를 수행하고 할 것인가?</param>
        /// <param name="action">시간 측정이 필요한 함수</param>
        /// <returns>지정된 함수 수행 시간 (milliseconds)</returns>
        /// <example>
        /// <code>
        /// int msec = With.OperationTime(false,
        ///			   delegate 
        ///            {
        ///                // some code...
        ///            });
        /// </code>
        /// </example>
        public static double OperationTimer(Action action, bool gabageCollect = false) {
            action.ShouldNotBeNull("action");

            if(gabageCollect)
                GabageCollect();

            var stopwatch = new Stopwatch();

            try {
                stopwatch.Start();

                action();
            }
            finally {
                stopwatch.Stop();
            }

            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// 성능 검사를 하기 위해서는 메모리 청소를 먼저 수행해야 한다.
        /// </summary>
        private static void GabageCollect() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForFullGCComplete();
        }
    }
}